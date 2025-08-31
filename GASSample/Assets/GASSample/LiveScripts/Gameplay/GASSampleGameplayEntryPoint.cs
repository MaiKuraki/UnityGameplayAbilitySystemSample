using System;
using System.Threading;
using CycloneGames.GameplayFramework;
using CycloneGames.UIFramework;
using Cysharp.Threading.Tasks;
using GASSample.UI;
using VContainer.Unity;

namespace GASSample.Gameplay
{
    public class GASSampleGameplayEntryPoint : IAsyncStartable, IDisposable
    {
        private readonly IWorld world;
        private readonly IGameMode gameMode;
        private readonly IUIService uIService;

        public GASSampleGameplayEntryPoint(IWorld world, IGameMode gameMode, IUIService uIService)
        {
            this.world = world;
            this.gameMode = gameMode;
            this.uIService = uIService;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            await uIService.OpenUIAsync(UIWindowName.GameplayHUD);
            await gameMode.LaunchGameModeAsync(cancellation);
            world.SetGameMode(gameMode);
        }

        public void Dispose()
        {
            world.SetGameMode(null);
        }
    }
}
