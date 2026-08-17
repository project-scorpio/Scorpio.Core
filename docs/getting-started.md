# Scorpio.Core 使用指南

本文档面向使用 Scorpio.Core 开发应用的开发人员，说明如何引入框架、定义模块、注册服务、接入 Generic Host，以及使用 Options、插件、异常通知等核心能力。

## 1. 概念模型

Scorpio.Core 的应用入口是一个**启动模块**：

```text
StartupModule
    └── Bootstrapper / IHostBuilder.AddScorpio
            ├── 发现依赖模块与插件模块
            ├── 按依赖顺序执行 PreConfigureServices / ConfigureServices / PostConfigureServices
            ├── 创建 IServiceProvider
            └── 按依赖顺序执行 PreInitialize / Initialize / PostInitialize
```

模块之间通过 `[DependsOn]` 声明依赖，框架使用拓扑排序决定加载与初始化顺序。`KernelModule` 始终最先加载，启动模块通常最后加载。

## 2. 引入 Scorpio.Core

### 2.1 本地开发引用

在应用项目文件中添加项目引用：

```xml
<ItemGroup>
  <ProjectReference Include="..\..\src\Scorpio\Scorpio.csproj" />
  <ProjectReference Include="..\..\src\Scorpio.Hosting\Scorpio.Hosting.csproj" />
  <ProjectReference Include="..\..\src\Scorpio.Utilities\Scorpio.Utilities.csproj" />
</ItemGroup>
```

仅使用核心能力时，也可以只引用 `Scorpio` 与 `Scorpio.Utilities`；需要 Generic Host 集成时再引用 `Scorpio.Hosting`。

### 2.2 NuGet 引用

按程序集名称引用（包 ID 以实际发布物为准）：

```xml
<ItemGroup>
  <PackageReference Include="Scorpio" Version="0.1.2" />
  <PackageReference Include="Scorpio.Hosting" Version="0.1.2" />
  <PackageReference Include="Scorpio.Utilities" Version="0.1.2" />
</ItemGroup>
```

## 3. 最小可运行示例

### 3.1 定义模块

```csharp
using Scorpio.Modularity;

namespace MyApp
{
    public class MyAppModule : ScorpioModule
    {
    }
}
```

模块必须是非抽象、非泛型、实现 `IScorpioModule` 的类。通常继承 `ScorpioModule`。

### 3.2 使用 Bootstrapper 独立启动

```csharp
using Microsoft.Extensions.DependencyInjection;

using Scorpio;
using Scorpio.Modularity;

using (var bootstrapper = Bootstrapper.Create<MyAppModule>())
{
    bootstrapper.Initialize();

    var module = bootstrapper.ServiceProvider.GetRequiredService<MyAppModule>();
    // 使用 module 或从 ServiceProvider 解析其他服务
}
```

要点：

- `Bootstrapper.Create<T>()` 会加载模块并构建服务容器，但不会执行 `Initialize` 生命周期。
- 调用 `bootstrapper.Initialize()` 后才执行模块的 `PreInitialize`、`Initialize`、`PostInitialize`。
- `Dispose()` 会自动执行 `Shutdown()`，因此推荐使用 `using`。

### 3.3 接入 .NET Generic Host

引用 `Scorpio.Hosting` 后，使用 `AddScorpio<TStartupModule>()`：

```csharp
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using Scorpio.Modularity;

public class MyAppModule : ScorpioModule
{
}

public class Program
{
    public static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .AddScorpio<MyAppModule>()
            .RunConsoleAsync();
    }
}
```

Generic Host 模式下，框架会：

1. 替换 `IServiceProviderFactory<IServiceCollection>`。
2. 在创建服务容器时初始化 Scorpio 模块。
3. 在 `ApplicationStopping` 时执行模块 `Shutdown`。
4. 在 `ApplicationStopped` 时释放引导程序。

## 4. 模块生命周期

| 阶段 | 上下文 | 主要用途 |
| --- | --- | --- |
| `PreConfigureServices` | `ConfigureServicesContext` | 注册约定注册器、设置前置配置、替换框架服务 |
| `ConfigureServices` | `ConfigureServicesContext` | 注册模块服务、配置 Options、绑定配置 |
| `PostConfigureServices` | `ConfigureServicesContext` | 替换/装饰已注册服务、最终校验 |
| `PreInitialize` | `ApplicationInitializationContext` | 准备资源、预检查依赖 |
| `Initialize` | `ApplicationInitializationContext` | 启动核心功能、后台任务、中间件 |
| `PostInitialize` | `ApplicationInitializationContext` | 依赖其他模块完成后的操作、健康检查 |
| `Shutdown` | `ApplicationShutdownContext` | 释放资源、停止任务、保存状态 |

