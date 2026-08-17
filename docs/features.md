# 功能说明（Feature Description）

本文档介绍 Scorpio.Core 各功能模块的职责、关键类型与核心机制，供后续开发与扩展时查阅。

## 1. 引导程序（Bootstrapper）

**位置**：`src/Scorpio/Scorpio`（`Bootstrapper.cs`、`IBootstrapper.cs`、`InternalBootstrapper.cs` 等）

**职责**：应用程序的入口与生命周期管理者。

关键类型：

- `IBootstrapper`：引导程序接口（`IDisposable`），暴露 `StartupModuleType`、`Services`（`IServiceCollection`）、`Configuration`、`Properties`（键值共享状态）、`ServiceProvider`，以及 `Initialize(params object[])` 与 `Shutdown()`。
- `Bootstrapper`：抽象基类，编排「构建服务容器 → 加载模块 → 注册服务 → 初始化模块 → 初始化管线」的完整流程。
- `InternalBootstrapper`：内部实现（`internal`）。
- `BootstrapperCreationOptions`：引导程序创建选项。
- `KernelModule`：框架核心模块，永远最先加载。
- `IServiceFactoryAdapter` / `ServiceFactoryAdapter`：服务工厂适配器，隔离具体 DI 容器的差异。

## 2. 模块系统（Modularity）

**位置**：`src/Scorpio/Scorpio/Modularity`

**职责**：以模块为粒度组织应用，支持模块依赖与生命周期。

核心机制：

- `IScorpioModule` / `ScorpioModule`：模块契约与抽象基类，提供 7 个生命周期虚方法：

```
PreConfigureServices → ConfigureServices → PostConfigureServices
→ PreInitialize → Initialize → PostInitialize → Shutdown
```

- `DependsOnAttribute` / `DependsOnAttribute<TModule>`（`NET7_0_OR_GREATER` 下的泛型版本，`AllowMultiple = true`）：声明模块依赖。
- `IModuleLoader` / `ModuleLoader`：从启动模块递归发现依赖，并从插件源加载插件，返回 `IModuleDescriptor[]`。
- `IModuleManager` / `ModuleManager`：按序执行三个初始化阶段，按逆序关闭模块。
- `ModuleHelper`：DFS 查找模块类型；未声明依赖时默认依赖 `KernelModule`。
- `ListExtensions.SortByDependencies`：依赖拓扑排序。
- 上下文对象：`ConfigureServicesContext`、`ApplicationInitializationContext`、`ApplicationShutdownContext`。

## 3. 插件系统（Plugins）

**位置**：`src/Scorpio/Scorpio/Modularity/Plugins`

**职责**：动态加载外部模块（程序集）。

核心机制：

- `IPlugInSource`：插件源契约（`GetModules()`）。
- 三种插件源实现：
  - `TypePlugInSource`：直接指定模块类型。
  - `FilePlugInSource`：从指定程序集文件（`.dll`/`.exe`）加载。
  - `FolderPlugInSource`：扫描文件夹，用 `Matcher` + `IFileProvider` + `AssemblyLoadContext` 加载。
- `IPlugInSourceList` / `PlugInSourceList`：插件源集合。
- `PlugInSourceListExtensions`：`AddType<TModule>()`、`AddFile(...)`、`AddFolder(...)` 等便捷方法。

## 4. 依赖注入与约定式注册

**位置**：`src/Scorpio/Scorpio/DependencyInjection`（含 `Conventional` 子目录）、`src/Scorpio/Scorpio/Conventional`

**职责**：提供标记接口式自动注册与可扩展的约定注册框架。

### 4.1 标记接口

- `IDependency`（空标记）、`ITransientDependency` / `IScopedDependency` / `ISingletonDependency`：实现即按对应生命周期自动注册。
- `IExposedServiceTypesProvider` / `ExposeServicesAttribute`：显式声明暴露的服务类型与生命周期。
- `NotAutowiredAttribute`：跳过自动注入；`ReplaceServiceAttribute`：替换同名服务。
- `IServiceProviderAccessor`：统一获取 `IServiceProvider`。
- `IHybridServiceScopeFactory` / `DefaultServiceScopeFactory`：混合作用域工厂。

