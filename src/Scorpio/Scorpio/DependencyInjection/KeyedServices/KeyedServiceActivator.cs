#if !NET8_0_OR_GREATER
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

using Microsoft.Extensions.DependencyInjection;

namespace Scorpio.DependencyInjection.KeyedServices
{
    /// <summary>
    /// 键控感知的实例激活器。
    /// 负责按 Microsoft DI 的构造器选择规则创建实例，并处理
    /// <see cref="FromKeyedServicesAttribute"/> 和 <see cref="ServiceKeyAttribute"/> 构造注入。
    /// </summary>
    internal sealed class KeyedServiceActivator
    {
        /// <summary>
        /// 普通服务类型索引，用于判断普通构造参数是否可解析。
        /// </summary>
        private readonly OrdinaryServiceRegistry _ordinaryServices;

        /// <summary>
        /// 缓存实现类型与键组合对应的构造器解析方案，避免重复反射。
        /// </summary>
        private readonly ConcurrentDictionary<ServiceIdentifier, ConstructorPlan> _planCache =
            new ConcurrentDictionary<ServiceIdentifier, ConstructorPlan>();

        /// <summary>
        /// 保存当前解析链，用于检测键控服务之间的循环依赖。
        /// </summary>
        private readonly AsyncLocal<HashSet<ServiceIdentifier>> _resolutionChain =
            new AsyncLocal<HashSet<ServiceIdentifier>>();

        /// <summary>
        /// 初始化 <see cref="KeyedServiceActivator"/> 类的新实例。
        /// </summary>
        /// <param name="ordinaryServices">普通服务类型索引</param>
        public KeyedServiceActivator(OrdinaryServiceRegistry ordinaryServices)
            => _ordinaryServices = ordinaryServices ?? throw new ArgumentNullException(nameof(ordinaryServices));

        /// <summary>
        /// 创建指定实现类型的实例。
        /// </summary>
        /// <param name="implementationType">实现类型</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">当前解析使用的键，可以为 null</param>
        /// <param name="serviceProvider">用于解析构造参数的服务提供程序</param>
        /// <returns>创建完成的实现类型实例</returns>
        public object CreateInstance(
            Type implementationType,
            Type serviceType,
            object serviceKey,
            IServiceProvider serviceProvider)
        {
            var plan = GetPlan(implementationType, serviceKey, serviceProvider);

            // 使用可跨异步上下文流动的链检测键控服务循环依赖
            var chain = _resolutionChain.Value;
            var ownsChain = chain == null;
            if (ownsChain)
            {
                chain = new HashSet<ServiceIdentifier>();
                _resolutionChain.Value = chain;
            }

            var identifier = new ServiceIdentifier(implementationType, serviceKey);
            if (!chain.Add(identifier))
            {
                throw new InvalidOperationException(
                    $"A circular dependency was detected for the service of type '{serviceType}'.");
            }

            try
            {
                return InvokePlan(plan, serviceKey, serviceProvider);
            }
            finally
            {
                chain.Remove(identifier);
                if (ownsChain)
                {
                    _resolutionChain.Value = null;
                }
            }
        }

        /// <summary>
        /// 获取或构建指定实现类型和键的构造器解析方案。
        /// </summary>
        /// <param name="implementationType">实现类型</param>
        /// <param name="serviceKey">当前解析使用的键，可以为 null</param>
        /// <param name="serviceProvider">用于查询服务可解析性的服务提供程序</param>
        /// <returns>构造器解析方案</returns>
        private ConstructorPlan GetPlan(
            Type implementationType,
            object serviceKey,
            IServiceProvider serviceProvider)
            => _planCache.GetOrAdd(
                new ServiceIdentifier(implementationType, serviceKey),
                identifier => BuildPlan(implementationType, serviceKey, serviceProvider));

