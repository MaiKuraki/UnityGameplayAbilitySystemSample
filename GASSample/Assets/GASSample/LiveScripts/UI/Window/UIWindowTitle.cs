using CycloneGames.AssetManagement.Runtime;
using CycloneGames.AssetManagement.Runtime.Integrations.Navigathena;
using CycloneGames.UIFramework.Runtime;
using Cysharp.Threading.Tasks;
using GASSample.APIGateway;
using GASSample.AssetManagement;
using GASSample.Scene;
using MackySoft.Navigathena.SceneManagement;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace GASSample.UI
{
    public class UIWindowTitle : UIWindow
    {
        [Inject] private readonly IAssetModule assetModule;
        [Inject] private readonly ISceneManagementAPIGateway sceneManagementAPIGateway;
        [SerializeField] private Button buttonStart;
        [SerializeField] private AppVersionInfo appVersionInfo;
        private const string EDITOR_VERSION_TEXT = "EDITOR_MODE";
        private const string INVALID_VERSION_TEXT = "INVALID";

        protected override void Awake()
        {
            base.Awake();

            buttonStart.OnClickAsObservable().Subscribe(_ => ClickStart());
        }

        void Start()
        {
            appVersionInfo?.UpdateVersionDisplayEvent?.Invoke(assetModule)
                .AttachExternalCancellation(this.GetCancellationTokenOnDestroy())
                .Forget();
        }

        void ClickStart()
        {
            // CLogger.LogInfo("[UIWindowTitle] ClickStart");            
            var pkg = assetModule.GetPackage(AssetPackageName.DefaultPackage);
            AssetManagementSceneIdentifier sceneGameplay = new AssetManagementSceneIdentifier(pkg, ScenePath.Gameplay, LoadSceneMode.Additive, true);
            GlobalSceneNavigator.Instance.Push(sceneGameplay);
        }
    }
}