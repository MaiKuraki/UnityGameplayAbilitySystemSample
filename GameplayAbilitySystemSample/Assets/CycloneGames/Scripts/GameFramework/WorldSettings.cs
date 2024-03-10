namespace CycloneGames.GameFramework
{
    public interface IWorldSettings
    {
        GameMode GameModeClass { get; }
        void SetGameModeClass(GameMode NewGameModeClass);
        PlayerController PlayerControllerClass { get; }
        void SetPlayerControllerClass(PlayerController NewPlayerControllerClass);
        Pawn PawnClass { get; }
        void SetPawnClass(Pawn NewPawnClass);
        PlayerState PlayerStateClass { get; }
        void SetPlayerStateClass(PlayerState NewPlayerStateClass);
        SpectatorPawn SpectatorPawnClass { get; }
        void SetSpectatorPawnClass(SpectatorPawn NewSpectatorPawnClass);
        CameraManager CameraManagerClass { get; }
        void SetCameraManagerClass(CameraManager NewCameraManagerClass);
        void SetGameplayFramework(GameMode NewGameModeClass, PlayerController NewPlayerControllerClass,
            Pawn NewDefaultPawnClass, PlayerState NewPlayerStateClass, SpectatorPawn NewSpectatorPawnClass, CameraManager NewCameraManagerClass);
        void ClearAll();
    }
    public class WorldSettings : IWorldSettings
    {
        private GameMode gameModeClass;
        public GameMode GameModeClass => gameModeClass;
        public void SetGameModeClass(GameMode NewGameModeClass)
        {
            gameModeClass = NewGameModeClass;
        }
        
        private PlayerController playerControllerClass;
        public PlayerController PlayerControllerClass => playerControllerClass;
        public void SetPlayerControllerClass(PlayerController NewPlayerControllerClass)
        {
            throw new System.NotImplementedException();
        }
        
        private Pawn pawnClass;
        public Pawn PawnClass => pawnClass;
        public void SetPawnClass(Pawn NewPawnClass)
        {
            pawnClass = NewPawnClass;
        }

        private PlayerState playerStateClass;
        public PlayerState PlayerStateClass => playerStateClass;
        public void SetPlayerStateClass(PlayerState NewPlayerStateClass)
        {
            playerStateClass = NewPlayerStateClass;
        }
        
        private SpectatorPawn spectatorPawnClass;
        public SpectatorPawn SpectatorPawnClass => spectatorPawnClass;
        public void SetSpectatorPawnClass(SpectatorPawn NewSpectatorPawnClass)
        {
            spectatorPawnClass = NewSpectatorPawnClass;
        }

        private CameraManager cameraManagerClass;
        public CameraManager CameraManagerClass => cameraManagerClass;

        public void SetCameraManagerClass(CameraManager NewCameraManagerClass)
        {
            cameraManagerClass = NewCameraManagerClass;
        }

        public void SetGameplayFramework(GameMode NewGameModeClass, PlayerController NewPlayerControllerClass, Pawn NewDefaultPawnClass,
            PlayerState NewPlayerStateClass, SpectatorPawn NewSpectatorPawnClass, CameraManager NewCameraManagerClass)
        {
            //  TODO: Fill the Default GameModeClass
            gameModeClass = NewGameModeClass != null ? NewGameModeClass : null;
            playerControllerClass = NewPlayerControllerClass != null ? NewPlayerControllerClass : null;
            pawnClass = NewDefaultPawnClass != null ? NewDefaultPawnClass : null;
            playerStateClass = NewPlayerStateClass != null ? NewPlayerStateClass : null;
            spectatorPawnClass = NewSpectatorPawnClass != null ? NewSpectatorPawnClass : null;
            cameraManagerClass = NewCameraManagerClass != null ? NewCameraManagerClass : null;
        }

        public void ClearAll()
        {
            gameModeClass = null;
            playerControllerClass = null;
            pawnClass = null;
            playerStateClass = null;
            spectatorPawnClass = null;
            cameraManagerClass = null;
        }
    }
}