        /// <summary>
        /// 按 Microsoft DI 规则选择构造器并构建参数解析方案。
        /// </summary>
        /// <param name="implementationType">实现类型</param>
        /// <param name="serviceKey">当前解析使用的键，可以为 null</param>
        /// <param name="serviceProvider">用于查询服务可解析性的服务提供程序</param>
        /// <returns>构造器解析方案</returns>
        private ConstructorPlan BuildPlan(
            Type implementationType,
            object serviceKey,
            IServiceProvider serviceProvider)
        {
            var constructors = implementationType.GetConstructors();
            if (constructors.Length == 0)
            {
                throw new InvalidOperationException(
                    $"A suitable constructor for type '{implementationType}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.");
            }

            if (constructors.Length == 1)
            {
                var parameters = constructors[0].GetParameters();
                var plans = CreateParameterPlans(parameters, serviceKey, serviceProvider, implementationType, true);
                return new ConstructorPlan(constructors[0], plans);
            }

            // 按参数数量降序依次尝试，选择参数均可解析且数量最多的构造器
            Array.Sort(constructors, (left, right) => right.GetParameters().Length.CompareTo(left.GetParameters().Length));

            ConstructorPlan bestPlan = null;
            HashSet<Type> bestParameterTypes = null;
            for (var i = 0; i < constructors.Length; i++)
            {
                var parameters = constructors[i].GetParameters();
                var plans = CreateParameterPlans(parameters, serviceKey, serviceProvider, implementationType, false);
                if (plans == null)
                {
                    continue;
                }

                if (bestPlan == null)
                {
                    bestPlan = new ConstructorPlan(constructors[i], plans);
                    continue;
                }

                if (bestParameterTypes == null)
                {
                    bestParameterTypes = new HashSet<Type>();
                    foreach (var parameter in bestPlan.Constructor.GetParameters())
                    {
                        bestParameterTypes.Add(parameter.ParameterType);
                    }
                }

                foreach (var parameter in parameters)
                {
                    if (!bestParameterTypes.Contains(parameter.ParameterType))
                    {
                        throw new InvalidOperationException(
                            $"Unable to activate type '{implementationType}'. The following constructors are ambiguous:" +
                            Environment.NewLine + bestPlan.Constructor + Environment.NewLine + constructors[i]);
                    }
                }
            }

            if (bestPlan == null)
            {
                throw new InvalidOperationException(
                    $"No constructor for type '{implementationType}' can be instantiated using services from the service container and default values.");
            }

            return bestPlan;
        }

        /// <summary>
        /// 为构造器的全部参数构建解析方案。
        /// </summary>
        /// <param name="parameters">构造器参数</param>
        /// <param name="serviceKey">当前解析使用的键，可以为 null</param>
        /// <param name="serviceProvider">服务提供程序</param>
        /// <param name="implementationType">实现类型</param>
        /// <param name="throwIfNotFound">参数不可解析时是否立即抛出异常</param>
        /// <returns>参数解析方案数组；如果存在不可解析参数且不要求抛异常，则返回 null</returns>
        private ParameterPlan[] CreateParameterPlans(
            ParameterInfo[] parameters,
            object serviceKey,
            IServiceProvider serviceProvider,
            Type implementationType,
            bool throwIfNotFound)
        {
            var plans = new ParameterPlan[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var plan = CreateParameterPlan(parameters[i], serviceKey, serviceProvider);
                if (plan == null)
                {
                    if (throwIfNotFound)
                    {
                        throw new InvalidOperationException(
                            $"Unable to resolve service for type '{parameters[i].ParameterType}' while attempting to activate '{implementationType}'.");
                    }

                    return null;
                }

                plans[i] = plan;
            }

            return plans;
        }

        /// <summary>
        /// 为单个构造器参数构建解析方案。
        /// </summary>
        /// <param name="parameter">构造器参数</param>
        /// <param name="serviceKey">当前解析使用的键，可以为 null</param>
        /// <param name="serviceProvider">服务提供程序</param>
        /// <returns>参数解析方案；如果参数不可解析，则返回 null</returns>
        private ParameterPlan CreateParameterPlan(
            ParameterInfo parameter,
            object serviceKey,
            IServiceProvider serviceProvider)
        {
            foreach (var attribute in parameter.GetCustomAttributes(true))
            {
                // 与原生一致：仅当以非空键解析时，ServiceKeyAttribute 才注入当前键
                if (serviceKey != null && attribute is ServiceKeyAttribute)
                {
                    if (parameter.ParameterType != serviceKey.GetType())
                    {
                        throw new InvalidOperationException(
                            "The type of the key used for lookup doesn't match the type in the constructor parameter with the ServiceKey attribute.");
                    }

                    return ParameterPlan.ServiceKey(parameter);
                }

                if (attribute is FromKeyedServicesAttribute keyedAttribute)
                {
                    if (IsKeyedServiceResolvable(serviceProvider, parameter.ParameterType, keyedAttribute.Key))
                    {
                        return ParameterPlan.Keyed(parameter, keyedAttribute.Key);
                    }

                    // 与原生一致：键控服务未注册时回退为普通服务解析
                    break;
                }
            }

            if (IsServiceResolvable(parameter.ParameterType))
            {
                return ParameterPlan.Ordinary(parameter);
            }

            if (TryGetDefaultValue(parameter, out var defaultValue))
            {
                return ParameterPlan.Default(parameter, defaultValue);
            }

            return null;
        }

