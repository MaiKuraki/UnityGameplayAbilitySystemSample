using CycloneGames.AssetManagement.Runtime;
using CycloneGames.UIFramework.Runtime;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace GASSample.UI
{
    public class UIWindowTitle : UIWindow
    {
        [Inject] private readonly IAssetModule assetModule;
        [Inject] private readonly IUIService uiService;

        [SerializeField] private Button buttonStart;

        protected override void Awake()
        {
            base.Awake();

            buttonStart.OnClickAsObservable().Subscribe(_ => ClickStart().Forget());
        }

        private async UniTask ClickStart()
        {
            // Cover first: the title scene has no geometry, so closing the window early shows skybox.
            await ScreenFade.FadeOutAsync();
            await uiService.CloseAsync(WindowId);
            await SceneFlow.LoadAsync(assetModule, SceneFlow.Gameplay);
        }
    }
}
