# 键控服务兼容方案需求文档

## 1. 背景与目标

.NET 8 在 `Microsoft.Extensions.DependencyInjection` 中正式引入键控服务（Keyed Services）。Scorpio.Core 当前源码项目同时面向 `netstandard2.0`、`net5.0`、`net6.0`、`net7.0`、`net8.0`、`net9.0`、`net10.0`，但 `netstandard2.0` ~ `net7.0` 目标仍使用各自时代对应的旧版 `Microsoft.Extensions.DependencyInjection`，默认不提供键控服务。

本需求的目标是：**在 .NET 8 以下目标框架中补齐键控服务能力，使注册、解析、构造注入、作用域生命周期、异常行为等与 .NET 8 原生行为一致。**

## 2. 适用范围

### 2.1 目标框架

| 目标框架 | 当前 `Microsoft.Extensions.DependencyInjection` 版本 | 是否已有键控服务 |
| --- | --- | --- |
| `netstandard2.0` | 3.1.31 | 否 |
| `net5.0` | 5.0.1 | 否 |
| `net6.0` | 6.0.1 | 否 |
| `net7.0` | 7.0.0 | 否 |
| `net8.0` | 8.0.0 | 是 |
| `net9.0` | 9.0.0 | 是 |
| `net10.0` | 10.0.0 | 是 |

兼容能力应覆盖 `netstandard2.0`、`net5.0`、`net6.0`、`net7.0` 四个目标。`net8.0` 及以上使用原生实现，不重复实现。

### 2.2 影响范围

- `src/Scorpio`：DI 扩展的主要载体。
- `test/Scorpio.Tests`：新增键控服务兼容性测试。
- 文档：新增本需求文档，并更新技术规范或开发指南中的兼容能力说明。

## 3. 参考基线：.NET 8 键控服务 API

### 3.1 注册 API

.NET 8 通过 `IServiceCollection` 扩展方法提供三类键控注册：

- `AddKeyedSingleton`
- `AddKeyedScoped`
- `AddKeyedTransient`

每个生命周期包含以下重载形式：

```text
AddKeyedSingleton<TService>(this IServiceCollection, object? serviceKey)
AddKeyedSingleton<TService>(this IServiceCollection, object? serviceKey, TService implementationInstance)
AddKeyedSingleton<TService>(this IServiceCollection, object? serviceKey, Func<IServiceProvider, object, TService> implementationFactory)
AddKeyedSingleton<TService, TImplementation>(this IServiceCollection, object? serviceKey)
AddKeyedSingleton<TService, TImplementation>(this IServiceCollection, object? serviceKey, Func<IServiceProvider, object, TImplementation> implementationFactory)

AddKeyedSingleton(this IServiceCollection, Type serviceType, object? serviceKey)
AddKeyedSingleton(this IServiceCollection, Type serviceType, object? serviceKey, Type implementationType)
AddKeyedSingleton(this IServiceCollection, Type serviceType, object? serviceKey, object implementationInstance)
AddKeyedSingleton(this IServiceCollection, Type serviceType, object? serviceKey, Func<IServiceProvider, object, object> implementationFactory)
```

`AddKeyedScoped` 与 `AddKeyedTransient` 除单例实例重载外，拥有相同形式的类型和工厂重载。

同时需要支持 `TryAddKeyed*` 系列：

```text
TryAddKeyedSingleton
TryAddKeyedScoped
TryAddKeyedTransient
```

以及：

```text
RemoveAllKeyed<TService>(this IServiceCollection, object? serviceKey)
RemoveAllKeyed(this IServiceCollection, Type serviceType, object? serviceKey)
```

### 3.2 解析 API

```text
GetKeyedService<T>(this IServiceProvider, object? serviceKey)
GetRequiredKeyedService<T>(this IServiceProvider, object? serviceKey)
GetRequiredKeyedService(this IServiceProvider, Type serviceType, object? serviceKey)
GetKeyedServices<T>(this IServiceProvider, object? serviceKey)
GetKeyedServices(this IServiceProvider, Type serviceType, object? serviceKey)
```

`IServiceProvider` 的键控实现通过 `IKeyedServiceProvider` 暴露：

