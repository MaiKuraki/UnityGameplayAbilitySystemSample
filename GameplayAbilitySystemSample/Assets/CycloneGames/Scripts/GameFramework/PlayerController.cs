using UnityEngine;
using Zenject;

namespace CycloneGames.GameFramework
{
    public class PlayerController : Controller
    {
        [Inject] private DiContainer DiContainer;
        [Inject] private IWorldSettings WroldSettings;

        private SpectatorPawn spectatorPawn;
        public SpectatorPawn GetSpectatorPawn() => spectatorPawn;
        private CameraManager cameraManager;
        public CameraManager GetCameraManager() => cameraManager;
        public SpectatorPawn SpawnSpectatorPawn()
        {
            //  TODO:
            GameObject spectatorGO = DiContainer.InstantiatePrefab(WroldSettings.SpectatorPawnClass);
            spectatorPawn = spectatorGO ? spectatorGO.GetComponent<SpectatorPawn>() : null;
            DiContainer.BindInstance(spectatorPawn).AsCached();
            
            return spectatorPawn;
        }

        void SpawnCameraManager()
        {
            if (WroldSettings?.CameraManagerClass == null)
            {
                Debug.LogError($"Invalid CameraManager Class");
            }
            GameObject cameraManagerGO = DiContainer.InstantiatePrefab(WroldSettings.CameraManagerClass);
            cameraManager = cameraManagerGO.GetComponent<CameraManager>();
            DiContainer.BindInstance(cameraManager).AsCached();
            if (cameraManager)
            {
                cameraManager.SetOwner(this);
                cameraManager.InitializeFor(this);
            }
        }

        protected override void Awake()
        {
            base.Awake();

            InitPlayerState();
            SpawnCameraManager();
            SpawnSpectatorPawn();
        }
    }
}