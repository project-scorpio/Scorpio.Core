using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection.Extensions;

using Shouldly;

using Xunit;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 键控服务兼容层测试。
    /// 同一套用例在低版本目标验证兼容层实现，在 .NET 8 及以上目标验证原生实现不退化。
    /// </summary>
    public class KeyedService_Tests
    {
        [Fact]
        public void Should_Resolve_Different_Implementations_By_Key()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<ICache>("premium", new PremiumCache());
            services.AddKeyedSingleton<ICache>("sql", new SqlCache());

            using (var provider = BuildProvider(services) as IDisposable)
            {
                provider.ShouldNotBeNull();
                var serviceProvider = (IServiceProvider)provider;
                serviceProvider.GetRequiredKeyedService<ICache>("premium").ShouldBeOfType<PremiumCache>();
                serviceProvider.GetRequiredKeyedService<ICache>("sql").ShouldBeOfType<SqlCache>();
            }
        }

        [Fact]
        public void Should_Isolate_Keyed_And_Unkeyed_Registrations()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ICache>(new MemoryCache());
            services.AddKeyedSingleton<ICache>("premium", new PremiumCache());

            using (var provider = BuildProvider(services) as IDisposable)
            {
                var serviceProvider = (IServiceProvider)provider;
                serviceProvider.GetRequiredService<ICache>().ShouldBeOfType<MemoryCache>();
                serviceProvider.GetRequiredKeyedService<ICache>("premium").ShouldBeOfType<PremiumCache>();
            }
        }

        [Fact]
        public void Should_Return_Null_When_Keyed_Service_Not_Found()
        {
            var services = new ServiceCollection();
            using (var provider = BuildProvider(services) as IDisposable)
            {
                ((IServiceProvider)provider).GetKeyedService<ICache>("missing").ShouldBeNull();
            }
        }

        [Fact]
        public void Should_Throw_When_Required_Keyed_Service_Not_Found()
        {
            var services = new ServiceCollection();
            using (var provider = BuildProvider(services) as IDisposable)
            {
                Should.Throw<InvalidOperationException>(() =>
                    ((IServiceProvider)provider).GetRequiredKeyedService<ICache>("missing"));
            }
        }

        [Fact]
        public void Should_Last_Registration_Win()
        {
            var services = new ServiceCollection();
            services.AddKeyedTransient(typeof(ICache), "sql", typeof(SqlCache));
            services.AddKeyedTransient(typeof(ICache), "sql", typeof(MemoryCache));

            using (var provider = BuildProvider(services) as IDisposable)
            {
                ((IServiceProvider)provider)
                    .GetRequiredKeyedService<ICache>("sql")
                    .ShouldBeOfType<MemoryCache>();
            }
        }

        [Fact]
        public void Should_GetKeyedServices_Return_In_Registration_Order()
        {
            var services = new ServiceCollection();
            services.AddKeyedTransient(typeof(ICache), "sql", typeof(SqlCache));
            services.AddKeyedTransient(typeof(ICache), "sql", typeof(MemoryCache));

            using (var provider = BuildProvider(services) as IDisposable)
            {
                var caches = ((IServiceProvider)provider).GetKeyedServices<ICache>("sql").ToList();
                caches.Count.ShouldBe(2);
                caches[0].ShouldBeOfType<SqlCache>();
                caches[1].ShouldBeOfType<MemoryCache>();
            }
        }

        [Fact]
        public void Should_Match_Key_By_Object_Equality()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<ICache>(new CustomKey("premium"), new PremiumCache());

            using (var provider = BuildProvider(services) as IDisposable)
            {
                ((IServiceProvider)provider)
                    .GetRequiredKeyedService<ICache>(new CustomKey("premium"))
                    .ShouldBeOfType<PremiumCache>();
            }
        }

        [Fact]
        public void Should_Support_Null_Key()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<ICache>(null, new PremiumCache());

            using (var provider = BuildProvider(services) as IDisposable)
            {
                ((IServiceProvider)provider)
                    .GetRequiredKeyedService<ICache>(null)
                    .ShouldBeOfType<PremiumCache>();
            }
        }

