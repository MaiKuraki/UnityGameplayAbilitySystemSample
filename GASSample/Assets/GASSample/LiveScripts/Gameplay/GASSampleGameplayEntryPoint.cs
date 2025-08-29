using CycloneGames.GameplayFramework;
using VContainer;
using VContainer.Unity;

namespace GASSample.Gameplay
{
    public class GASSampleGameplayEntryPoint : IStartable
    {
        [Inject] private IGameMode gameMode;

        public void Start()
        {
            gameMode.LaunchGameMode();
        }
    }
}