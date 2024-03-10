using Zenject;

namespace CycloneGames.GameFramework
{
    public interface IWorld
    {
        public void SetGameMode(GameMode inGameModeRef);
        public GameMode GetGameMode();
        public PlayerController GetPlayerController();      //  TODO: maybe a player index
        public Pawn GetPlayerPawn();                        //  TODO: maybe a player index
    }
    //  TODO: This class is not similar with UWorld in UnrealEngine
    public class World : IWorld, IInitializable
    {
        private GameMode savedGameMode;
        
        public void Initialize()
        {
            
        }

        public void SetGameMode(GameMode inGameModeRef)
        {
            savedGameMode = inGameModeRef;
        }

        public GameMode GetGameMode()
        {
            return savedGameMode;
        }

        public PlayerController GetPlayerController()
        {
            return savedGameMode?.GetPlayerController();
        }

        public Pawn GetPlayerPawn()
        {
            return savedGameMode?.GetPlayerController()?.GetPawn();
        }
    }
}