        /// <summary>
        /// 判断普通服务类型是否已注册。
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns>如果可解析，则返回 true；否则返回 false</returns>
        private bool IsServiceResolvable(Type serviceType)
            => _ordinaryServices.IsService(serviceType);

        /// <summary>
        /// 判断键控服务是否可由指定服务提供程序解析。
        /// </summary>
        /// <param name="serviceProvider">服务提供程序</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="serviceKey">服务键，可以为 null</param>
        /// <returns>如果可解析，则返回 true；否则返回 false</returns>
        private static bool IsKeyedServiceResolvable(IServiceProvider serviceProvider, Type serviceType, object serviceKey)
            => serviceProvider is IServiceProviderIsKeyedService keyedProvider && keyedProvider.IsKeyedService(serviceType, serviceKey);

        /// <summary>
        /// 执行构造器解析方案并创建实例。
        /// </summary>
        /// <param name="plan">构造器解析方案</param>
        /// <param name="serviceKey">当前解析使用的键，可以为 null</param>
        /// <param name="serviceProvider">服务提供程序</param>
        /// <returns>创建完成的实例</returns>
        private object InvokePlan(
            ConstructorPlan plan,
            object serviceKey,
            IServiceProvider serviceProvider)
        {
            var arguments = new object[plan.Parameters.Length];
            for (var i = 0; i < plan.Parameters.Length; i++)
            {
                var parameterPlan = plan.Parameters[i];
                switch (parameterPlan.Kind)
                {
                    case ParameterPlanKind.Ordinary:
                        arguments[i] = serviceProvider.GetService(parameterPlan.Parameter.ParameterType);
                        break;
                    case ParameterPlanKind.KeyedService:
                        arguments[i] = serviceProvider.GetKeyedService(parameterPlan.Parameter.ParameterType, parameterPlan.KeyedServiceKey);
                        break;
                    case ParameterPlanKind.ServiceKey:
                        arguments[i] = serviceKey;
                        break;
                    default:
                        arguments[i] = parameterPlan.DefaultValue;
                        break;
                }
            }

            return plan.Constructor.Invoke(arguments);
        }

        /// <summary>
        /// 获取参数声明的默认值。
        /// 复刻 Microsoft DI 对值类型和可空类型的默认值处理规则。
        /// </summary>
        /// <param name="parameter">构造器参数</param>
        /// <param name="defaultValue">输出参数，参数声明的默认值</param>
        /// <returns>如果参数具有默认值，则返回 true；否则返回 false</returns>
        private static bool TryGetDefaultValue(ParameterInfo parameter, out object defaultValue)
        {
            var hasDefaultValue = CheckHasDefaultValue(parameter, out var tryToGetDefaultValue);
            defaultValue = null;

            if (hasDefaultValue)
            {
                if (tryToGetDefaultValue)
                {
                    defaultValue = parameter.DefaultValue;
                }

                var isNullableParameterType = parameter.ParameterType.IsGenericType &&
                    parameter.ParameterType.GetGenericTypeDefinition() == typeof(Nullable<>);

                if (defaultValue == null && parameter.ParameterType.IsValueType && !isNullableParameterType)
                {
                    defaultValue = FormatterServices.GetUninitializedObject(parameter.ParameterType);
                }

                if (defaultValue != null && isNullableParameterType)
                {
                    var underlyingType = Nullable.GetUnderlyingType(parameter.ParameterType);
                    if (underlyingType != null && underlyingType.IsEnum)
                    {
                        defaultValue = Enum.ToObject(underlyingType, defaultValue);
                    }
                }
            }

            return hasDefaultValue;
        }

        /// <summary>
        /// 检查参数是否声明了默认值，并兼容 DateTime 参数的已知反射问题。
        /// </summary>
        /// <param name="parameter">构造器参数</param>
        /// <param name="tryToGetDefaultValue">输出参数，指示是否应读取默认值</param>
        /// <returns>如果参数具有默认值，则返回 true；否则返回 false</returns>
        private static bool CheckHasDefaultValue(ParameterInfo parameter, out bool tryToGetDefaultValue)
        {
            tryToGetDefaultValue = true;
            try
            {
                return parameter.HasDefaultValue;
            }
            catch (FormatException) when (parameter.ParameterType == typeof(DateTime))
            {
                tryToGetDefaultValue = false;
                return true;
            }
        }

