using System.Threading;
using CycloneGames.UIFramework.Runtime;
using Cysharp.Threading.Tasks;
using GASSample.UI;
using VContainer.Unity;

namespace GASSample.Scene
{
    /// <summary>Opens the title window.</summary>
    public class TitleEntryPoint : IAsyncStartable
    {
        private readonly IUIService uiService;

        public TitleEntryPoint(IUIService uiService)
        {
            this.uiService = uiService;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            await uiService.OpenAsync(UIWindowName.Title, cancellationToken: cancellation);
            await ScreenFade.FadeInAsync();
        }
    }
}