### 4.2 约定注册框架（Conventional）

模板方法模式：**配置（Configuration）→ 上下文（Context）→ 动作（Action）**。

- `IConventionalRegistrar` / `ConventionalActionBase`：注册器与动作基类。
- `ConventionalConfiguration` / `ConventionalContext`：持有服务与类型集合，用 `Expression<Func<Type,bool>>` 筛选类型。
- 选择器：`ServiceSelector`（`DefaultInterfaceSelector` 按 `IFoo`↔`Foo` 命名匹配、`AllInterfaceSelector`、`SelfSelector`、`ExposeServicesSelector`）与 `LifetimeSelector`（Transient/Scoped/Singleton）。
- 扩展：`As<T>()`、`AsDefault()`、`AsAll()`、`AsSelf()`、`AsExposeService()`、`Lifetime(...)`。

### 4.3 Microsoft.Extensions.DependencyInjection 扩展

`src/Scorpio/Microsoft/Extensions/DependencyInjection` 下：

- `ServiceCollectionExtensions`：`GetSingletonInstanceOrNull/OrAdd`、`ReplaceSingleton`、`ReplaceTransient`、`RemoveService<T>`、`ReplaceOrAdd` 等。
- `DependencyInjectionServiceCollectionExtensions`：`AddConventionalRegistrar`、`RegisterAssemblyByConvention`。

## 5. AOP 与动态代理

**位置**：`src/Scorpio/Scorpio/Aspects`、`src/Scorpio/Scorpio/DynamicProxy`

**职责**：横切关注点管理与拦截器抽象。

### 5.1 横切关注点（Aspects）

- `IAvoidDuplicateCrossCuttingConcerns`：记录已应用关注点名称，避免重复应用。
- `CrossCuttingConcerns`（静态类）：`AddApplied` / `RemoveApplied` / `IsApplied` / `Applying`（RAII 临时应用）。内部先 `UnProxy()` 再操作。

### 5.2 动态代理（DynamicProxy）

框架只定义**拦截器抽象**，实际代理由外部容器（Autofac / Castle / AspectCore）实现：

- `IInterceptor`：`Task InterceptAsync(IMethodInvocation invocation)`。
- `IMethodInvocation`：`Arguments`、`ArgumentsDictionary`、`GenericArguments`、`TargetObject`、`Method`、`ReturnValue`、`ProceedAsync()`。
- `IMethodInvocationRuntimeExtensions.IsAsync`：判定异步方法（缓存判定结果，识别 `Task/Task<T>/ValueTask/ValueTask<T>`）。
- `IProxyTargetProvider` / `ProxyHelper`：`IsProxy` / `UnProxy`。
- 约定拦截：`ConventionalInterceptorAction`、`ConventionalInterceptorExtensions.Intercept<TInterceptor>()`。

## 6. 异常处理（ExceptionHandling）

**位置**：`src/Scorpio/Scorpio/ExceptionHandling`

**职责**：发布-订阅式异常通知。

- `IExceptionNotifier` / `ExceptionNotifier`（`ITransientDependency`）：创建作用域，逐一遍历 `IExceptionSubscriber` 执行 `HandleAsync`；单个订阅者异常被捕获并记录 Error 日志。
- `IExceptionSubscriber` / `ExceptionSubscriber`（抽象基类）。
- `ExceptionNotificationContext`：`Exception`、`LogLevel`（可由 `exception.GetLogLevel()` 推断）、`Handled`。
- `NullExceptionNotifier`：空对象模式单例。
- `ILocalizeErrorMessage`：异常消息本地化契约。

## 7. 初始化管线（Initialization）

**位置**：`src/Scorpio/Scorpio/Initialization`

**职责**：按指定顺序初始化 `IInitializable` 类型。