```csharp
public interface IKeyedServiceProvider : IServiceProvider
{
    object GetKeyedService(Type serviceType, object? serviceKey);
    object GetRequiredKeyedService(Type serviceType, object? serviceKey);
}
```

### 3.3 特性与辅助类型

| 类型 | 作用 |
| --- | --- |
| `FromKeyedServicesAttribute` | 标注构造函数参数，指定使用哪个 key 解析该依赖 |
| `ServiceKeyAttribute` | 标注构造函数参数，注入当前服务注册或解析时使用的 key |
| `KeyedService.AnyKey` | 特殊 key，用于注册任意 key 的兜底实现 |
| `IServiceProviderIsKeyedService` | 用于查询某服务类型和 key 是否已注册 |

`FromKeyedServicesAttribute` 的构造为：

```csharp
public FromKeyedServicesAttribute(object key)
```

`ServiceKeyAttribute` 只能用于参数：

```csharp
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class ServiceKeyAttribute : Attribute
{
}
```

### 3.4 ServiceDescriptor 扩展

.NET 8 的 `ServiceDescriptor` 新增了键控描述能力和静态工厂：

- `ServiceKey`
- `KeyedImplementationType`
- `KeyedImplementationInstance`
- `KeyedImplementationFactory`
- `IsKeyedService`
- `DescribeKeyed(...)`
- `KeyedSingleton(...)`
- `KeyedScoped(...)`
- `KeyedTransient(...)`

这些成员属于外部 `Microsoft.Extensions.DependencyInjection.Abstractions` 程序集，Scorpio.Core 无法直接向既有 `ServiceDescriptor` 类型追加成员。是否要求完整 API 对齐需在实现方案中单独决策，见第 6 章。

## 4. 行为要求

### 4.1 键控空间与非键控空间隔离

- 非键控 `GetService<T>()` 不应返回任何键控注册。
- 键控 `GetKeyedService<T>(key)` 不应返回非键控注册。
- 同一个服务类型可以同时拥有非键控注册和多个不同 key 的键控注册。

### 4.2 key 语义

- key 可以是任意对象，不限于字符串。
- key 使用对象相等性进行匹配。
- `null` 是合法的 key，且与“无 key”的非键控服务语义分离。
- 相同 `serviceType + serviceKey` 重复注册时，最后一次注册生效。

### 4.3 KeyedService.AnyKey

- 使用 `KeyedService.AnyKey` 注册的键控服务作为兜底实现。
- 解析某 key 时，优先返回该 key 的精确注册；没有精确注册时，返回 `KeyedService.AnyKey` 兜底实现。
- 通过 `GetKeyedServices<T>(KeyedService.AnyKey)` 可查询 `AnyKey` 兜底注册。
- `AnyKey` 不应干扰精确 key 的优先匹配。

### 4.4 解析结果

- `GetKeyedService<T>(key)` 未找到时返回 `default(T)`（引用类型为 `null`）。
- `GetRequiredKeyedService<T>(key)` 未找到时抛出 `InvalidOperationException`，异常信息应接近或等同于：

```text
No service for type '<serviceType>' has been registered.
```

- `GetKeyedServices<T>(key)` 返回所有匹配该 `serviceType + key` 的注册，保持注册顺序。

### 4.5 生命周期与作用域

- `Singleton`：同一根容器内解析结果相同。
- `Scoped`：同一作用域内解析结果相同，不同作用域结果不同。
- `Transient`：每次解析都创建新实例。
- 容器销毁或作用域销毁时，按 Microsoft DI 既有规则释放由容器创建的实例。
- 在根容器解析 `Scoped` 键控服务时，应遵循标准作用域校验规则。

### 4.6 构造注入

- 构造函数参数标注 `[FromKeyedServices(key)]` 时，必须解析指定 key 的键控服务。
- 构造函数参数标注 `[ServiceKey]` 时，必须注入当前解析使用的 key。
- `ServiceKey` 参数类型必须与实际 key 类型兼容；不兼容时抛出 `InvalidOperationException`。
- 构造器选择规则应遵循 Microsoft DI 标准：选择“参数均可被解析”的最多参数构造函数；出现歧义时抛出异常。

### 4.7 开放泛型

键控服务应支持开放泛型服务类型和开放泛型实现类型，例如：

