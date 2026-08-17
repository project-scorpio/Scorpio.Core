# Scorpio.Core 开发指南

本文档面向参与 Scorpio.Core 框架开发的维护者，说明环境准备、常用命令、项目扩展方式、测试和发版流程。

## 1. 开发环境

推荐环境：

- Windows / macOS / Linux。
- .NET SDK 10.0 或更高版本。
- 安装测试目标框架对应的运行时：`netcoreapp3.1`、`net5.0`、`net6.0`、`net7.0`、`net8.0`、`net9.0`、`net10.0`。
- 官方 NuGet 源可访问：`https://api.nuget.org/v3/index.json`。

确认环境：

```powershell
dotnet --info
dotnet --list-sdks
dotnet --list-runtimes
```

## 2. 仓库结构

```text
Scorpio.Core/
├── src/
│   ├── Scorpio/
│   ├── Scorpio.Hosting/
│   └── Scorpio.Utilities/
├── test/
│   ├── Scorpio.TestBase/
│   ├── Scorpio.Tests/
│   ├── Scorpio.Hosting.Tests/
│   └── Scorpio.Utilities.Tests/
├── docs/
├── common.props
├── versions.props
├── Directory.Build.props
├── Directory.Build.targets
├── Directory.Packages.props
├── src/Directory.Build.props
├── test/Directory.Build.props
├── test/Directory.Packages.props
├── nuget.config
├── Scorpio.Core.sln
└── Scorpio.Core.slnx
```

职责划分详见 [architecture.md](./architecture.md) 与 [development-standards.md](./development-standards.md)。

## 3. 日常命令

### 3.1 还原

```powershell
dotnet restore Scorpio.Core.slnx
```

旧工具链可使用：

```powershell
dotnet restore Scorpio.Core.sln
```

### 3.2 构建

```powershell
dotnet build Scorpio.Core.slnx -c Debug
dotnet build Scorpio.Core.slnx -c Release --no-restore
```

只构建核心库：

```powershell
dotnet build src/Scorpio/Scorpio.csproj -c Release
```

### 3.3 测试

```powershell
dotnet test Scorpio.Core.slnx -c Release --no-build
```

只测试指定项目：

```powershell
dotnet test test/Scorpio.Tests/Scorpio.Tests.csproj -c Release
```

只测试指定目标框架：

```powershell
dotnet test test/Scorpio.Tests/Scorpio.Tests.csproj -c Release -f net10.0
```

### 3.4 覆盖率

```powershell
dotnet test test/Scorpio.Tests/Scorpio.Tests.csproj `
  -c Release `
  -p:CollectCoverage=true `
  -p:CoverletOutputFormat=cobertura
```

### 3.5 打包

```powershell
dotnet pack src/Scorpio/Scorpio.csproj -c Release -o artifacts --no-build
dotnet pack src/Scorpio.Hosting/Scorpio.Hosting.csproj -c Release -o artifacts --no-build
dotnet pack src/Scorpio.Utilities/Scorpio.Utilities.csproj -c Release -o artifacts --no-build
```

## 4. MSBuild 配置体系

构建配置按目录层级合并：

```text
versions.props
common.props
Directory.Build.props（根）
Directory.Build.targets（根）
src/Directory.Build.props 或 test/Directory.Build.props
```

关键约定：

- `common.props` 定义元数据、`LangVersion=latest` 和目标框架列表。
- `src/Directory.Build.props` 启用 XML 文档、代码风格构建期检查、打包和 SourceLink。
- `test/Directory.Build.props` 统一注入测试 SDK 与 `Scorpio.TestBase` 引用。
- `Directory.Packages.props` 使用 Central Package Management；项目文件中的 `PackageReference` 不写版本。

修改依赖版本时，应修改 `Directory.Packages.props` 或 `test/Directory.Packages.props`，不要写进单个 `.csproj`。

## 5. 新增源码项目

1. 在 `src/` 下创建 `<Project>/<Project>.csproj`。
2. 使用 SDK-style 项目，继承父级 `Directory.Build.props` 会自动获得目标框架、文档生成和打包设置。
3. 在 `.csproj` 中声明项目引用和包引用，不写包版本。
4. 把项目加入 `Scorpio.Core.sln` 和 `Scorpio.Core.slnx`。

例如：

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <AssemblyTitle>Scorpio example library</AssemblyTitle>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\Scorpio\Scorpio.csproj" />
  </ItemGroup>
</Project>
```

