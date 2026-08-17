# Scorpio.Core 项目文档

本目录是 **Scorpio.Core** 框架的维护与开发文档，供后续开发人员在日常开发、代码评审与功能扩展时参考。

## 项目简介

Scorpio.Core 是一个**模块化、跨平台的 .NET 框架**（类 ABP 风格）。它以 `IScorpioModule` 为核心契约构建模块系统，并提供 AOP、模块化、插件化、依赖注入、约定式注册、配置选项、本地化、异常处理、时钟/计时器以及大量常用扩展方法等基础设施能力。

框架由三个源码项目构成：

| 项目 | 程序集标题 | 主命名空间 | 职责 |
| --- | --- | --- | --- |
| `src/Scorpio` | Scorpio core library | `Scorpio` | 核心库：引导程序、模块系统、DI、AOP、异常处理、配置等 |
| `src/Scorpio.Hosting` | Scorpio data library | `Scorpio.Hosting` / `Microsoft.Extensions.Hosting` | 宿主库：接入 .NET Generic Host |
| `src/Scorpio.Utilities` | Scorpio utility library | `Scorpio` / `System.*` | 工具库：参数校验、集合、表达式、反射等扩展 |

## 文档导航

| 文档 | 说明 |
| --- | --- |
| [快速上手 / 使用指南](./getting-started.md) | 从零接入框架、模块开发、DI、Generic Host、Options、插件与异常处理示例 |
| [架构说明](./architecture.md) | 程序集分层、启动流程、模块机制、核心扩展点与设计边界 |
| [开发指南](./development-guide.md) | 开发环境、构建测试、Git 分支规则、新增项目/模块、测试与 GitHub Tag/Release 发版流程 |
| [开发规范](./development-standards.md) | 项目结构、命名、代码组织、注释、异常、依赖注入、模块开发与测试等约定 |
| [技术规范](./technical-standards.md) | 目标框架、构建体系、中央包版本管理、版本号、代码风格强制与发布元数据 |
| [功能说明](./features.md) | 各功能模块的职责、关键类型与核心机制说明 |

## 快速上手（新增一个模块）

1. 在 `src/Scorpio/Scorpio/<Area>` 下新建文件夹与 `*Module.cs`，继承 `ScorpioModule` 并覆写生命周期方法。
2. 通过 `[DependsOn]` 声明依赖模块；服务类实现 `ITransientDependency` / `IScopedDependency` / `ISingletonDependency` 即可被约定注册。
3. 在 `test/Scorpio.Tests/<Area>` 下新增 `<Type>_Tests.cs`，使用 xunit + Shouldly 编写测试。
4. 保持中文 XML 文档注释完整，遵循 [开发规范](./development-standards.md)。

如果是要**使用**这个框架开发应用，请先阅读 [快速上手 / 使用指南](./getting-started.md)；如果要**参与框架维护**，请先阅读 [开发指南](./development-guide.md) 和 [架构说明](./architecture.md)。
