# 开发规范（Development Standards）

本文档规定 Scorpio.Core 项目的编码与开发约定，所有新增代码必须遵循。约定均基于现有代码实际观察总结而来。

## 1. 项目结构规范

```
Scorpio.Core/
├── src/                        # 源码项目（可打包发布，多目标框架）
│   ├── Scorpio/                # 核心库（命名空间 Scorpio / Microsoft.Extensions.* / System.Reflection）
│   ├── Scorpio.Hosting/        # 宿主库（命名空间 Scorpio / Microsoft.Extensions.Hosting）
│   └── Scorpio.Utilities/      # 工具库（命名空间 Scorpio / System.* / Microsoft.Extensions.*）
├── test/                       # 测试项目（不可打包）
│   ├── Scorpio.TestBase/       # 测试基础设施（基类、断言扩展）
│   ├── Scorpio.Tests/          # 核心库测试
│   ├── Scorpio.Hosting.Tests/  # 宿主库测试
│   └── Scorpio.Utilities.Tests/# 工具库测试
├── common.props                # 公共元数据（作者、许可、目标框架列表）
├── versions.props              # 版本号生成
├── Directory.Build.props       # 根构建属性（引入 versions/common）
├── Directory.Build.targets     # 根构建目标（清空 RootNamespace）
├── Directory.Packages.props    # 中央包版本管理
└── nuget.config                # NuGet 源配置
```

### 1.1 目录与命名空间映射

命名空间必须与目录路径严格对应：

| 目录 | 命名空间 |
| --- | --- |
| `src/Scorpio/Scorpio/Modularity/Plugins` | `Scorpio.Modularity.Plugins` |
| `src/Scorpio/Microsoft/Extensions/DependencyInjection` | `Microsoft.Extensions.DependencyInjection` |
| `src/Scorpio.Utilities/System/Linq` | `System.Linq` |
| `src/Scorpio.Utilities/System/Collections/Generic` | `System.Collections.Generic` |

> 注意：`Microsoft` 与 `System` 目录下的扩展类型，其命名空间使用官方 BCL / 框架命名空间（与官方扩展合并），而非 `Scorpio.*`，这是本项目的核心约定。

### 1.2 新增功能模块的放置规则

- 框架自有类型放在 `Scorpio/Scorpio/<Area>/`，命名空间 `Scorpio.<Area>`。
- 对 `Microsoft.Extensions.*` 的扩展放在 `Microsoft/Extensions/<Area>/`，命名空间 `Microsoft.Extensions.<Area>`。
- 对 BCL 的通用扩展放在 `System/<Area>/`，命名空间 `System.<Area>`。
- 每个 `<Area>` 内接口与实现分文件放置，`internal` 实现与公开 API 分开放置。

## 2. 命名规范

| 元素 | 约定 | 示例 |
| --- | --- | --- |
| 接口 | `I` 前缀 | `IScorpioModule`、`IDependency`、`IClock` |
| 抽象类 | 无特殊前后缀 | `ScorpioModule`、`Bootstrapper`、`ConventionalActionBase` |
| 类/结构体 | PascalCase | `ModuleLoader`、`TypeList` |
| 属性/方法 | PascalCase | `ServiceProvider`、`InitializeModules` |
| 私有字段 | `_camelCase` | `_serviceFactory`、`_logger` |
| 常量 | PascalCase | `Interceptors` |
| 局部变量/参数 | camelCase | `startupModuleType` |
| 泛型类文件 | 类型名加 `_T` 后缀 | `PipelineBuilder_T.cs`、`ExpressionTranslation_T.cs` |
| 测试类/文件 | `<被测类型>_Tests(.cs)` | `Bootstrapper_Tests.cs`、`Check_Tests.cs` |

## 3. 代码组织规范

### 3.1 文件组织

- 原则：**一个类型一个文件**，文件名 = 类型名。
- 少数强相关类型可共用一个文件（如选择器集合 `ServiceSelector.cs`、`LifetimeSelector.cs`、同步+异步泛型重载 `PreConfigureOptions.cs`）。
- 接口与实现分离为独立文件。

### 3.2 命名空间声明

- **必须使用 block namespace**，禁止使用 file-scoped namespace：

```csharp
namespace Scorpio.Modularity
{
    public class ModuleLoader : IModuleLoader
    {
        // ...
    }
}
```

### 3.3 using 指令组织

按下列顺序**分组**，组间用空行分隔，组内按字母序排列：

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Scorpio.Modularity.Plugins;

using Nito.AsyncEx;   // 第三方库（如有）
```

分组顺序：`System.*` → `Microsoft.*` → `Scorpio.*` → 第三方库。

## 4. 注释规范

- **所有公开（public/protected）成员必须写中文 XML 文档注释**。
- 注释结构应包含：`<summary>`，必要时 `remarks`（支持 `<list>`、`<para>`、`<example>`、`<code>`）、`<param>`、`<returns>`、`<value>`、`<exception>`、`<seealso>` / `<see cref>`。
- `internal`/`private` 成员也鼓励添加中文注释；方法体内可用中文行内 `//` 注释说明关键逻辑。
- 典型示例：

