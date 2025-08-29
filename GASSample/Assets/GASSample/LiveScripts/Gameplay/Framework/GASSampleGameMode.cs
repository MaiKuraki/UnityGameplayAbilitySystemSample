using System.Threading;
using CycloneGames.Factory.Runtime;
using CycloneGames.GameplayFramework;
using Cysharp.Threading.Tasks;
using VContainer;

namespace GASSample.Gameplay
{
    public class GASSampleGameMode : GameMode
    {
        private const string DEBUG_FLAG = "[GASSampleGameMode]";

        [Inject]
        public override void Initialize(IUnityObjectSpawner objectSpawner, IWorldSettings worldSettings)
        {
            base.Initialize(objectSpawner, worldSettings);

        }

        public override UniTask LaunchGameModeAsync(CancellationToken cancellationToken = default)
        {
            return base.LaunchGameModeAsync(cancellationToken);
        }
    }
}