## 6. 新增测试项目

1. 在 `test/` 下创建 `<Project>.Tests/`。
2. 测试项目会自动引用 `Scorpio.TestBase`，并注入 xunit、测试 SDK 和 coverlet。
3. 若需要额外包，在 `test/Directory.Packages.props` 声明版本，在项目文件中引用。
4. 测试类命名 `<Type>_Tests`，方法命名 `Should_<行为>_<场景>`。

## 7. 新增模块或功能

### 7.1 核心框架类型

- 模块放在 `src/Scorpio/Scorpio/<Area>/`。
- 命名空间使用 `Scorpio.<Area>`。
- 对 BCL 的扩展放在 `src/Scorpio.Utilities/System/<Area>/`，命名空间使用 `System.<Area>`。
- 对 `Microsoft.Extensions.*` 的扩展放在 `src/Scorpio/Microsoft/Extensions/<Area>/`，命名空间使用 `Microsoft.Extensions.<Area>`。

### 7.2 模块示例

```csharp
using Scorpio.DependencyInjection;
using Scorpio.Modularity;

namespace Scorpio.Auditing
{
    public class AuditingModule : ScorpioModule
    {
        public override void PreConfigureServices(ConfigureServicesContext context)
        {
            // 前置配置
        }

        public override void ConfigureServices(ConfigureServicesContext context)
        {
            // 主要服务注册
        }

        public override void Initialize(ApplicationInitializationContext context)
        {
            // 初始化逻辑
        }
    }
}
```

### 7.3 服务与约定

优先使用标记接口注册服务：

```csharp
using Scorpio.DependencyInjection;

public class AuditingStore : IAuditingStore, ITransientDependency
{
}
```

需要精确暴露时使用 `[ExposeServices]`。

## 8. 测试策略

- 单元测试优先覆盖公开 API 和边界条件。
- 使用 xunit + Shouldly；需要模拟时优先 Moq，必要时 NSubstitute。
- 集成测试可继承 `IntegratedTest<TStartupModule>` 或 `TestBaseWithServiceProvider`。
- 模块加载、生命周期、DI 约定、插件和异常处理等框架核心机制应有回归测试。
- 每个改动至少运行相关项目测试，建议再运行整个解决方案测试。

## 9. 代码风格与质量门禁

- 必须遵循 [development-standards.md](./development-standards.md)。
- 所有公开成员写中文 XML 文档注释。
- 使用 block namespace，不使用 file-scoped namespace。
- 不启用可空引用类型；不使用 `record` 定义公开类型。
- 保持 `netstandard2.0` 兼容，反射等场景使用兼容 API。
- 构建期启用 `EnforceCodeStyleInBuild`，风格问题会导致构建失败。

提交前建议执行：

```powershell
dotnet build Scorpio.Core.slnx -c Release
dotnet test Scorpio.Core.slnx -c Release --no-build
```

## 10. Git 分支与版本发布规则

### 10.1 分支策略

- `main` 是主分支，**不得直接在 `main` 上提交或推送代码**。
- 所有新功能必须在 `feature/*` 分支开发，完成后通过 Pull Request 合并回 `main`。
- 所有 Bug 修复必须在 `fix/*` 分支开发，完成后通过 Pull Request 合并回 `main`。
- 分支命名使用小写短横线或斜杠形式，例如：
  - `feature/module-initialization`
  - `feature/options-preconfigure`
  - `fix/module-shutdown-order`
- 分支应基于最新的 `main` 创建；合并后及时删除已完成的功能或修复分支。

### 10.2 分支操作示例

```powershell
# 功能开发
git checkout main
git pull
git checkout -b feature/add-auditing-module
git push -u origin feature/add-auditing-module

# Bug 修复
git checkout main
git pull
git checkout -b fix/module-shutdown-order
git push -u origin fix/module-shutdown-order
```

