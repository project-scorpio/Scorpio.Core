# Scorpio.Core

Scorpio.Core 是一个模块化、跨平台的 .NET 基础框架，采用类 ABP 的模块化与约定式开发风格。它将引导程序、模块生命周期、依赖注入、插件加载、AOP 抽象、配置选项、异常处理、本地化、时钟/线程工具及常用 BCL 扩展能力封装成三个可复用程序集。

## 核心能力

- 模块系统：以 `IScorpioModule` / `ScorpioModule` 为核心，支持 `[DependsOn]` 依赖声明、拓扑排序、插件模块加载和完整生命周期。
- 引导程序：`Bootstrapper` 统一编排配置、服务注册、服务容器创建、模块初始化与关闭。
- 约定式 DI：服务实现 `ITransientDependency` / `IScopedDependency` / `ISingletonDependency` 或使用 `[ExposeServices]` 即可自动注册。
- .NET Generic Host 集成：通过 `AddScorpio<TStartupModule>()` 将框架接入 `IHostBuilder`。
- 基础设施：三阶段 Options、可排序初始化、发布订阅异常通知、本地化上下文、AOP 拦截器抽象、时钟与计时器。
- 工具扩展：参数校验、集合/字符串/表达式/反射/异步 LINQ/中间件管道等常用扩展。

## 项目结构

```text
Scorpio.Core/
├── src/
│   ├── Scorpio/             # 核心库：模块、引导、DI、AOP、Options 等
│   ├── Scorpio.Hosting/     # .NET Generic Host 集成
│   └── Scorpio.Utilities/   # 通用工具与 BCL/框架扩展
├── test/
│   ├── Scorpio.TestBase/
│   ├── Scorpio.Tests/
│   ├── Scorpio.Hosting.Tests/
│   └── Scorpio.Utilities.Tests/
├── docs/                    # 项目文档
├── common.props
├── versions.props
├── Directory.Build.props
├── Directory.Build.targets
├── Directory.Packages.props
├── Scorpio.Core.sln
└── Scorpio.Core.slnx
```

## 环境要求

- .NET SDK 10.0 或更高版本（源码项目多目标编译包含 `net10.0`）。
- 本地应安装构建测试所需的 `netcoreapp3.1`、`net5.0` ~ `net10.0` 运行时；如果只跑单个目标框架测试，可只安装对应运行时。
- NuGet 使用 `nuget.config` 中的官方源 `https://api.nuget.org/v3/index.json`。

## 快速开始

定义一个启动模块，然后通过引导程序启动：

```csharp
using Microsoft.Extensions.DependencyInjection;

using Scorpio;
using Scorpio.Modularity;

public class HelloModule : ScorpioModule
{
}

using (var bootstrapper = Bootstrapper.Create<HelloModule>())
{
    bootstrapper.Initialize();
    var module = bootstrapper.ServiceProvider.GetRequiredService<HelloModule>();
}
```

更完整的模块开发、依赖注入、Generic Host、Options、插件和异常处理示例见 [docs/getting-started.md](docs/getting-started.md)。

## 构建、测试与打包

```powershell
dotnet restore Scorpio.Core.slnx
dotnet build Scorpio.Core.slnx -c Release --no-restore
dotnet test Scorpio.Core.slnx -c Release --no-build

# 按项目打包，输出到 artifacts
dotnet pack src/Scorpio/Scorpio.csproj -c Release -o artifacts --no-build
dotnet pack src/Scorpio.Hosting/Scorpio.Hosting.csproj -c Release -o artifacts --no-build
dotnet pack src/Scorpio.Utilities/Scorpio.Utilities.csproj -c Release -o artifacts --no-build
```

如果使用的工具链不支持 `.slnx`，可将命令中的 `Scorpio.Core.slnx` 替换为 `Scorpio.Core.sln`。

## 文档导航

| 文档 | 说明 |
| --- | --- |
| [docs/getting-started.md](docs/getting-started.md) | 使用方快速接入、模块开发与核心 API 示例 |
| [docs/architecture.md](docs/architecture.md) | 系统架构、启动流程、模块机制和设计边界 |
| [docs/development-guide.md](docs/development-guide.md) | 开发环境、构建测试、分支管理、扩展与发版流程 |
| [docs/features.md](docs/features.md) | 各功能模块的职责、关键类型与机制 |
| [docs/development-standards.md](docs/development-standards.md) | 编码与代码组织规范 |
| [docs/technical-standards.md](docs/technical-standards.md) | 构建体系、依赖版本与技术规范 |

## 许可证

本项目使用 MIT 许可证，详见 [LICENSE](LICENSE)。
