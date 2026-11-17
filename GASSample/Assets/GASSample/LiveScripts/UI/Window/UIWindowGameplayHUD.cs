using CycloneGames.AssetManagement.Runtime;
using CycloneGames.AssetManagement.Runtime.Integrations.Navigathena;
using CycloneGames.UIFramework.Runtime;
using Cysharp.Threading.Tasks;
using GASSample.AssetManagement;
using GASSample.Gameplay;
using GASSample.Scene;
using MackySoft.Navigathena.SceneManagement;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace GASSample.UI
{
    /// <summary>
    /// Gameplay HUD: three bars driven by the local player character, plus a button back to title.
    /// </summary>
    public class UIWindowGameplayHUD : UIWindow
    {
        [Inject] private readonly IAssetModule assetModule;

        [SerializeField] private Button Btn_Back;
        [SerializeField] private StatusBar Bar_Health;
        [SerializeField] private StatusBar Bar_Stamina;
        [SerializeField] private StatusBar Bar_Exp;

        private GASSamplePlayerCharacter player;

        protected override void Awake()
        {
            base.Awake();

            Bar_Health.Reset();
            Bar_Stamina.Reset();
            Bar_Exp.Reset();

            Btn_Back.OnClickAsObservable().Subscribe(_ => BackToTitle().Forget());

            // The world is started before this window opens, so the player is already spawned.
            player = FindFirstObjectByType<GASSamplePlayerCharacter>();
            player.HealthChanged += OnHealthChanged;
            player.StaminaChanged += OnStaminaChanged;
            player.ExperienceChanged += OnExperienceChanged;

            // The initial values were applied before this window existed, so read them once.
            OnHealthChanged(player.Health, player.MaxHealth);
            OnStaminaChanged(player.Stamina, player.MaxStamina);
            OnExperienceChanged(player.Experience, player.ExperienceToNextLevel);
        }

        protected override void OnDestroy()
        {
            if (player != null)
            {
                player.HealthChanged -= OnHealthChanged;
                player.StaminaChanged -= OnStaminaChanged;
                player.ExperienceChanged -= OnExperienceChanged;
            }

            base.OnDestroy();
        }

        private void OnHealthChanged(float value, float max)
        {
            RefreshBar(Bar_Health, value, max);
        }

        private void OnStaminaChanged(float value, float max)
        {
            RefreshBar(Bar_Stamina, value, max);
        }

        private void OnExperienceChanged(float value, float max)
        {
            RefreshBar(Bar_Exp, value, max);
        }

        private static void RefreshBar(StatusBar bar, float value, float max)
        {
            float normalized = max <= 0f ? 0f : Mathf.Clamp01(value / max);
            bar.Refresh(normalized, $"{Mathf.FloorToInt(value)} / {Mathf.FloorToInt(max)}");
        }

        private async UniTask BackToTitle()
        {
            var pkg = assetModule.GetPackage(AssetPackageName.DefaultPackage);
            var sceneLoader = (IAssetSceneLoader)pkg;
            AssetManagementSceneIdentifier sceneTitle = new AssetManagementSceneIdentifier(sceneLoader, ScenePath.Title, LoadSceneMode.Additive, true);
            AssetManagementSceneIdentifier sceneTransition = new AssetManagementSceneIdentifier(sceneLoader, ScenePath.Transition, LoadSceneMode.Additive, true);
            await GlobalSceneNavigator.Instance.Push(sceneTitle, interruptOperation: new UnloadPackageAssetsOperation(pkg), transitionDirector: new SimpleTransitionDirector(assetModule, sceneTransition, new TransitionDisplayData(0.25f, 0.25f)));
        }
    }
}