        /// <summary>
        /// 构造器参数解析方案中的参数类型枚举。
        /// </summary>
        private enum ParameterPlanKind
        {
            /// <summary>
            /// 按普通服务解析。
            /// </summary>
            Ordinary,

            /// <summary>
            /// 按键控服务解析。
            /// </summary>
            KeyedService,

            /// <summary>
            /// 注入当前解析使用的键。
            /// </summary>
            ServiceKey,

            /// <summary>
            /// 使用参数声明的默认值。
            /// </summary>
            Default
        }

        /// <summary>
        /// 单个构造器参数的解析方案。
        /// </summary>
        private sealed class ParameterPlan
        {
            /// <summary>
            /// 初始化 <see cref="ParameterPlan"/> 类的新实例。
            /// </summary>
            /// <param name="parameter">构造器参数</param>
            /// <param name="kind">解析方式</param>
            /// <param name="keyedServiceKey">键控服务使用的键</param>
            /// <param name="defaultValue">参数声明的默认值</param>
            private ParameterPlan(ParameterInfo parameter, ParameterPlanKind kind, object keyedServiceKey, object defaultValue)
            {
                Parameter = parameter;
                Kind = kind;
                KeyedServiceKey = keyedServiceKey;
                DefaultValue = defaultValue;
            }

            /// <summary>
            /// 获取构造器参数。
            /// </summary>
            /// <value>构造器参数</value>
            public ParameterInfo Parameter { get; }

            /// <summary>
            /// 获取解析方式。
            /// </summary>
            /// <value>解析方式</value>
            public ParameterPlanKind Kind { get; }

            /// <summary>
            /// 获取键控服务使用的键。
            /// </summary>
            /// <value>服务键，仅键控解析方式使用</value>
            public object KeyedServiceKey { get; }

            /// <summary>
            /// 获取参数声明的默认值。
            /// </summary>
            /// <value>默认值，仅默认值解析方式使用</value>
            public object DefaultValue { get; }

            /// <summary>
            /// 创建普通服务解析方案。
            /// </summary>
            /// <param name="parameter">构造器参数</param>
            /// <returns>普通服务解析方案</returns>
            public static ParameterPlan Ordinary(ParameterInfo parameter)
                => new ParameterPlan(parameter, ParameterPlanKind.Ordinary, null, null);

            /// <summary>
            /// 创建键控服务解析方案。
            /// </summary>
            /// <param name="parameter">构造器参数</param>
            /// <param name="key">服务键</param>
            /// <returns>键控服务解析方案</returns>
            public static ParameterPlan Keyed(ParameterInfo parameter, object key)
                => new ParameterPlan(parameter, ParameterPlanKind.KeyedService, key, null);

            /// <summary>
            /// 创建当前键注入方案。
            /// </summary>
            /// <param name="parameter">构造器参数</param>
            /// <returns>当前键注入方案</returns>
            public static ParameterPlan ServiceKey(ParameterInfo parameter)
                => new ParameterPlan(parameter, ParameterPlanKind.ServiceKey, null, null);

            /// <summary>
            /// 创建默认值解析方案。
            /// </summary>
            /// <param name="parameter">构造器参数</param>
            /// <param name="defaultValue">参数声明的默认值</param>
            /// <returns>默认值解析方案</returns>
            public static ParameterPlan Default(ParameterInfo parameter, object defaultValue)
                => new ParameterPlan(parameter, ParameterPlanKind.Default, null, defaultValue);
        }

        /// <summary>
        /// 构造器及其参数解析方案。
        /// </summary>
        private sealed class ConstructorPlan
        {
            /// <summary>
            /// 初始化 <see cref="ConstructorPlan"/> 类的新实例。
            /// </summary>
            /// <param name="constructor">选中的构造器</param>
            /// <param name="parameters">参数解析方案</param>
            public ConstructorPlan(ConstructorInfo constructor, ParameterPlan[] parameters)
            {
                Constructor = constructor;
                Parameters = parameters;
            }

            /// <summary>
            /// 获取选中的构造器。
            /// </summary>
            /// <value>构造器</value>
            public ConstructorInfo Constructor { get; }

            /// <summary>
            /// 获取构造器参数解析方案。
            /// </summary>
            /// <value>参数解析方案数组</value>
            public ParameterPlan[] Parameters { get; }
        }
    }
}
#endif
