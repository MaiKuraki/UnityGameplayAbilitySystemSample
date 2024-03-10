Unity Gameplay Ability Sample (2D Platformer)
<p align="center">
    <br> English | <a href="README_CHN.md">中文</a>
</p>

## Overview
This project, originally structured after [UnityStartUp_NoHotUpdate](https://github.com/MaiKuraki/UnityStartUp_NoHotUpdate), is based on the Gameplay Ability System and provides a demo of an ARPG. It can serve as a starting template for side-scrolling 2D action games, and the code can be refactored relatively easily to production-level quality.

This project is suitable for developers familiar with the Unreal Engine and the GameplayAbilitySystem. It may take some time for beginners to learn Unreal Engine's GameplayFramework and GameplayAbilitySystem.
## Unity Version Dependency
The minimum Unity version required for this project is Unity 2022.3. It will not run correctly on Unity 2021 or earlier versions.
## Preview
![Preview](./README/preview.gif)
## File Directory Structure
-   Assets/CycloneGames
    -   This assembly provides a framework design akin to Unreal Engine's GameplayFramework, featuring types such as GameInstance, World, GameMode, PlayerController, and PlayerState. It offers a comfortable transition for users familiar with Unreal Engine.
    -   [README](./GameplayAbilitySystemSample/Assets/CycloneGames/README.md)
-   Assets/CycloneGames.Service
    -   This assembly delivers services such as resource management (Addressable) and display management (GraphicsSettings).
    -   [README](./GameplayAbilitySystemSample/Assets/CycloneGames.Service/README.md)
-   Assets/CycloneGames.UIFramework
    -   This assembly provides a simple layered UI framework.
    -   It depends on Addressable from CycloneGames.Service for loading UI Prefabs, and currently, the dependency cannot be eliminated.
    -   [README](./GameplayAbilitySystemSample/Assets/CycloneGames.UIFramework/README.md)
-   Assets/ARPGSample
    -   This folder contains the directories of the Sample project, providing a launch scene, a starting scene, and gameplay scenes for testing the game flow.
## Launch Scene
Please search within the project for Scene_Launch.