服务配置阶段按模块依赖顺序执行，初始化阶段也按依赖顺序执行；关闭阶段按相反顺序执行。

### 4.1 依赖声明

```csharp
using Scorpio.Modularity;

[DependsOn(typeof(AuditingModule))]
[DependsOn(typeof(DataModule))]
public class ApplicationModule : ScorpioModule
{
}
```

在 `NET7_0_OR_GREATER` 下还可以使用泛型形式：

```csharp
[DependsOn<AuditingModule>]
[DependsOn<DataModule>]
public class ApplicationModule : ScorpioModule
{
}
```

如果模块没有声明依赖，默认依赖 `KernelModule`。

### 4.2 在生命周期中注册和使用服务

```csharp
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Scorpio.DependencyInjection;
using Scorpio.Modularity;

public interface IGreeter
{
    string Greet(string name);
}

public class Greeter : IGreeter, ITransientDependency
{
    public string Greet(string name) => $"Hello, {name}!";
}

public class MyAppModule : ScorpioModule
{
    public override void Initialize(ApplicationInitializationContext context)
    {
        var greeter = context.ServiceProvider.GetRequiredService<IGreeter>();
        greeter.Greet("Scorpio");
    }
}
```

注意：`IGreeter` 与 `Greeter` 满足默认接口命名约定，因此即使没有 `ConfigureServices`，也会被自动注册。

## 5. 服务注册与生命周期

### 5.1 标记接口自动注册

| 标记接口 | 生命周期 |
| --- | --- |
| `ITransientDependency` | Transient |
| `IScopedDependency` | Scoped |
| `ISingletonDependency` | Singleton |

默认选择规则：

- 如果实现类 `Foo` 实现了 `IFoo`，且 `Foo` 名称以接口名去掉 `I` 后缀结尾，则注册 `IFoo` 和 `Foo` 两个服务。
- 所有标记服务也会注册实现类自身。

### 5.2 显式暴露服务

使用 `[ExposeServices]` 精确控制服务类型和生命周期：

```csharp
using Microsoft.Extensions.DependencyInjection;

using Scorpio.DependencyInjection;

[ExposeServices(typeof(IEmailSender), ServiceLifetime = ServiceLifetime.Singleton)]
public class EmailSender : IEmailSender
{
}
```

### 5.3 手动注册

仍然可以使用标准 `IServiceCollection` 扩展：

```csharp
public override void ConfigureServices(ConfigureServicesContext context)
{
    context.Services.AddTransient<IMyService, MyService>();
    context.Services.AddOptions<MyOptions>()
        .Bind(context.Configuration.GetSection("MyOptions"));
}
```

框架还提供了 `ReplaceOrAdd`、`ReplaceSingleton`、`ReplaceTransient`、`ReplaceScoped`、`RemoveService<T>` 等服务集合增强方法。

## 6. Options 配置

Scorpio.Core 扩展了 .NET Options，支持 `PreConfigure -> Configure -> PostConfigure` 三阶段。

```csharp
using Scorpio.Options;

public class MyOptions : ExtensibleOptions
{
    public string Name { get; set; } = "default";
}
```

在模块中配置：

```csharp
public override void ConfigureServices(ConfigureServicesContext context)
{
    context.Services.Options<MyOptions>()
        .PreConfigure(options => options.Name = "pre")
        .Configure(options => options.Name += "-configured");

    context.Services.Configure<MyOptions>(options => options.Name += "-post");
}
```

> `PreConfigure` 来自 Scorpio 的 `OptionsBuilder`，`Configure` 是 Microsoft 标准扩展。

`ExtensibleOptions` 允许运行时挂载键值扩展配置：

```csharp
options.SetOption("TraceId", Guid.NewGuid().ToString());
var traceId = options.GetOption<string>("TraceId");
```

## 7. 可排序初始化

实现 `IInitializable` 的类型会被自动扫描；使用 `[InitializationOrder]` 控制顺序，数值越大越先执行。

```csharp
using Scorpio.DependencyInjection;
using Scorpio.Initialization;

[InitializationOrder(100)]
public class DatabaseInitializer : IInitializable, ITransientDependency
{
    public void Initialize()
    {
        // 执行数据库初始化
    }
}
```

