using Zenject;
using CycloneGames.Service;
using CycloneGames.GameFramework;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CycloneGames.Gameplay
{
    public class GameModeInstaller : MonoInstaller
    {
        [Inject] private IAddressablesService addressablesService;
        [Inject] private IWorld world;

        [SerializeField] private WorldSettingsData worldSettingsData = null;
        [SerializeField] private string KillZVolumePrefabPath =
            "Assets/CycloneGames/Prefabs/GameplayFramework/KillZVolume.prefab";

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<WorldSettings>()
                .AsCached()
                .OnInstantiated(LoadWorldSettings)
                .NonLazy();

            Container.BindInterfacesTo<GameMode>()
                .FromComponentInNewPrefab(worldSettingsData.GameModeClass)
                .WithGameObjectName(worldSettingsData.GameModeClass.name)
                .AsCached()
                .OnInstantiated(InitializeGameMode)
                .NonLazy();
        }

        void LoadWorldSettings(InjectContext context, object InworldSettings)
        {
            WorldSettings worldSettings = InworldSettings as WorldSettings;
            
            if (worldSettings == null)
            {
                if(Application.isPlaying) Debug.LogError($"Invalid Injection (WorldSettings), check your IOC settings");
                return;
            }
            if (!IsWorldSettingsValid())
            {
                Debug.LogError($"Invalid WorldSettingsData, Check your GameModeInstaller");
                return;
            }

            worldSettings.SetGameplayFramework(
                NewGameModeClass: worldSettingsData.GameModeClass,
                NewPlayerControllerClass: worldSettingsData.PlayerControllerClass,
                NewDefaultPawnClass: worldSettingsData.DefaultPawnClass,
                NewPlayerStateClass: worldSettingsData.PlayerStateClass,
                NewSpectatorPawnClass: worldSettingsData.SpectatorPawnClass,
                NewCameraManagerClass: worldSettingsData.CameraManagerClass);
        }

        bool IsWorldSettingsValid() => worldSettingsData != null;

        void InitializeGameMode(InjectContext context, object InGameMode)
        {
            GameMode gameMode = InGameMode as GameMode;
            
            if (world == null)
            {
                if(Application.isPlaying) Debug.LogError("Invalid Injection (IWorld), check your IOC settings");
                return;
            }
            world.SetGameMode(gameMode);
            InitializeGameModeAsync(gameMode).Forget();
        }
        
        async UniTask InitializeGameModeAsync(GameMode inGameMode)
        {
            if (inGameMode == null)
            {
                Debug.LogError($"Invalid GameModeParam");
                return;
            }
            
            if (!IsWorldSettingsValid())
            {
                Debug.LogError($"Invalid WorldSettings, Check your GameModeInstaller");
                return;
            }
            
            await UniTask.WaitUntil(() => addressablesService.IsServiceReady());

            Actor killZVolume = null;
            var killzPrefab = await addressablesService.LoadAssetWithRetentionAsync<GameObject>($"{KillZVolumePrefabPath}");
            
            GameObject killZGO = Container.InstantiatePrefab(killzPrefab);
            if (killZGO != null)
            {
                killZVolume = killZGO.GetComponent<Actor>();
                killZVolume.SetActorPosition(new Vector3(0, worldSettingsData.KillZ, 0));
            }

            await UniTask.WaitUntil(() => killZVolume != null);
            
            inGameMode.LaunchGameMode();
        }
    }
}