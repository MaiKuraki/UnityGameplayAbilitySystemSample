using CycloneGames.AssetManagement.Runtime;
using CycloneGames.AssetManagement.Runtime.Integrations.Navigathena;
using CycloneGames.UIFramework.Runtime;
using Cysharp.Threading.Tasks;
using GASSample.APIGateway;
using GASSample.AssetManagement;
using GASSample.Message;
using GASSample.Scene;
using MackySoft.Navigathena.SceneManagement;
using R3;
using RPGSample.Message;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace GASSample.UI
{
    public struct StatusData
    {
        public float OldValue { get; private set; }
        public float NewValue { get; private set; }
        public float OldMaxValue{ get; private set; }
        public float NewMaxValue{ get; private set; }
        public StatusData(float oldValue, float newValue, float oldMaxValue, float newMaxValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
            OldMaxValue = oldMaxValue;
            NewMaxValue = newMaxValue;
        }
    }
    [VitalRouter.Routes]
    public partial class UIWindowGameplayHUD : UIWindow
    {
        [Inject] IAssetModule assetModule;
        [Inject] ISceneManagementAPIGateway sceneManagementAPI;
        [SerializeField] Button Btn_Back;
        [SerializeField] StatusBar Bar_Health;
        [SerializeField] StatusBar Bar_Stamina;
        [SerializeField] StatusBar Bar_Exp;

        protected override void Awake()
        {
            base.Awake();
            
            Bar_Health?.Reset();
            Bar_Stamina?.Reset();
            Bar_Exp?.Reset();

            MapTo(MessageContext.UIRouter);
            Btn_Back.OnClickAsObservable().Subscribe(async _ => await BackToTitle());
        }

        protected override void OnDestroy()
        {
            UnmapRoutes();

            base.OnDestroy();
        }

        async UniTask BackToTitle()
        {
            var pkg = assetModule.GetPackage(AssetPackageName.DefaultPackage);
            AssetManagementSceneIdentifier sceneTitle = new AssetManagementSceneIdentifier(pkg, ScenePath.Title, LoadSceneMode.Additive, true);
            AssetManagementSceneIdentifier sceneTransition = new AssetManagementSceneIdentifier(pkg, ScenePath.Transition, LoadSceneMode.Additive, true);
            await GlobalSceneNavigator.Instance.Push(sceneTitle, interruptOperation: new UnloadPackageAssetsOperation(pkg), transitionDirector: new SimpleTransitionDirector(assetModule, sceneTransition, new TransitionDisplayData(0.25f, 0.25f)));
        }

        [VitalRouter.Route]
        void RefreshHealth(UIMessage<StatusData> msg)
        {
            if (msg.CommandID == MessageConstant.UpdateHealth)
            {
                float normalizedProgress = msg.Arg.NewMaxValue == 0 ? 0 : msg.Arg.NewValue / msg.Arg.NewMaxValue;
                normalizedProgress = Mathf.Clamp01(normalizedProgress);
                Bar_Health.Refresh(normalizedProgress, $"{Mathf.FloorToInt(msg.Arg.NewValue)} / {Mathf.FloorToInt(msg.Arg.NewMaxValue)}");
            }
            else if (msg.CommandID == MessageConstant.UpdateStamina)
            {
                float normalizedProgress = msg.Arg.NewMaxValue == 0 ? 0 : msg.Arg.NewValue / msg.Arg.NewMaxValue;
                normalizedProgress = Mathf.Clamp01(normalizedProgress);
                Bar_Stamina.Refresh(normalizedProgress, $"{Mathf.FloorToInt(msg.Arg.NewValue)} / {Mathf.FloorToInt(msg.Arg.NewMaxValue)}");
            }
            else if (msg.CommandID == MessageConstant.UpdateExperience)
            {
                float normalizedProgress = msg.Arg.NewMaxValue == 0 ? 0 : msg.Arg.NewValue / msg.Arg.NewMaxValue;
                normalizedProgress = Mathf.Clamp01(normalizedProgress);
                Bar_Exp.Refresh(normalizedProgress, $"{Mathf.FloorToInt(msg.Arg.NewValue)} / {Mathf.FloorToInt(msg.Arg.NewMaxValue)}");
            }
        }
    }
}