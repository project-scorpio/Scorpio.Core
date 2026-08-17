# 技术规范（Technical Standards）

本文档描述 Scorpio.Core 的技术栈、构建体系、依赖与版本管理、发布元数据等技术规范。

## 1. 技术栈概览

| 维度 | 选型 |
| --- | --- |
| 语言 | C#（`LangVersion = latest`） |
| 目标框架 | `netstandard2.0` + `net5.0`~`net10.0`（详见下） |
| 构建系统 | MSBuild + SDK-style 项目，多级 `Directory.Build.props` |
| 依赖管理 | NuGet 中央包版本管理（Central Package Management, CPM） |
| 单元测试 | xunit 2.4.2 + Shouldly 4.1.0（Moq / NSubstitute 模拟） |
| 覆盖率 | coverlet（msbuild / collector） |
| 动态代理 | Castle.Core / Autofac.DynamicProxy / AspectCore（仅作依赖版本声明，运行时由外部容器接入） |
| 异步上下文 | Nito.AsyncEx.Context（`AsyncHelper.RunSync` 依赖） |

## 2. 目标框架

### 2.1 源码项目（`src/`）

`common.props` 定义：

```xml
<StandardTargetFrameworks>netstandard2.0;net5.0;net6.0;net7.0;net8.0;net9.0;net10.0</StandardTargetFrameworks>
```

`src/Directory.Build.props` 中 `TargetFrameworks = $(StandardTargetFrameworks)`。

### 2.2 测试项目（`test/`）

```xml
<MultiTargetFrameworks>netcoreapp3.1;net5.0;net6.0;net7.0;net8.0;net9.0;net10.0</MultiTargetFrameworks>
```

`test/Directory.Build.props` 中 `TargetFrameworks = $(MultiTargetFrameworks)`。

> 提示：测试使用 `netcoreapp3.1` 而非 `netstandard2.0`（netstandard 不可直接运行测试）。

## 3. 构建体系（MSBuild 层级）

构建属性通过嵌套导入层层叠加，靠近源码的配置优先级更高：

```
versions.props                      # 版本号生成
common.props                         # 作者/许可/产品元数据 + 目标框架列表 + LangVersion
Directory.Build.props（根）          # 导入 versions.props + common.props，定义 SourceRoot
Directory.Build.targets（根）        # 清空 RootNamespace，避免 SDK 默认根命名空间干扰
src/Directory.Build.props            # SrcRoot、GenerateDocumentationFile、EnforceCodeStyleInBuild、IsPackable、SourceLink
test/Directory.Build.props           # TestRoot、IsPackable=false、ServerGarbageCollection、统一注入测试包引用
```

### 3.1 源码项目关键设置

| 属性 | 值 | 说明 |
| --- | --- | --- |
| `GenerateDocumentationFile` | `true` | 生成 XML 文档（配合中文注释） |
| `EnforceCodeStyleInBuild` | `True` | 构建期强制执行代码风格 |
| `IsPackable` | `true` | 参与打包发布 |
| `SourceLinkCreate` | `true` | 生成 SourceLink 调试信息 |
| `SourceLinkOriginUrl` | `https://github.com/project-scorpio/Scorpio` | 源码链接来源 |

### 3.2 测试项目关键设置

| 属性 | 值 | 说明 |
| --- | --- | --- |
| `IsPackable` | `false` | 不参与打包 |
| `GenerateDocumentationFile` | `false` | 不生成文档 |
| `ServerGarbageCollection` | `true` | 启用服务器 GC |
| 公共包引用 | Microsoft.NET.Test.Sdk / xunit / xunit.runner.visualstudio / coverlet.collector | 统一注入到所有测试项目 |
| 公共项目引用 | `Scorpio.TestBase.csproj` | 统一引用测试基础设施 |

## 4. 中央包版本管理（CPM）

`Directory.Packages.props` 设置 `ManagePackageVersionsCentrally = true`，所有包版本集中声明，项目文件中 `PackageReference` 不再写版本号。

### 4.1 版本声明按目标框架区分

`Microsoft.Extensions.*` 系列包版本随目标框架变化：