- `IInitializable`：`void Initialize()`。
- `IInitializationManager` / `InitializationManager`：从 `InitializationOptions.Initializables`（`SortedDictionary<int, ITypeList<IInitializable>>`，降序）逐个解析并初始化。
- `InitializationOrderAttribute`：`Order` 越大越先执行（默认 0）。
- `InitializationOptions`：`AddInitializable<T>(order)`。
- `InitializationConventionalRegistrar` / `InitializationConventionalAction`：自动扫描 `IInitializable` 实现。

## 8. 配置选项（Options）

**位置**：`src/Scorpio/Scorpio/Options`

**职责**：扩展 .NET Options 机制，支持三阶段配置。

- `OptionsFactory<TOptions>`：`PreConfigure → Configure → PostConfigure` 三阶段，在 `KernelModule.PreConfigureServices` 中替换默认 `IOptionsFactory<>`。
- `OptionsBuilder<TOptions>`：新增 `PreConfigure`（最多 4 个依赖参数的泛型重载）。
- `IPreConfigureOptions<in TOptions>` / `PreConfigureOptions<TOptions>`（及 1~4 依赖泛型版本）。
- `ExtensibleOptions` / `ExtensibleOptionsExtensions`：键值对式动态扩展选项（`GetOption<T>` / `SetOption<T>`）。

## 9. 本地化（Localization）

**位置**：`src/Scorpio/Scorpio/Localization`

- `LocalizationContext`：基于 `AsyncLocal<LocalizationContext>` 的上下文，持有 `IServiceProvider` 与 `IStringLocalizerFactory`；`Use(context)` 返回 `DisposeAction`。
- `CultureHelper`：`Use(culture, uiCulture?)` 临时切换 `CurrentCulture/CurrentUICulture`，释放时恢复。

## 10. 线程 / 时钟 / 异步 / 运行时

**位置**：`src/Scorpio/Scorpio/Threading`、`Timing`、`Linq`、`Runtime`、`Security`

| 模块 | 关键类型 | 说明 |
| --- | --- | --- |
| Threading | `AsyncHelper` | `IsAsync`、`IsTask`、`UnwrapTask`、`RunSync`（基于 `Nito.AsyncEx.AsyncContext`） |
| Threading | `IScorpioTimer` / `ScorpioTimer` | 不重叠执行的健壮计时器（`Period`、`RunOnStart`、`Elapsed`） |
| Threading | `ICancellationTokenProvider` / `NoneCancellationTokenProvider` | 取消令牌提供者（默认 `CancellationToken.None`） |
| Threading | `LockExtensions.Locking(...)` | `lock` 的链式封装 |
| Timing | `IClock` / `Clock` | 时钟抽象（`Now`、`Kind`、`Normalize`） |
| Linq | `IAsyncQueryableExecuter` / `DefaultAsyncQueryableExecuter` | 异步查询执行器（`CountAsync`、`ToListAsync`、`FirstOrDefaultAsync`） |
| Runtime | `IAmbientDataContext` / `AsyncLocalAmbientDataContext` | 基于 `AsyncLocal<object>` 的环境数据 |
| Runtime | `IAmbientScopeProvider<T>` / `AmbientDataContextAmbientScopeProvider<T>` | 环境作用域链（`ScopeItem` + `ConcurrentDictionary` + `DisposeAction`） |
| Runtime | `IRunnable` | `StartAsync/StopAsync` 可运行契约 |
| Security | `ICurrentPrincipalAccessor` / `ThreadCurrentPrincipalAccessor` | `Thread.CurrentPrincipal` 访问器 |

## 11. 宿主集成（Scorpio.Hosting）

**位置**：`src/Scorpio.Hosting`

**职责**：将 Scorpio 引导程序接入 .NET Generic Host。

- `InternalBootstrapper`（`internal`）：暴露 `SetServiceProviderInternal`。
- `ServiceProviderFactory`（`internal`，`IServiceProviderFactory<IServiceCollection>`）：`CreateBuilder` 时创建 `InternalBootstrapper` 并保存 `IHostEnvironment`；`CreateServiceProvider` 时注册 `ApplicationStopping → Shutdown`、`ApplicationStopped → Dispose` 回调并调用 `Initialize()`。
- `HostBuilderExtensions`：`IHostBuilder.AddScorpio<TStartupModule>()` / `AddScorpio(Type, optionsAction)`。