```csharp
services.AddKeyedTransient(typeof(IRepository<>), "sql", typeof(SqlRepository<>));
```

解析时按照标准开放泛型匹配规则返回构造后的泛型实例。

## 5. 与现有 Scorpio 机制的兼容

### 5.1 现有约定式注册

- 键控注册不得破坏现有 `ITransientDependency` / `IScopedDependency` / `ISingletonDependency` 自动注册。
- 键控服务不应被现有约定式扫描误注册为普通服务。
- 键控注册应可在 `ConfigureServicesContext` 中调用，与普通 `IServiceCollection` 注册并存。

### 5.2 Bootstrapper / Generic Host

- `Bootstrapper.Create<T>()` 创建的容器应支持键控解析。
- `AddScorpio<TStartupModule>()` 接入 Generic Host 后，容器仍应支持键控解析。
- 键控服务在 `Initialize()` 阶段应能被正常解析，包括通过构造注入使用 `[FromKeyedServices]`。

### 5.3 第三方容器

- 如果用户通过 `UseServiceProviderFactory<TContainerBuilder>()` 替换容器，键控能力由该第三方容器决定。
- 兼容方案应明确：默认 Microsoft 容器路径必须完整支持；第三方容器路径至少不应破坏普通 DI。

## 6. 实现方案

### 6.1 方案结论

Scorpio.Core 是框架项目，**不采用升级 `Microsoft.Extensions.DependencyInjection` 到 8.0 的方案**，但对外实现方案和使用方式应与 .NET 8 原生键控服务保持一致。

升级官方包虽然可以复用键控服务实现，但会改变宿主应用和第三方框架的依赖树，容易与使用旧版 DI 包的其他框架产生版本冲突。因此，兼容能力应由 Scorpio.Core 内部提供，并通过与原生相同的 API 暴露。

本需求确定采用 **Scorpio 自研键控服务兼容层**，并将以下目标作为硬性约束：

1. 不改变 `Microsoft.Extensions.DependencyInjection` 的包版本。
2. 对外 API、特性和命名空间与 .NET 8 原生键控服务一致。
3. 只在 `net8.0` 以下目标启用兼容层；`net8.0` 及以上继续使用原生键控服务。
4. 对默认 Microsoft DI 容器提供完整运行时能力，包括构造注入。
5. 使用派生自 `ServiceDescriptor` 的 `KeyedServiceDescriptor` 表达键控描述符。

### 6.2 设计原则

- **原生 API 对齐**：公开扩展方法、特性和辅助类型使用与 .NET 8 相同的名称和命名空间。
- **条件编译隔离**：兼容类型只在 `!NET8_0_OR_GREATER` 下编译，`net8.0` 及以上不重复定义。
- **Descriptor 继承扩展**：`ServiceDescriptor` 在旧版目标程序集中不是 sealed，可派生 `KeyedServiceDescriptor` 表达键控注册。
- **注册与容器分离**：键控描述符存入独立注册表，避免旧版 Microsoft 容器把键控描述符当作普通服务解析。
- **包装而非替换全部容器**：尽可能复用 Microsoft 默认容器的普通服务解析、作用域和释放机制。

### 6.3 代码位置

```text
src/Scorpio/
├── Scorpio/
│   └── DependencyInjection/
│       └── KeyedServices/
│           ├── KeyedServiceRegistry.cs
│           ├── KeyedServiceProvider.cs
│           ├── KeyedServiceScope.cs
│           ├── KeyedServiceActivator.cs
│           └── IKeyedServiceProvider.cs
└── Microsoft/
    └── Extensions/
        └── DependencyInjection/
            ├── KeyedServiceDescriptor.cs
            ├── FromKeyedServicesAttribute.cs
            ├── ServiceKeyAttribute.cs
            ├── KeyedService.cs
            ├── KeyedServiceCollectionExtensions.cs
            └── KeyedServiceProviderExtensions.cs
```

命名空间约定：

- 对外 API：`Microsoft.Extensions.DependencyInjection`，与原生一致
- 内部实现：`Scorpio.DependencyInjection.KeyedServices`
- 内部实现类型仅编译于 `!NET8_0_OR_GREATER`

### 6.4 对外 API 设计

对外 API 与 .NET 8 原生使用方式保持一致：