`KernelModule.Initialize` 会调用 `IInitializationManager` 执行这些初始化逻辑。

## 8. 插件模块

插件模块不是启动模块的显式依赖，而是在 `BootstrapperCreationOptions.PlugInSources` 中动态加入。

```csharp
using Scorpio;
using Scorpio.Modularity.Plugins;

using (var bootstrapper = Bootstrapper.Create<MyAppModule>(options =>
{
    options.PlugInSources.AddType<MyPluginModule>();
}))
{
    bootstrapper.Initialize();
}
```

支持的插件源：

| 方法 | 说明 |
| --- | --- |
| `AddType<TModule>()` / `AddType(params Type[])` | 直接指定模块类型 |
| `AddFile(params string[] filePaths)` | 从 `.dll` / `.exe` 程序集文件加载 |
| `AddFolder(string path, Func<string,bool> predicate = null)` | 扫描目录并按过滤器加载程序集 |

## 9. 异常通知

Scorpio.Core 提供发布-订阅式异常通知：

```csharp
using System;
using System.Threading.Tasks;

using Scorpio.ExceptionHandling;
using Scorpio.DependencyInjection;

public class ConsoleExceptionSubscriber : ExceptionSubscriber
{
    public override async Task HandleAsync(ExceptionNotificationContext context)
    {
        Console.WriteLine(context.Exception);
        await Task.CompletedTask;
    }
}
```

`ExceptionSubscriber` 已通过 `[ExposeServices(typeof(IExceptionSubscriber))]` 声明为异常订阅者；`ExceptionNotifier` 会自动遍历所有订阅者。

在业务代码中通知异常：

```csharp
var notifier = serviceProvider.GetRequiredService<IExceptionNotifier>();
await notifier.NotifyAsync(ex);
```

## 10. AOP 拦截器

框架在 `Scorpio.DynamicProxy` 中定义了 `IInterceptor`、`IMethodInvocation` 和约定拦截扩展，但**不包含具体代理容器实现**。实际拦截需要由外部 DI/代理容器接入，例如 Castle、Autofac.DynamicProxy 或 AspectCore 对应的服务提供程序工厂。

```csharp
public class LoggingInterceptor : IInterceptor
{
    public async Task InterceptAsync(IMethodInvocation invocation)
    {
        Console.WriteLine($"Before: {invocation.Method.Name}");
        await invocation.ProceedAsync();
        Console.WriteLine($"After: {invocation.Method.Name}");
    }
}
```

约定拦截入口为 `services.RegisterConventionalInterceptor(types, config => ...)` 与 `ConventionalInterceptorExtensions.Intercept<TInterceptor>()`，但只有在注册了 `IProxyConventionalAction` 的代理容器环境中才会真正生效。

## 11. 常用工具速览

| 能力 | 关键 API |
| --- | --- |
| 参数校验 | `Check.NotNull`、`Check.NotNullOrEmpty`、`Check.NotNullOrWhiteSpace`、`Check.AssignableTo<T>()` |
| 集合扩展 | `IsNullOrEmpty`、`WhereIf`、`ForEach`、`ForEachAsync`、`GetOrAdd` |
| 字符串扩展 | `ToCamelCase`、`ToPascalCase`、`ToEnum`、`ToMd5`、`EnsureEndsWith` |
| 表达式 | `PredicateBuilder`、`ConditionalPredicateBuilder`、`ReplaceExpressionVisitor` |
| 异步 LINQ | `AsyncQueryable`、`AsyncEnumerable` |
| 中间件管道 | `IPipelineBuilder<TContext>`、`PipelineBuilder` |

## 12. 常见注意事项

- 模块类型必须是 `class`、非抽象、非泛型且实现 `IScorpioModule`。
- `Bootstrapper.ServiceProvider` 在 `Create<T>()` 后已可用，但模块的初始化生命周期要等到 `Initialize()`。
- 默认自动服务注册按模块程序集扫描；实现标记接口的类不要忘记是 public 且满足标准类型筛选。
- 源码库目标框架包含 `netstandard2.0`，新增代码应保持多目标兼容；避免使用仅在单个目标框架可用的 API 时不做条件编译。
- 如果模块需要完全控制服务注册，可在模块内设置 `SkipAutoServiceRegistration = true`，但该属性是 `protected internal`，只在模块子类中可用。
- 框架默认使用官方 NuGet 源；不要向项目内添加私有源，除非有明确的发布需求。
