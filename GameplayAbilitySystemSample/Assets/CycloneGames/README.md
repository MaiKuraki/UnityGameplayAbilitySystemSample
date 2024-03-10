## NOTE
```
These classes are designed to imitate the naming and some approximate usage of the Unreal Engine. However, it's important to note that they do not completely replicate the workings and framework of the Unreal Engine. As a result, the content provided may not be entirely reliable. It's crucial to remember that these classes solely reflect the personal ideas of the author (Cyclone mai.k@live.com).

这些类主要是为了模仿虚幻引擎的命名和一些大概的使用方式，但并没有完全模仿虚幻引擎的工作方式和框架运行方式，因此内容可能不完全可靠。这些类仅代表作者（旋风冲锋 mai.k@live.com）个人思路的初步尝试。```

```
The 'Prefabs' and 'ScriptableObject' folder must be Addressable
```

```
If you want to use the CameraManager, there must be a 'CinemachineBrain' component add to your working camera.
```

### You can get GameMode / PlayerController From This Way
-   [Inject] IWorld World;
-   GameMode GM = World.GetGameMode();
-   PlayerController PC = GM.GetPlayerController();