```csharp
using Microsoft.Extensions.DependencyInjection;

services.AddKeyedSingleton<ICache>("premium", new PremiumCache());
services.AddKeyedScoped<IUnitOfWork, SqlUnitOfWork>("sql");
services.AddKeyedTransient<ICommandHandler, CreateCommandHandler>("create");

var cache = provider.GetKeyedService<ICache>("premium");
var required = provider.GetRequiredKeyedService<ICache>("premium");
var caches = provider.GetKeyedServices<ICache>("premium");
```

需要提供的注册方法：

- `AddKeyedSingleton`
- `AddKeyedScoped`
- `AddKeyedTransient`
- `TryAddKeyedSingleton`
- `TryAddKeyedScoped`
- `TryAddKeyedTransient`
- `RemoveAllKeyed`

需要提供的解析方法：

- `GetKeyedService`
- `GetRequiredKeyedService`
- `GetKeyedServices`

需要提供的特性与辅助类型：

- `FromKeyedServicesAttribute`
- `ServiceKeyAttribute`
- `KeyedService`
- `IKeyedServiceProvider`
- `IServiceProviderIsKeyedService`

### 6.5 注册存储

兼容层通过派生 `ServiceDescriptor` 表达键控描述符，同时将键控注册保存在独立注册表中。

关键类型：

```csharp
public sealed class KeyedServiceDescriptor : ServiceDescriptor
{
    public object? ServiceKey { get; }
    public Type? KeyedImplementationType { get; }
    public object? KeyedImplementationInstance { get; }
    public Func<IServiceProvider, object?, object>? KeyedImplementationFactory { get; }
    public bool IsKeyedService { get; }
}

internal sealed class KeyedServiceRegistry
{
    // 以 (serviceType, serviceKey) 为索引保存键控注册
}
```

`KeyedServiceDescriptor` 的基类构造仅用于满足 `ServiceDescriptor` 类型约束，**不应被当作普通服务描述符直接交给旧版 Microsoft 容器**。键控描述符统一由 `KeyedServiceRegistry` 管理。

注册表应满足：

- 支持实现类型、实例和工厂三种注册形式。
- 支持开放泛型。
- 以 `(serviceType, key)` 为唯一索引，重复注册时后注册覆盖先注册。
- `TryAdd*` 仅在不存在相同索引时添加。
- `RemoveAll*` 删除相同 `serviceType + key` 的所有注册。
- 不将键控描述符直接写入 Microsoft 默认容器的普通服务描述符集合。

注册表通过 `IServiceCollection` 上的单例扩展对象保存，与现有 `ConventionalRegistrarList` 的附加模式保持一致。

### 6.6 解析与服务提供者包装

在默认 Microsoft DI 容器路径下，需要提供：

```csharp
internal sealed class KeyedServiceProvider :
    IServiceProvider,
    IKeyedServiceProvider,
    IServiceProviderIsKeyedService
{
}
```

包装器必须：

- 实现普通 `GetService(Type)`，将非键控解析委托给底层 `IServiceProvider`。
- 实现 `GetKeyedService(Type, object?)`。
- 实现 `GetRequiredKeyedService(Type, object?)`。
- 实现 `IsKeyedService(Type, object?)`。
- 提供 `CreateScope()`，作用域内的 `ServiceProvider` 同样支持键控解析。
- 保持普通服务解析、作用域验证和释放行为不被破坏。

`GetKeyedService` 的解析流程：

1. 将开放泛型请求匹配为闭合泛型。
2. 查找 `(serviceType, key)` 精确注册。
3. 未找到时查找 `KeyedService.AnyKey` 兜底注册。
4. 未找到时返回 `null`。
5. `GetRequiredKeyedService` 未找到时抛出 `InvalidOperationException`。
6. 命中后根据生命周期创建或缓存实例。

### 6.7 构造注入

构造注入是本兼容层的核心难点，也是验收硬性条件。

旧版 Microsoft DI 容器不认识 `[FromKeyedServices]` 和 `[ServiceKey]`，因此不能只包装 `IServiceProvider`。

必须引入 `KeyedServiceActivator`，实现键控感知的构造器解析：

