using UnityEngine;

namespace CycloneGames.GameFramework
{
    [CreateAssetMenu(menuName = "CycloneGames/General/WorldSettings")]
    [System.Serializable]
    public class WorldSettingsData : ScriptableObject
    {
        [SerializeField] private GameMode gameModeClass;
        [SerializeField] private PlayerController playerControllerClass;
        [SerializeField] private Pawn defaultPawnClass;
        [SerializeField] private PlayerState playerStateClass;
        [SerializeField] private SpectatorPawn spectatorPawnClass;
        [SerializeField] private CameraManager cameraManagerClass;
        [SerializeField] private int killZ;
        
        public GameMode GameModeClass => gameModeClass;
        public PlayerController PlayerControllerClass => playerControllerClass;
        public Pawn DefaultPawnClass => defaultPawnClass;
        public PlayerState PlayerStateClass => playerStateClass;
        public SpectatorPawn SpectatorPawnClass => spectatorPawnClass;
        public CameraManager CameraManagerClass => cameraManagerClass;
        public int KillZ => killZ;
    }
}