```csharp
/// <summary>
/// 检查指定的值不为 null
/// </summary>
/// <typeparam name="T">值的类型</typeparam>
/// <param name="value">要检查的值</param>
/// <param name="parameterName">参数名称，用于异常消息</param>
/// <returns>如果值不为 null，则返回原始值</returns>
/// <exception cref="ArgumentNullException">当 <paramref name="value"/> 为 null 时抛出</exception>
public static T NotNull<T>(T value, string parameterName) where T : class
{
    if (value == null)
    {
        throw new ArgumentNullException(parameterName);
    }

    return value;
}
```

- 当 `cref` 因条件编译等原因无法解析时，用 `#pragma warning disable CS1574` 包裹。
- 分析器抑制统一使用 `[SuppressMessage(...)]` 特性或 `#pragma warning disable Sxxxx`。

## 5. 异常与参数校验规范

- **参数校验统一使用 `Check` 静态类**（位于 `Scorpio.Utilities`，命名空间 `Scorpio`）：
  - `Check.NotNull(value, nameof(value))`
  - `Check.NotNullOrDefault(value, nameof(value))`
  - `Check.NotNullOrEmpty(...)` / `Check.NotNullOrWhiteSpace(...)`
  - `Check.AssignableTo<T>(type, nameof(type))`
- `Check` 方法返回原值，支持表达式体赋值：

```csharp
ServiceScopeFactory = Check.NotNull(serviceScopeFactory, nameof(serviceScopeFactory));
```

- 框架业务异常统一使用 `ScorpioException`（`[Serializable]`，含序列化构造，`#if !NET8_0_OR_GREATER`）。
- 无框架语义的局部校验可直接 `throw new ArgumentNullException(nameof(x))` / `ArgumentException`。

## 6. 依赖注入使用规范

- **约定式注册**：服务类实现标记接口即可自动注册，无需手动 `AddXxx`：
  - `ITransientDependency` → Transient
  - `IScopedDependency` → Scoped
  - `ISingletonDependency` → Singleton
- 需要显式控制暴露的服务类型时，使用 `[ExposeServices(typeof(IXxx))]`。
- 属性跳过自动注入使用 `[NotAutowired]`；替换同名服务使用 `[ReplaceService]`。
- 需要自定义约定注册时，实现 `IConventionalRegistrar` 并通过 `AddConventionalRegistrar` 注册。

## 7. 模块开发规范

1. 新模块继承 `ScorpioModule`，覆写生命周期方法：
   `PreConfigureServices → ConfigureServices → PostConfigureServices → PreInitialize → Initialize → PostInitialize → Shutdown`。
2. 使用 `[DependsOn(typeof(XxxModule))]`（`AllowMultiple = true`）声明模块依赖；不声明依赖时默认依赖 `KernelModule`。
3. 模块类必须满足：非抽象、非泛型、实现 `IScorpioModule`（`ScorpioModule.IsModule` 校验）。
4. 需要跳过自动服务注册时，覆写 `SkipAutoServiceRegistration => true`。
5. 初始化顺序敏感的逻辑实现 `IInitializable` 并用 `[InitializationOrder]` 指定顺序（值越大越先执行）。

## 8. 测试规范

- 使用 **xunit** + **Shouldly** 作为主测试与断言库；模拟优先 **Moq**（必要时 NSubstitute）。
- 测试类命名 `<被测类型>_Tests`，文件 `<被测类型>_Tests.cs`。
- 测试方法命名采用行为驱动风格：`Should_<行为>_<场景>`（如 `Should_Initialize_Single_Module`）。
- 需要服务容器环境的集成测试继承 `Scorpio.TestBase` 中的 `IntegratedTest<TStartupModule>` 或 `TestBaseWithServiceProvider`。
- 断言风格示例：

```csharp
result.ShouldNotBeNull();
result.ShouldBeOfType<SomeType>();
Should.Throw<ScorpioException>(() => action());
```

- 服务注册断言可使用 `ServiceCollectionShouldlyExtensions`（`ShouldContainTransient/Singleton/Scoped`）。

## 9. 代码风格（强制项）

- `LangVersion = latest`，鼓励使用现代 C# 语法：`switch` 表达式、模式匹配（`is not`）、`??/??=`、表达式体成员、`nameof`、目标类型 `new`、`default` 字面量。
- **不启用可空引用类型**（不要引入 `#nullable enable` 或 `string?` 标注），保持与现有代码一致。
- **不使用 `record`** 定义公开类型。
- 多目标框架兼容：涉及反射/类型操作时使用 `GetTypeInfo()` / `TypeInfo` 等 netstandard 兼容 API。
- 条件编译按目标框架使用 `#if NET7_0_OR_GREATER`、`#if !NET8_0_OR_GREATER` 等预定义符号。
