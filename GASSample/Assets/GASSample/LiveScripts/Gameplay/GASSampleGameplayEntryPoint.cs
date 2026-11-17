using System;
using System.Threading;
using CycloneGames.GameplayFramework.Runtime;
using CycloneGames.UIFramework.Runtime;
using Cysharp.Threading.Tasks;
using GASSample.UI;
using UnityEngine;
using VContainer.Unity;

namespace GASSample.Gameplay
{
    /// <summary>
    /// Starts the world from <see cref="WorldSettings"/> and opens the HUD.
    /// </summary>
    public class GASSampleGameplayEntryPoint : IAsyncStartable, IDisposable
    {
        private readonly GameInstance gameInstance;
        private readonly WorldSettings worldSettings;
        private readonly IUIService uiService;

        public GASSampleGameplayEntryPoint(
            GameInstance gameInstance,
            WorldSettings worldSettings,
            IUIService uiService)
        {
            this.gameInstance = gameInstance;
            this.worldSettings = worldSettings;
            this.uiService = uiService;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            await gameInstance.StartWorldAsync(
                worldSettings,
                WorldNetMode.Standalone,
                cancellationToken: cancellation);

            await uiService.OpenAsync(UIWindowName.GameplayHUD, cancellationToken: cancellation);

            // Give the camera one frame to follow the new pawn, or the fade-in shows empty sky.
            await UniTask.Yield();
            await ScreenFade.FadeInAsync();
        }

        public void Dispose()
        {
            gameInstance.Dispose();
        }
    }

    /// <summary>Forwards Unity player loop phases to the manually composed <see cref="GameInstance"/>.</summary>
    public class GameInstanceTickDriver : ITickable, IFixedTickable, ILateTickable
    {
        private readonly GameInstance gameInstance;

        public GameInstanceTickDriver(GameInstance gameInstance)
        {
            this.gameInstance = gameInstance;
        }

        public void Tick()
        {
            if (!gameInstance.IsDisposed)
            {
                gameInstance.Tick(ActorTickPhase.Update, Time.deltaTime);
            }
        }

        public void FixedTick()
        {
            if (!gameInstance.IsDisposed)
            {
                gameInstance.Tick(ActorTickPhase.FixedUpdate, Time.fixedDeltaTime);
            }
        }

        public void LateTick()
        {
            if (!gameInstance.IsDisposed)
            {
                gameInstance.Tick(ActorTickPhase.LateUpdate, Time.deltaTime);
            }
        }
    }
}