## 12. 工具库（Scorpio.Utilities）

**位置**：`src/Scorpio.Utilities`

### 12.1 参数校验 `Check`

`Scorpio.Check`（`[DebuggerStepThrough]`）：`NotNull`、`NotNullOrDefault`、`NotNullOrWhiteSpace`、`NotNullOrEmpty`、`AssignableTo<TBaseType>` 等，返回原值以支持链式调用。

### 12.2 类型集合

`ITypeList` / `TypeList`、`ITypeDictionary` / `TypeDictionary`：类型安全的强类型集合（`Add<T>()`、`TryAdd<T>()`、`Contains<T>()`、`Remove<T>()`、`GetOrDefault<TKey>()`）。

### 12.3 中间件管道（洋葱模型）

`src/Scorpio.Utilities/Scorpio/Middleware/Pipeline`：

- `IPipelineBuilder<TPipelineContext>` / `PipelineBuilder<TPipelineContext>`：`Build()` 用 `Aggregate` 组装洋葱模型。
- `PipelineBuilder`（静态扩展）：`Use` 委托、`UseMiddleware<TMiddleware>`（表达式树编译注入）。
- `PipelineRequestDelegate<in TPipelineContext>`：管道请求委托。

### 12.4 异常/日志标记接口

- `IHasErrorCode`（`Code`）、`IHasErrorDetails`（`Details`）。
- `IHasLogLevel`、`IExceptionWithSelfLogging`。
- `IRemoteService`：远程服务标记接口。

### 12.5 BCL 扩展（`System.*` 命名空间）

| 命名空间 | 关键扩展 |
| --- | --- |
| `System.Collections.Generic` | `CollectionExtensions`（`IsNullOrEmpty`、`AddIfNotContains`、`RemoveAll`、`GetOrAdd`）、`DictionaryExtensions`、`EnumerableExtensions`（`WhereIf`、`ForEach`、`ForEachAsync`、`AnyAsync`、`AllAsync`、`ExpandToString`）、`ListExtensions`（`FindIndex`、`AddFirst/AddLast`、`InsertAfter/Before`、`MoveItem`） |
| `System.Linq` | `QueryableExtensions`（`PageBy`、`WhereIf`）、`QueryableMethods`（缓存 LINQ 方法 `MethodInfo`） |
| `System.Linq.Expressions` | `PredicateBuilder`（`True/False/Equal/And/Or`）、`ConditionalPredicateBuilder`、`TranslationBuilder`、`ReplaceExpressionVisitor` |
| `System.Linq.Async` | `AsyncQueryable`（`Average/Count/First/Sum/...`，EF Core 风格）、`AsyncEnumerable`、`IAsyncQueryProvider`、`AsyncIteratorBase<TSource>` |
| `System.Reflection` | `TypeExtensions`（`IsStandardType`、`IsAssignableTo`）、`MemberInfoExtensions`（`GetAttribute`、`GetDescription`、`GetDisplayName`） |
| 其它 | `ObjectExtensions`（`As<T>`、`To<T>`、`SafelyDispose`）、`StringExtensions`（`EnsureEndsWith/StartsWith`、`ToCamelCase/ToPascalCase`、`ToEnum`、`ToMd5`、`Truncate`）、`ExceptionExtensions`（`ReThrow`、`GetLogLevel`）、`DateTimeExtensions`（`ToUnixTimestamp`）、`DisposeAction`/`AsyncDisposeAction`、`NullDisposable`/`NullAsyncDispose` |

## 13. 类型帮助器（System.Reflection）

- `TypeHelper`（`src/Scorpio/System/Reflection`）：`IsNonNullablePrimitiveType`、`IsFunc`、`IsPrimitiveExtended`、`IsNullable`、`IsEnumerable` 等类型判断与转换。