```csharp
internal sealed class KeyedServiceActivator
{
    object CreateInstance(
        Type implementationType,
        object? serviceKey,
        IServiceProvider serviceProvider)
    {
        // 1. 按 Microsoft DI 规则选择构造函数。
        // 2. 解析普通参数。
        // 3. 处理 [FromKeyedServices] 参数。
        // 4. 处理 [ServiceKey] 参数。
    }
}
```

构造器解析规则：

- 优先选择“所有参数都能解析”的最多参数构造函数。
- 歧义时抛出 `InvalidOperationException`。
- `[FromKeyedServices(key)]` 参数按指定 key 解析。
- `[ServiceKey]` 参数注入当前解析使用的 key。
- `[ServiceKey]` 参数类型必须与 key 类型兼容，否则抛出 `InvalidOperationException`。

为保证普通服务中也能使用 `[FromKeyedServices]`，实现必须处理以下两种场景：

1. 解析键控服务时，由 `KeyedServiceActivator` 直接构造实现类型。
2. 解析普通服务时，预扫描普通注册，若实现类型构造函数包含键控特性，则将该普通服务替换为工厂注册，由 `KeyedServiceActivator` 构造。

只支持 `GetKeyedService` 定位、但不支持构造注入的实现不应视为完成。

### 6.8 作用域与释放

- 键控 Singleton 由根容器缓存。
- 键控 Scoped 由当前 `KeyedServiceScope` 缓存。
- 键控 Transient 每次创建新实例。
- 容器创建的可释放实例由对应作用域或根容器释放。
- 根容器解析 Scoped 键控服务时，遵循 Microsoft DI 的作用域验证规则。

`KeyedServiceScope` 应包装底层 `IServiceScope`，并保证：

```csharp
scope.ServiceProvider.GetKeyedService<T>(key)
```

与作用域外解析保持正确生命周期边界。

### 6.9 与 Scorpio 容器工厂集成

兼容层应提供默认 `IServiceProviderFactory<IServiceCollection>` 包装器：

```text
ScorpioKeyedServiceProviderFactory
```

该工厂在 `CreateServiceProvider` 时：

1. 先使用底层 `DefaultServiceProviderFactory` 创建普通容器。
2. 读取附加在 `IServiceCollection` 上的 `KeyedServiceRegistry`。
3. 返回 `KeyedServiceProvider` 包装器。

`Bootstrapper` 和 `AddScorpio<TStartupModule>()` 应默认使用该工厂。

当用户通过 `UseServiceProviderFactory<TContainerBuilder>()` 使用第三方容器时：

- 如果第三方容器支持键控服务，由其自行处理。
- 如果第三方容器不支持键控服务，Scorpio 不强制注入自研包装器，但应保证普通 DI 行为不回归。

### 6.10 与 .NET 8 原生实现的边界

- 兼容层只在 `#if !NET8_0_OR_GREATER` 下编译。
- `net8.0`、`net9.0`、`net10.0` 继续使用原生键控服务。
- 不生成与官方 `KeyedService`、`FromKeyedServicesAttribute`、`ServiceKeyAttribute` 同名同命名空间的类型。
- 在 `net8.0` 及以上不生成同名类型，避免与原生类型冲突。

## 7. 推荐结论

采用 **Scorpio 自研键控服务兼容层**：

- 不升级 `Microsoft.Extensions.DependencyInjection`。
- 对外使用与 .NET 8 原生一致的 `Microsoft.Extensions.DependencyInjection` API。
- 使用 `KeyedServiceDescriptor : ServiceDescriptor` 表达键控描述符。
- 内部实现独立注册表、键控服务提供者包装和键控感知构造器。
- 将 `[FromKeyedServices]` 和 `[ServiceKey]` 构造注入作为硬性验收条件。

## 8. 验收矩阵

以下验收项使用与 .NET 8 原生一致的 API（`AddKeyed*`、`GetKeyed*`、`GetRequiredKeyed*`）。

### 8.1 注册与解析