#if !NET8_0_OR_GREATER
        [Fact]
        public void Should_Isolate_Null_Keyed_From_Unkeyed_Registration()
        {
            // 需求要求 null 键与无键的普通服务语义分离；该行为由兼容层保证
            var services = new ServiceCollection();
            services.AddSingleton<ICache>(new MemoryCache());
            services.AddKeyedSingleton<ICache>(null, new PremiumCache());

            using (var provider = BuildProvider(services) as IDisposable)
            {
                var serviceProvider = (IServiceProvider)provider;
                serviceProvider.GetRequiredService<ICache>().ShouldBeOfType<MemoryCache>();
                serviceProvider.GetRequiredKeyedService<ICache>(null).ShouldBeOfType<PremiumCache>();
            }
        }
#endif

        [Fact]
        public void Should_Exact_Key_Take_Precedence_Over_AnyKey()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<ICache>(KeyedService.AnyKey, new MemoryCache());
            services.AddKeyedSingleton<ICache>("premium", new PremiumCache());

            using (var provider = BuildProvider(services) as IDisposable)
            {
                ((IServiceProvider)provider)
                    .GetRequiredKeyedService<ICache>("premium")
                    .ShouldBeOfType<PremiumCache>();
            }
        }

        [Fact]
        public void Should_Fall_Back_To_AnyKey_When_Exact_Key_Missing()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<ICache>(KeyedService.AnyKey, new MemoryCache());

            using (var provider = BuildProvider(services) as IDisposable)
            {
                var serviceProvider = (IServiceProvider)provider;
                serviceProvider.GetRequiredKeyedService<ICache>("missing").ShouldBeOfType<MemoryCache>();
                serviceProvider.GetRequiredService<IServiceProviderIsKeyedService>()
                    .IsKeyedService(typeof(ICache), "missing")
                    .ShouldBeTrue();
            }
        }

        // 仅兼容层验证 .NET 8 语义；net9/net10 原生对枚举解析中的 AnyKey 行为有演进
#if !NET8_0_OR_GREATER
        [Fact]
        public void Should_GetKeyedServices_AnyKey_Return_Only_AnyKey_Registrations()
        {
            var services = new ServiceCollection();
            services.AddKeyedTransient(typeof(ICache), "sql", typeof(SqlCache));
            services.AddKeyedTransient(typeof(ICache), KeyedService.AnyKey, typeof(MemoryCache));

            using (var provider = BuildProvider(services) as IDisposable)
            {
                var caches = ((IServiceProvider)provider).GetKeyedServices<ICache>(KeyedService.AnyKey).ToList();
                caches.Count.ShouldBe(1);
                caches[0].ShouldBeOfType<MemoryCache>();
            }
        }
#endif

        [Fact]
        public void Should_GetKeyedServices_Exact_Key_Exclude_AnyKey_Registrations()
        {
            var services = new ServiceCollection();
            services.AddKeyedTransient(typeof(ICache), KeyedService.AnyKey, typeof(MemoryCache));
            services.AddKeyedTransient(typeof(ICache), "sql", typeof(SqlCache));

            using (var provider = BuildProvider(services) as IDisposable)
            {
                var caches = ((IServiceProvider)provider).GetKeyedServices<ICache>("sql").ToList();
                caches.Count.ShouldBe(1);
                caches[0].ShouldBeOfType<SqlCache>();
            }
        }

        // 仅兼容层验证 .NET 8 语义；net10 原生不再将 AnyKey 注册纳入枚举解析
#if !NET8_0_OR_GREATER
        [Fact]
        public void Should_GetKeyedServices_Fall_Back_To_AnyKey_In_Registration_Order()
        {
            var services = new ServiceCollection();
            services.AddKeyedTransient(typeof(ICache), KeyedService.AnyKey, typeof(MemoryCache));
            services.AddKeyedTransient(typeof(ICache), KeyedService.AnyKey, typeof(PremiumCache));

            using (var provider = BuildProvider(services) as IDisposable)
            {
                var caches = ((IServiceProvider)provider).GetKeyedServices<ICache>("missing").ToList();
                caches.Count.ShouldBe(2);
                caches[0].ShouldBeOfType<MemoryCache>();
                caches[1].ShouldBeOfType<PremiumCache>();
            }
        }