### 10.3 Pull Request 要求

- 目标分支为 `main`。
- PR 标题能清晰描述改动，建议使用 `feat:` / `fix:` / `docs:` / `refactor:` / `test:` 前缀。
- 提交前必须通过构建和测试：

```powershell
dotnet build Scorpio.Core.slnx -c Release
dotnet test Scorpio.Core.slnx -c Release --no-build
```

- 涉及公开 API、模块行为或使用方式的改动，需同步更新 `docs/` 下相关文档。
- 未获得必要评审前，不直接向 `main` 推送。

### 10.4 GitHub Tag + Release 发布

版本发布以 GitHub 的 **Tag + Release** 作为正式版本入口：

1. 在 `feature/*` 或 `fix/*` 分支中完成代码和文档改动。
2. 在对应 PR 中更新 `versions.props` 的 `VersionMajor`、`VersionMinor`、`VersionPatch`；`VersionBuild` 由 MSBuild 自动生成，无需手工维护。
3. PR 通过评审并合并到 `main`。
4. 基于合并后的 `main` 提交创建带注释的版本标签，标签名统一使用 `vX.Y.Z`：

```powershell
git checkout main
git pull
git tag -a v0.1.2 -m "Release v0.1.2"
git push origin v0.1.2
```

5. 在 GitHub 上基于该 Tag 创建 Release，填写版本号与变更说明，可生成 Release Notes。
6. 以 Tag 对应的源码为基准打包并发布 NuGet 包。

> 发布标签必须指向 `main` 上的合并提交，不能指向尚未合并的临时开发分支。

## 11. 发版与 NuGet 打包流程

1. 修改 `versions.props` 中的 `VersionMajor`、`VersionMinor`、`VersionPatch`；`VersionBuild` 无需手工维护。
2. 更新根 `README.md` 或 `docs/` 中受影响的文档。
3. 跑完整构建与测试。
4. 打包三个源码项目并检查生成的 `.nupkg`。
5. 使用 `dotnet nuget push` 发布到目标源。

示例：

```powershell
dotnet pack src/Scorpio/Scorpio.csproj -c Release -o artifacts --no-build
dotnet nuget push artifacts/Scorpio.0.1.2.*.nupkg --source <目标源> --api-key <key>
```

## 12. 常见问题

### 12.1 构建失败：找不到某个 TargetFramework

安装对应版本的 .NET SDK 或运行时；如果只需开发部分目标框架，可在命令行通过 `-f` 单独构建，但发版前仍需完整多目标构建。

### 12.2 还原失败或找不到包

确认 `nuget.config` 中的官方源可访问。项目使用中央包版本管理，新增包必须在 `Directory.Packages.props` 中声明版本。

### 12.3 构建期代码风格报错

按报错提示修正风格；确认没有引入与现有约定不一致的语法或命名。

### 12.4 XML 文档 CS1574

多目标框架下部分 `cref` 可能无法解析。可参照现有代码使用 `#pragma warning disable CS1574` 局部抑制，并保留中文注释。

### 12.5 测试运行缓慢

仓库 `Scorpio.Core.lutconfig` 已启用并行构建和并行测试，单用例超时 180 秒。日常只运行受影响的测试项目，提交前再跑全量。

## 13. 给后续 AI/协作开发的约定

- 先读 [architecture.md](./architecture.md)，理解启动链路与模块边界。
- 修改公开 API 时同步更新 `features.md`、`getting-started.md` 或本指南。
- 新增类型必须遵循命名空间与目录映射、中文 XML 注释和测试命名规范。
- 不擅自修改 `versions.props`、`Directory.Packages.props` 或全局构建配置，除非任务明确包含发版或依赖升级。
- 优先保持向后兼容与 `netstandard2.0` 兼容性。
- 遵守分支规则：不在 `main` 直接开发；功能改动使用 `feature/*`，Bug 修复使用 `fix/*`；版本通过 GitHub Tag + Release 发布。
