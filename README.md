# Unity GameplayAbility Sample (2D Platformer)

<p align="left"><strong>English</strong> | <a href="README_CHN.md">中文</a></p>

---

> [!NOTE]
> **Just a heads-up! The current codebase is hitting some maintenance roadblocks and the tech is a bit outdated. To keep this project healthy and sustainable, I'm planning a major refactor soon. The new version will be refactor with [[UnityStarter]](https://github.com/MaiKuraki/UnityStarter), with a big focus on improving code quality and better showcasing the new GAS features. Thanks for all your support!**

## Overview

This project, originally structured after [UnityStartUp_NoHotUpdate](https://github.com/MaiKuraki/UnityStartUp_NoHotUpdate), is based on the Gameplay Ability System and provides a demo of an ARPG. It can serve as a starting template for side-scrolling 2D action games, and the code can be refactored relatively easily to production-level quality.

~~This project is suitable for developers familiar with the Unreal Engine and the GameplayAbilitySystem. It may take some time for beginners to learn Unreal Engine's GameplayFramework and GameplayAbilitySystem.~~

## Branches
-   The old version will be archived in the `Legacy(Zenject)` branch, and the `main` branch will serve as the new development branch with new GAS features.
-   <img src="./README/branches.png" alt="Branch Select" style="width: 50%; height: auto; max-width: 360px;" />

## Unity Version Dependency

The minimum Unity version required for this project is `Unity 2022.3`. It will not run correctly on `Unity 2021` or earlier versions.

## Preview

![Preview](./README/preview.gif)

## WebGL Demo

[➡️ Click here for the WebGL Demo](https://maikuraki.github.io/2024/10/07/Unity_WebGL_Demo/)

## File Directory Structure

-   `Assets/CycloneGames`
    -   This assembly provides a framework design akin to Unreal Engine's GameplayFramework, featuring types such as `GameInstance`, `World`, `GameMode`, `PlayerController`, and `PlayerState`. It offers a comfortable transition for users familiar with Unreal Engine.
    -   [README](./GameplayAbilitySystemSample/Assets/CycloneGames/README.md)
-   `Assets/CycloneGames.Service`
    -   This assembly delivers services such as resource management (Addressable) and display management (GraphicsSettings).
    -   [README](./GameplayAbilitySystemSample/Assets/CycloneGames.Service/README.md)
-   `Assets/CycloneGames.UIFramework`
    -   This assembly provides a simple layered UI framework.
    -   It depends on `Addressable` from `CycloneGames.Service` for loading UI Prefabs, and currently, the dependency cannot be eliminated.
    -   [README](./GameplayAbilitySystemSample/Assets/CycloneGames.UIFramework/README.md)
-   `Assets/ARPGSample`
    -   This folder contains the directories of the Sample project, providing a launch scene, a starting scene, and gameplay scenes for testing the game flow.

## Launch Scene

Please search for the `Scene_Launch` scene within the project to start the game.