| 目标框架 | Microsoft.Extensions.* 版本 |
| --- | --- |
| `netstandard2.0` | 3.1.31（额外含 `Microsoft.Bcl.AsyncInterfaces`、`Microsoft.Bcl.HashCode`、`System.Runtime.Loader`） |
| `net5.0` | 5.0.x（额外含 `Microsoft.Bcl.HashCode`、`System.Runtime.Loader`） |
| `net6.0` | 6.0.x |
| `net7.0` | 7.0.0 |
| `net8.0` | 8.0.0 |
| `net9.0` | 9.0.0 |
| `net10.0` | 10.0.0 |

### 4.2 条件包版本

| 包 | netstandard2.0 | 其它框架 |
| --- | --- | --- |
| `EasyNetQ.DI.Microsoft` | 6.5.2 | 7.3.2 |
| `AutoMapper` | 10.1.1 | 12.0.0 |

### 4.3 主要第三方包（不随框架变化）

| 包 | 版本 |
| --- | --- |
| Nito.AsyncEx.Context | 5.1.2 |
| AspectCore.Extensions.DependencyInjection | 2.2.0 |
| Autofac | 6.5.0 |
| Autofac.Extras.DynamicProxy | 6.0.1 |
| Castle.Core | 5.0.0 |
| Castle.Core.AsyncInterceptor | 2.1.0 |
| Newtonsoft.Json | 13.0.1 |
| System.Linq.Dynamic.Core | 1.2.18 |
| Quartz.Extensions.DependencyInjection | 3.4.0 |
| Quartz.Plugins.TimeZoneConverter | 3.4.0 |

### 4.4 测试包（`test/Directory.Packages.props`）

| 包 | 版本 |
| --- | --- |
| Microsoft.NET.Test.Sdk | 17.4.0 |
| xunit | 2.4.2 |
| xunit.runner.visualstudio | 2.4.5 |
| coverlet.msbuild | 3.1.2 |
| coverlet.collector | 3.2.0 |
| Shouldly | 4.1.0 |
| Moq | 4.18.2 |
| NSubstitute | 4.4.0 |
| Divergic.Logging.Xunit | 4.0.0 |

> 添加新包时：只在 `Directory.Packages.props` 中声明版本，项目文件内仅写 `<PackageReference Include="..." />`。

## 5. 版本号管理（`versions.props`）

- 主/次/修订版本：`VersionMajor=0`、`VersionMinor=1`、`VersionPatch=2` → `VersionPrefix = 0.1.2`。
- 构建号 `VersionBuild`：**自 2022-09-21 起的天数**（MSBuild 内联 `DateTime` 运算自动计算）。
- `AssemblyVersion = $(VersionPrefix).$(VersionBuild)`，形如 `0.1.2.xxxx`。
- `VersionSuffix` 为空，无预发布后缀。

> 发版时修改 `VersionMajor/Minor/Patch`；构建号无需手工维护。

## 6. 代码分析与风格强制

- `EnforceCodeStyleInBuild = True`：构建期强制执行 `.editorconfig` 风格规则，风格问题将导致构建失败。
- 已接入 Sonar 规则（`S` 编号）分析；局部抑制使用 `[SuppressMessage]` 或 `#pragma warning disable`。
- XML 文档 `cref` 无法解析警告 `CS1574` 就地抑制。
- `Scorpio.TestBase.csproj` 设置 `SonarQubeTestProject=false`（标记为测试项目）。

## 7. 发布与打包元数据（`common.props`）

| 属性 | 值 |
| --- | --- |
| Authors | Zidan.Wang |
| Company | Pluto Studio |
| Product | Scorpio |
| Copyright | Copyright © Pluto Studio 2015-2022 |
| PackageLicenseExpression | MIT |
| PackageProjectUrl | https://project-scorpio.github.io/Scorpio.Core/ |
| RepositoryUrl | https://github.com/project-scorpio/Scorpio.Core |
| RepositoryType | git |

## 8. NuGet 源配置（`nuget.config`）

仅使用官方源（`<clear/>` 后仅保留 `https://api.nuget.org/v3/index.json`），不继承全局源。

## 9. 解决方案结构

- `Scorpio.Core.sln` / `Scorpio.Core.slnx`：传统与新版 XML 格式解决方案文件。
- `Scorpio.Core.lutconfig`：LUT（Lightweight Unit Test）配置——启用并行构建、并行测试，单用例超时 180 秒。
