using System;
using System.Threading;
using CycloneGames.GameplayFramework;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace GASSample.Gameplay
{
    public class GASSampleGameplayEntryPoint : IAsyncStartable, IDisposable
    {
        private readonly IWorld world;
        private readonly IGameMode gameMode;

        public GASSampleGameplayEntryPoint(IWorld world, IGameMode gameMode)
        {
            this.world = world;
            this.gameMode = gameMode;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            await gameMode.LaunchGameModeAsync(cancellation);
            world.SetGameMode(gameMode);
        }

        public void Dispose()
        {
            world.SetGameMode(null);
        }
    }
}
