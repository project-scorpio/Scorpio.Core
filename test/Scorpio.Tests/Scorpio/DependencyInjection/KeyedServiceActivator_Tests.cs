#if !NET8_0_OR_GREATER
using System;

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

using Xunit;

namespace Scorpio.DependencyInjection
{
    /// <summary>
    /// 键控感知实例激活器测试，覆盖构造器选择、默认值、歧义和循环依赖场景。
    /// </summary>
    public class KeyedServiceActivator_Tests
    {
        [Fact]
        public void Should_Select_Longest_Resolvable_Constructor()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IDependency, Dependency>();
            services.AddKeyedSingleton<ISelectable, MultiConstructorService>("service");

            using (var provider = services.BuildKeyedServiceProvider() as IDisposable)
            {
                ((IServiceProvider)provider)
                    .GetRequiredKeyedService<ISelectable>("service")
                    .ConstructorIndex.ShouldBe(1);
            }
        }

        [Fact]
        public void Should_Throw_When_Constructors_Are_Ambiguous()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IDependency, Dependency>();
            services.AddSingleton<Dependency>();
            services.AddKeyedSingleton<ISelectable, AmbiguousService>("service");

            using (var provider = services.BuildKeyedServiceProvider() as IDisposable)
            {
                Should.Throw<InvalidOperationException>(() =>
                    ((IServiceProvider)provider).GetRequiredKeyedService<ISelectable>("service"));
            }
        }

        [Fact]
        public void Should_Use_Default_Values_When_Parameter_Not_Registered()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<DefaultValueService>("service");

            using (var provider = services.BuildKeyedServiceProvider() as IDisposable)
            {
                var instance = ((IServiceProvider)provider).GetRequiredKeyedService<DefaultValueService>("service");
                instance.Dependency.ShouldBeNull();
                instance.Count.ShouldBe(42);
            }
        }

        [Fact]
        public void Should_Fall_Back_To_Ordinary_Service_When_Keyed_Service_Missing()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ICache>(new MemoryCache());
            services.AddKeyedSingleton<FallbackConsumer>("service");

            using (var provider = services.BuildKeyedServiceProvider() as IDisposable)
            {
                ((IServiceProvider)provider)
                    .GetRequiredKeyedService<FallbackConsumer>("service")
                    .Cache.ShouldBeOfType<MemoryCache>();
            }
        }

        [Fact]
        public void Should_Detect_Circular_Dependency_Between_Keyed_Services()
        {
            var services = new ServiceCollection();
            services.AddKeyedTransient<ICircularA, CircularA>("a");
            services.AddKeyedTransient<ICircularB, CircularB>("b");

            using (var provider = services.BuildKeyedServiceProvider() as IDisposable)
            {
                Should.Throw<InvalidOperationException>(() =>
                    ((IServiceProvider)provider).GetRequiredKeyedService<ICircularA>("a"));
            }
        }

        /// <summary>
        /// 测试依赖接口。
        /// </summary>
        public interface IDependency
        {
        }

        /// <summary>
        /// 测试依赖实现。
        /// </summary>
        public class Dependency : IDependency
        {
        }

        /// <summary>
        /// 构造器选择测试接口。
        /// </summary>
        public interface ISelectable
        {
            /// <summary>
            /// 获取被选中的构造器索引。
            /// </summary>
            /// <value>构造器索引</value>
            int ConstructorIndex { get; }
        }

        /// <summary>
        /// 具有多个构造器的测试类。
        /// </summary>
        public class MultiConstructorService : ISelectable
        {
            /// <summary>
            /// 初始化 <see cref="MultiConstructorService"/> 类的新实例。
            /// </summary>
            public MultiConstructorService() => ConstructorIndex = 0;

            /// <summary>
            /// 初始化 <see cref="MultiConstructorService"/> 类的新实例。
            /// </summary>
            /// <param name="dependency">测试依赖</param>
            public MultiConstructorService(IDependency dependency) => ConstructorIndex = 1;

            /// <summary>
            /// 获取被选中的构造器索引。
            /// </summary>
            /// <value>构造器索引</value>
            public int ConstructorIndex { get; }
        }

        /// <summary>
        /// 构造器存在歧义的测试类。
        /// </summary>
        public class AmbiguousService : ISelectable
        {
            /// <summary>
            /// 初始化 <see cref="AmbiguousService"/> 类的新实例。
            /// </summary>
            /// <param name="dependency">接口依赖</param>
            public AmbiguousService(IDependency dependency) => ConstructorIndex = 1;

            /// <summary>
            /// 初始化 <see cref="AmbiguousService"/> 类的新实例。
            /// </summary>
            /// <param name="dependency">具体依赖</param>
            public AmbiguousService(Dependency dependency) => ConstructorIndex = 2;

            /// <summary>
            /// 获取被选中的构造器索引。
            /// </summary>
            /// <value>构造器索引</value>
            public int ConstructorIndex { get; }
        }

        /// <summary>
        /// 使用默认参数值的测试类。
        /// </summary>
        public class DefaultValueService
        {
            /// <summary>
            /// 初始化 <see cref="DefaultValueService"/> 类的新实例。
            /// </summary>
            /// <param name="dependency">未注册的依赖</param>
            /// <param name="count">默认值参数</param>
            public DefaultValueService(IUnregistered dependency = null, int count = 42)
            {
                Dependency = dependency;
                Count = count;
            }

            /// <summary>
            /// 获取未注册的依赖。
            /// </summary>
            /// <value>未注册的依赖</value>
            public IUnregistered Dependency { get; }

            /// <summary>
            /// 获取默认值参数。
            /// </summary>
            /// <value>默认值参数</value>
            public int Count { get; }
        }

        /// <summary>
        /// 未注册的测试接口。
        /// </summary>
        public interface IUnregistered
        {
        }

        /// <summary>
        /// 键控回退测试类。
        /// </summary>
        public class FallbackConsumer
        {
            /// <summary>
            /// 初始化 <see cref="FallbackConsumer"/> 类的新实例。
            /// </summary>
            /// <param name="cache">使用缺失键解析的缓存服务</param>
            public FallbackConsumer([FromKeyedServices("missing")] ICache cache) => Cache = cache;

            /// <summary>
            /// 获取注入的缓存服务。
            /// </summary>
            /// <value>缓存服务</value>
            public ICache Cache { get; }
        }

        /// <summary>
        /// 循环依赖测试接口 A。
        /// </summary>
        public interface ICircularA
        {
        }

        /// <summary>
        /// 循环依赖测试接口 B。
        /// </summary>
        public interface ICircularB
        {
        }

        /// <summary>
        /// 循环依赖实现 A。
        /// </summary>
        public class CircularA : ICircularA
        {
            /// <summary>
            /// 初始化 <see cref="CircularA"/> 类的新实例。
            /// </summary>
            /// <param name="b">循环依赖 B</param>
            public CircularA([FromKeyedServices("b")] ICircularB b)
            {
            }
        }

        /// <summary>
        /// 循环依赖实现 B。
        /// </summary>
        public class CircularB : ICircularB
        {
            /// <summary>
            /// 初始化 <see cref="CircularB"/> 类的新实例。
            /// </summary>
            /// <param name="a">循环依赖 A</param>
            public CircularB([FromKeyedServices("a")] ICircularA a)
            {
            }
        }
    }
}
#endif
