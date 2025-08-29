using CycloneGames.Factory.Runtime;
using CycloneGames.GameplayFramework;
using VContainer;

namespace GASSample.Gameplay
{
    public class GASSampleGameMode : GameMode
    {
        private const string DEBUG_FLAG = "[GASSampleGameMode]";
        [Inject] private IUnityObjectSpawner unityObjectSpawner;

        [Inject]
        public override void Initialize(IUnityObjectSpawner objectSpawner, IWorldSettings worldSettings)
        {
            base.Initialize(objectSpawner, worldSettings);

        }

        public override void LaunchGameMode()
        {
            base.LaunchGameMode();


        }
    }
}