using CycloneGames.AssetManagement.Runtime;
using CycloneGames.AssetManagement.Runtime.Integrations.Navigathena;
using CycloneGames.UIFramework.Runtime;
using GASSample.AssetManagement;
using GASSample.Scene;
using MackySoft.Navigathena.SceneManagement;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace GASSample.UI
{
    public class UIWindowTitle : UIWindow
    {
        [Inject] private readonly IAssetModule assetModule;
        [SerializeField] private Button buttonStart;

        protected override void Awake()
        {
            base.Awake();

            buttonStart.OnClickAsObservable().Subscribe(_ => ClickStart());
        }

        private void ClickStart()
        {
            var pkg = assetModule.GetPackage(AssetPackageName.DefaultPackage);
            var sceneLoader = (IAssetSceneLoader)pkg;
            var sceneGameplay = new AssetManagementSceneIdentifier(sceneLoader, ScenePath.Gameplay, LoadSceneMode.Additive, true);
            GlobalSceneNavigator.Instance.Push(sceneGameplay);
        }
    }
}
