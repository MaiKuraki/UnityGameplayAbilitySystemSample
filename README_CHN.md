# Unity GameplayAbility Sample (2D Platformer)

<p align="left"><a href="README.md">English</a> | <strong>中文</strong></p>

---

> [!NOTE]
> **当前项目代码存在可维护性瓶颈，且技术实现已明显滞后于行业。为构建可持续的示例项目，我计划在适当的时候重构此项目。新版本将基于 [[UnityStarter]](https://github.com/MaiKuraki/UnityStarter) 重构升级，重点提升代码质量以及对于新 GAS 系统的特性展示。感谢各位开发者长期以来的支持。**

## 关于

本项目原始框架 [UnityStartUp_NoHotUpdate](https://github.com/MaiKuraki/UnityStartUp_NoHotUpdate) 基于 Gameplay Ability System 提供了一个 ARPG 的 Demo。可以作为横板 2D 动作游戏的起始模板，代码可以比较容易的重构为 Production 级别的质量。

~~项目适合熟悉虚幻引擎以及 GameplayAbilitySystem 的开发者使用，初学者上手可能需要花一些时间学习虚幻引擎的 GameplayFramework 以及 GameplayAbilitySystem。~~

## 分支
-   旧版本将保存在 `Legacy(Zenject)` 分支作为存档备份，`main` 分支将作为新的开发分支, 来展示新的 GAS 系统特性。
-   <img src="./README/branches.png" alt="Branch Select" style="width: 50%; height: auto; max-width: 360px;" />

## Unity 版本依赖

本项目依赖 Unity 最低版本为 `Unity 2022.3`， 在 `Unity 2021` 以及更早的版本无法正常运行。

## Preview

![Preview](./README/preview.gif)

## WebGL Demo

[➡️ 点击这里查看 WebGL Demo](https://maikuraki.github.io/2024/10/07/Unity_WebGL_Demo/)

## 文件目录结构

-   `Assets/CycloneGames`
    -   该程序集提供了类似于虚幻引擎 GameplayFramework 的框架设计，包含 `GameInstance`、`World`、`GameMode`、`PlayerController` 和 `PlayerState` 等类型。对于熟悉虚幻引擎的用户，提供了舒适的过渡。
    -   [README](./GameplayAbilitySystemSample/Assets/CycloneGames/README_CHN.md)
-   `Assets/CycloneGames.Service`
    -   该程序集提供了资源管理（Addressable）和显示管理（GraphicsSettings）等服务。
    -   [README](./GameplayAbilitySystemSample/Assets/CycloneGames.Service/README_CHN.md)
-   `Assets/CycloneGames.UIFramework`
    -   该程序集提供了一个简易的分层 UI 框架。
    -   它依赖于 `CycloneGames.Service` 中的 `Addressable` 用于加载 UI Prefab，目前无法消除依赖关系。
    -   [README](./GameplayAbilitySystemSample/Assets/CycloneGames.UIFramework/README_CHN.md)
-   `Assets/ARPGSample`
    -   该文件夹是 Sample 项目的目录，提供了一个启动场景，开始场景和 Gameplay 场景，用作游戏流程测试。

## 启动场景

请于项目内搜索 `Scene_Launch` 场景以启动游戏。
