using System.Threading;
using CycloneGames.UIFramework.Runtime;
using Cysharp.Threading.Tasks;
using GASSample.UI;
using UnityEngine;
using VContainer.Unity;

namespace GASSample.Gameplay
{
    /// <summary>
    /// Spawns the player for the gameplay scene and opens the HUD.
    /// </summary>
    public class GASSampleGameplayEntryPoint : IAsyncStartable
    {
        private readonly PlayerSpawner playerSpawner;
        private readonly IUIService uiService;

        public GASSampleGameplayEntryPoint(PlayerSpawner playerSpawner, IUIService uiService)
        {
            this.playerSpawner = playerSpawner;
            this.uiService = uiService;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            playerSpawner.Spawn();

            await uiService.OpenAsync(UIWindowName.GameplayHUD, cancellationToken: cancellation);

            // Give the camera one frame to follow the new character, or the fade-in shows empty sky.
            await UniTask.Yield();
            await ScreenFade.FadeInAsync();
        }
    }
}