#endif

        [Fact]
        public void Should_Cache_Keyed_Singleton_In_Root_Container()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton(typeof(ICache), "sql", typeof(SqlCache));

            using (var provider = BuildProvider(services) as IDisposable)
            {
                var serviceProvider = (IServiceProvider)provider;
                serviceProvider.GetRequiredKeyedService<ICache>("sql")
                    .ShouldBeSameAs(serviceProvider.GetRequiredKeyedService<ICache>("sql"));
            }
        }

        [Fact]
        public void Should_Cache_Keyed_Scoped_Instance_Per_Scope()
        {
            var services = new ServiceCollection();
            services.AddKeyedScoped(typeof(ICache), "sql", typeof(SqlCache));

            using (var provider = BuildProvider(services) as IDisposable)
            {
                var serviceProvider = (IServiceProvider)provider;
                ICache firstScopeCache;
                using (var scope = serviceProvider.CreateScope())
                {
                    firstScopeCache = scope.ServiceProvider.GetRequiredKeyedService<ICache>("sql");
                    scope.ServiceProvider.GetRequiredKeyedService<ICache>("sql").ShouldBeSameAs(firstScopeCache);
                }

                using (var scope = serviceProvider.CreateScope())
                {
                    scope.ServiceProvider.GetRequiredKeyedService<ICache>("sql").ShouldNotBeSameAs(firstScopeCache);
                }
            }
        }

        [Fact]
        public void Should_Create_New_Instance_For_Keyed_Transient()
        {
            var services = new ServiceCollection();
            services.AddKeyedTransient(typeof(ICache), "sql", typeof(SqlCache));

            using (var provider = BuildProvider(services) as IDisposable)
            {
                var serviceProvider = (IServiceProvider)provider;
                serviceProvider.GetRequiredKeyedService<ICache>("sql")
                    .ShouldNotBeSameAs(serviceProvider.GetRequiredKeyedService<ICache>("sql"));
            }
        }

        [Fact]
        public void Should_Dispose_Keyed_Singleton_With_Root_Container()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton(typeof(ICache), "disposable", typeof(DisposableCache));

            var provider = (IServiceProvider)BuildProvider(services);
            var instance = provider.GetRequiredKeyedService<ICache>("disposable").ShouldBeOfType<DisposableCache>();
            ((IDisposable)provider).Dispose();
            instance.Disposed.ShouldBeTrue();
        }

        [Fact]
        public void Should_Dispose_Keyed_Scoped_And_Transient_With_Scope()
        {
            var services = new ServiceCollection();
            services.AddKeyedScoped(typeof(ICache), "scoped", typeof(DisposableCache));
            services.AddKeyedTransient(typeof(ICache), "transient", typeof(DisposableCache));

            var provider = (IServiceProvider)BuildProvider(services);
            try
            {
                DisposableCache scoped;
                DisposableCache firstTransient;
                DisposableCache secondTransient;
                using (var scope = provider.CreateScope())
                {
                    scoped = scope.ServiceProvider.GetRequiredKeyedService<ICache>("scoped").ShouldBeOfType<DisposableCache>();
                    firstTransient = scope.ServiceProvider.GetRequiredKeyedService<ICache>("transient").ShouldBeOfType<DisposableCache>();
                    secondTransient = scope.ServiceProvider.GetRequiredKeyedService<ICache>("transient").ShouldBeOfType<DisposableCache>();
                    firstTransient.ShouldNotBeSameAs(secondTransient);
                }

                scoped.Disposed.ShouldBeTrue();
                firstTransient.Disposed.ShouldBeTrue();
                secondTransient.Disposed.ShouldBeTrue();
            }
            finally
            {
                ((IDisposable)provider).Dispose();
            }
        }

        [Fact]
        public void Should_Inject_FromKeyedServices_Into_Keyed_Service()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<ICache>("premium", new PremiumCache());
            services.AddKeyedSingleton<IConsumer, KeyedConsumer>("consumer");

            using (var provider = BuildProvider(services) as IDisposable)
            {
                ((IServiceProvider)provider)
                    .GetRequiredKeyedService<IConsumer>("consumer")
                    .Cache.ShouldBeOfType<PremiumCache>();
            }
        }

        [Fact]
        public void Should_Inject_FromKeyedServices_Into_Ordinary_Service()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<ICache>("premium", new PremiumCache());
            services.AddSingleton<IConsumer, KeyedConsumer>();

            using (var provider = BuildProvider(services) as IDisposable)
            {
                ((IServiceProvider)provider)
                    .GetRequiredService<IConsumer>()
                    .Cache.ShouldBeOfType<PremiumCache>();
            }
        }

        [Fact]
        public void Should_Inject_ServiceKey_When_Type_Matches()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<StringKeyConsumer>("consumer");

            using (var provider = BuildProvider(services) as IDisposable)
            {
                ((IServiceProvider)provider)
                    .GetRequiredKeyedService<StringKeyConsumer>("consumer")
                    .Key.ShouldBe("consumer");
            }
        }

        [Fact]
        public void Should_Throw_When_ServiceKey_Type_Does_Not_Match()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<IntKeyConsumer>("consumer");

            using (var provider = BuildProvider(services) as IDisposable)
            {
                Should.Throw<InvalidOperationException>(() =>
                    ((IServiceProvider)provider).GetRequiredKeyedService<IntKeyConsumer>("consumer"));
            }
        }

        [Fact]
        public void Should_Resolve_Closed_Instance_From_Open_Generic_Registration()
        {
            var services = new ServiceCollection();
            services.AddKeyedTransient(typeof(IRepository<>), "sql", typeof(SqlRepository<>));

            using (var provider = BuildProvider(services) as IDisposable)
            {
                var repository = ((IServiceProvider)provider)
                    .GetRequiredKeyedService<IRepository<string>>("sql");
                repository.ShouldBeOfType<SqlRepository<string>>();
                repository.ItemType.ShouldBe(typeof(string));
            }
        }

        [Fact]
        public void Should_TryAdd_Keep_First_Registration()
        {
            var services = new ServiceCollection();
            var key = "cache";
            services.TryAddKeyedSingleton<ICache>(key, new PremiumCache());
            services.TryAddKeyedSingleton<ICache>(key, new MemoryCache());

            using (var provider = BuildProvider(services) as IDisposable)
            {
                ((IServiceProvider)provider)
                    .GetRequiredKeyedService<ICache>(key)
                    .ShouldBeOfType<PremiumCache>();
            }
        }

        [Fact]
        public void Should_RemoveAllKeyed_Remove_All_Registrations()
        {
            var services = new ServiceCollection();
            services.AddKeyedTransient(typeof(ICache), "sql", typeof(SqlCache));
            services.AddKeyedTransient(typeof(ICache), "sql", typeof(MemoryCache));
            services.RemoveAllKeyed<ICache>("sql");

            using (var provider = BuildProvider(services) as IDisposable)
            {
                ((IServiceProvider)provider).GetKeyedService<ICache>("sql").ShouldBeNull();
            }
        }

        [Fact]
        public void Should_IsKeyedService_Report_Registration_State()
        {
            var services = new ServiceCollection();
            services.AddKeyedSingleton<ICache>("sql", new SqlCache());

            using (var provider = BuildProvider(services) as IDisposable)
            {
                var keyedProvider = ((IServiceProvider)provider).GetRequiredService<IServiceProviderIsKeyedService>();
                keyedProvider.IsKeyedService(typeof(ICache), "sql").ShouldBeTrue();
                keyedProvider.IsKeyedService(typeof(ICache), "missing").ShouldBeFalse();
            }
        }

        /// <summary>
        /// 构建支持键控解析的服务提供程序。
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务提供程序</returns>
        private static IServiceProvider BuildProvider(IServiceCollection services)
        {
#if NET8_0_OR_GREATER
            return services.BuildServiceProvider();
#else
            return services.BuildKeyedServiceProvider();
#endif
        }
    }

    /// <summary>
    /// 缓存服务测试接口。
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// 获取缓存实现名称。
        /// </summary>
        /// <value>缓存实现名称</value>
        string Name { get; }
    }

    /// <summary>
    /// 高级缓存实现。
    /// </summary>
    public class PremiumCache : ICache
    {
        /// <summary>
        /// 获取缓存实现名称。
        /// </summary>
        /// <value>缓存实现名称</value>
        public string Name => "premium";
    }

    /// <summary>
    /// SQL 缓存实现。
    /// </summary>
    public class SqlCache : ICache
    {
        /// <summary>
        /// 获取缓存实现名称。
        /// </summary>
        /// <value>缓存实现名称</value>
        public string Name => "sql";
    }

    /// <summary>
    /// 内存缓存实现。
    /// </summary>
    public class MemoryCache : ICache
    {
        /// <summary>
        /// 获取缓存实现名称。
        /// </summary>
        /// <value>缓存实现名称</value>
        public string Name => "memory";
    }

    /// <summary>
    /// 可释放缓存实现。
    /// </summary>
    public class DisposableCache : ICache, IDisposable
    {
        /// <summary>
        /// 获取缓存实现名称。
        /// </summary>
        /// <value>缓存实现名称</value>
        public string Name => "disposable";

        /// <summary>
        /// 获取一个值，指示实例是否已释放。
        /// </summary>
        /// <value>如果已释放，则为 true；否则为 false</value>
        public bool Disposed { get; private set; }

        /// <summary>
        /// 释放实例。
        /// </summary>
        public void Dispose() => Disposed = true;
    }

    /// <summary>
    /// 消费缓存服务的测试接口。
    /// </summary>
    public interface IConsumer
    {
        /// <summary>
        /// 获取注入的缓存服务。
        /// </summary>
        /// <value>缓存服务</value>
        ICache Cache { get; }
    }

    /// <summary>
    /// 使用 <see cref="FromKeyedServicesAttribute"/> 构造注入的消费类。
    /// </summary>
    public class KeyedConsumer : IConsumer
    {
        /// <summary>
        /// 初始化 <see cref="KeyedConsumer"/> 类的新实例。
        /// </summary>
        /// <param name="cache">按键解析的缓存服务</param>
        public KeyedConsumer([FromKeyedServices("premium")] ICache cache) => Cache = cache;

        /// <summary>
        /// 获取注入的缓存服务。
        /// </summary>
        /// <value>缓存服务</value>
        public ICache Cache { get; }
    }

    /// <summary>
    /// 开放泛型仓储测试接口。
    /// </summary>
    /// <typeparam name="T">仓储元素类型</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// 获取仓储元素类型。
        /// </summary>
        /// <value>元素类型</value>
        Type ItemType { get; }
    }

    /// <summary>
    /// SQL 开放泛型仓储实现。
    /// </summary>
    /// <typeparam name="T">仓储元素类型</typeparam>
    public class SqlRepository<T> : IRepository<T>
    {
        /// <summary>
        /// 获取仓储元素类型。
        /// </summary>
        /// <value>元素类型</value>
        public Type ItemType => typeof(T);
    }

    /// <summary>
    /// 使用值相等性的自定义服务键。
    /// </summary>
    public class CustomKey
    {
        /// <summary>
        /// 存储键的内部值。
        /// </summary>
        private readonly string _value;

        /// <summary>
        /// 初始化 <see cref="CustomKey"/> 类的新实例。
        /// </summary>
        /// <param name="value">键值</param>
        public CustomKey(string value) => _value = value;

        /// <summary>
        /// 判断当前键是否与指定对象相等。
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        /// <returns>如果相等，则返回 true；否则返回 false</returns>
        public override bool Equals(object obj)
            => obj is CustomKey other && _value == other._value;

        /// <summary>
        /// 返回当前键的哈希代码。
        /// </summary>
        /// <returns>哈希代码</returns>
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
    }

    /// <summary>
    /// 注入字符串键的测试类。
    /// </summary>
    public class StringKeyConsumer
    {
        /// <summary>
        /// 初始化 <see cref="StringKeyConsumer"/> 类的新实例。
        /// </summary>
        /// <param name="key">当前解析使用的键</param>
        public StringKeyConsumer([ServiceKey] string key) => Key = key;

        /// <summary>
        /// 获取注入的键。
        /// </summary>
        /// <value>当前解析使用的键</value>
        public string Key { get; }
    }

    /// <summary>
    /// 注入整数键的测试类，用于验证键类型不匹配场景。
    /// </summary>
    public class IntKeyConsumer
    {
        /// <summary>
        /// 初始化 <see cref="IntKeyConsumer"/> 类的新实例。
        /// </summary>
        /// <param name="key">当前解析使用的键</param>
        public IntKeyConsumer([ServiceKey] int key) => Key = key;

        /// <summary>
        /// 获取注入的键。
        /// </summary>
        /// <value>当前解析使用的键</value>
        public int Key { get; }
    }
}
