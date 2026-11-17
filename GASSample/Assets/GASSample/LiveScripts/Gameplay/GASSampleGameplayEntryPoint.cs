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
    /// Starts the world for the gameplay scene and opens the HUD.
    /// The previous framework let the application assign a World and a GameMode by hand; the
    /// current one owns both: <see cref="GameInstance"/> creates the sealed World from a
    /// <see cref="WorldSettings"/> asset, and the World instantiates the GameMode prefab.
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

            // Let the camera settle on the freshly spawned pawn before the black curtain lifts,
            // otherwise the first visible frames are empty sky.
            await UniTask.Yield();
            await ScreenFade.FadeInAsync();
        }

        public void Dispose()
        {
            gameInstance.Dispose();
        }
    }

    /// <summary>
    /// Unity PlayerLoop bridge for the manually composed <see cref="GameInstance"/>.
    /// A <see cref="GameplayWorldHost"/> would own these drivers; the sample composes the instance
    /// through VContainer instead, so the phases are forwarded here.
    /// </summary>
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