| 编号 | 场景 | 期望 |
| --- | --- | --- |
| AC-01 | 使用不同 key 注册同一服务类型 | 按 key 分别解析到对应实现 |
| AC-02 | 非键控注册与键控注册共存 | `GetService` 只返回非键控服务，`GetKeyedService` 只返回键控服务 |
| AC-03 | `GetKeyedService` 未找到 | 返回 `null` |
| AC-04 | `GetRequiredKeyedService` 未找到 | 抛出 `InvalidOperationException` |
| AC-05 | 重复注册相同 serviceType + key | 最后注册的实现生效 |
| AC-06 | `GetKeyedServices` 返回多个注册 | 按注册顺序返回 |
| AC-07 | key 为自定义对象且 `Equals` 相等 | 视为同一 key |
| AC-08 | key 为 `null` | 能注册并解析 |

### 8.2 AnyKey

| 编号 | 场景 | 期望 |
| --- | --- | --- |
| AC-09 | 注册精确 key 和 `KeyedService.AnyKey` | 精确 key 优先命中 |
| AC-10 | 未注册精确 key，但注册 `AnyKey` | 返回 `AnyKey` 兜底实现 |
| AC-11 | `GetKeyedServices(KeyedService.AnyKey)` | 返回 `AnyKey` 注册 |

### 8.3 生命周期

| 编号 | 场景 | 期望 |
| --- | --- | --- |
| AC-12 | 键控 Singleton | 同容器内同一实例 |
| AC-13 | 键控 Scoped | 同作用域内同一实例，不同作用域不同实例 |
| AC-14 | 键控 Transient | 每次解析新实例 |
| AC-15 | 容器/作用域释放 | 容器创建的可释放实例被正确释放 |

### 8.4 构造注入

| 编号 | 场景 | 期望 |
| --- | --- | --- |
| AC-16 | 构造函数参数使用 `[FromKeyedServices(key)]` | 注入对应 key 的实现 |
| AC-17 | 构造函数参数使用 `[ServiceKey]` 且类型匹配 | 注入当前 key |
| AC-18 | `[ServiceKey]` 参数类型与 key 类型不匹配 | 抛出 `InvalidOperationException` |

### 8.5 开放泛型

| 编号 | 场景 | 期望 |
| --- | --- | --- |
| AC-19 | 键控注册开放泛型服务 | 按 key 解析出构造后的泛型实例 |

### 8.6 多目标框架

| 编号 | 场景 | 期望 |
| --- | --- | --- |
| AC-20 | `netstandard2.0` / `net5.0` / `net6.0` / `net7.0` 构建 | 编译通过 |
| AC-21 | 对应测试目标运行 AC-01 ~ AC-19 | 全部通过 |
| AC-22 | `net8.0` / `net9.0` / `net10.0` 回归测试 | 不退化，继续使用原生实现 |

## 9. 非功能需求

- **线程安全**：键控解析应保持与 Microsoft DI 相同的并发安全性。
- **性能**：键控解析不应造成明显的额外分配或线性扫描；至少应缓存解析结果和键控注册索引。
- **可维护性**：自研实现必须遵循项目命名、目录映射、中文 XML 注释和测试规范，避免把兼容类型散落到业务模块。
- **隔离性**：兼容类型仅在 `!NET8_0_OR_GREATER` 下编译，`net8.0` 及以上使用原生类型，避免与原生键控服务冲突。
- **向后兼容**：现有非键控 DI、模块系统、Options、约定注册和 Generic Host 行为不得改变。

## 10. 风险与开放问题

1. 构造注入实现需要处理旧版 Microsoft DI 容器对 `[FromKeyedServices]` / `[ServiceKey]` 不识别的问题，复杂度较高。
2. 普通服务中若使用键控构造注入，需要预扫描并替换为工厂注册，可能增加容器构建时间和反射开销。
3. `ServiceDescriptor` 键控静态 API 无法完整对齐；需要确认框架用户是否依赖这些 API。
4. 在 `Microsoft.Extensions.DependencyInjection` 命名空间提供标准 API 时，是否会与其他也实现键控服务兼容层的框架冲突。
5. 第三方容器接入时，键控服务应完全由容器实现还是由 Scorpio 提供降级包装。
6. 开放泛型、`null` key 和 `AnyKey` 的组合在低版本目标上的行为需要完整测试。

## 11. 交付物

- 本需求文档。
- Scorpio 自研键控服务兼容层源码。
- 键控服务单元测试与多目标回归测试。
- 更新 `docs/technical-standards.md` 或开发指南中的兼容能力说明。
