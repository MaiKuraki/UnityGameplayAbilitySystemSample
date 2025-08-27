using CycloneGames.UIFramework;
using GASSample.APIGateway;
using GASSample.Scene;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace GASSample.UI
{
    public class UIWindowTitle : UIWindow
    {
        [Inject] private readonly ISceneManagementAPIGateway sceneManagementAPIGateway;
        [SerializeField] private Button buttonStart;
        [SerializeField] private TMP_Text versionText;
        private const string EDITOR_VERSION_TEXT = "EDITOR_MODE";
        private const string INVALID_VERSION_TEXT = "INVALID";

        protected override void Awake()
        {
            base.Awake();

            buttonStart.OnClickAsObservable().Subscribe(_ => ClickStart());

#if UNITY_EDITOR
            versionText.text = EDITOR_VERSION_TEXT;
#else
            DisplayBuildVersion();
#endif
        }

        void ClickStart()
        {
            // CLogger.LogInfo("[UIWindowTitle] ClickStart");
            sceneManagementAPIGateway.Push(SceneDefinitions.Gameplay);
        }

        /// <summary>
        /// Loads version info from Resources and displays it.
        /// This method is only intended to be called in a built player.
        /// </summary>
        private void DisplayBuildVersion()
        {
            var versionInfo = Resources.Load<VersionInfoData>("VersionInfoData");

            if (versionInfo != null && !string.IsNullOrEmpty(versionInfo.commitHash))
            {
                string shortHash = versionInfo.commitHash.Length > 8
                    ? versionInfo.commitHash.Substring(0, 8)
                    : versionInfo.commitHash;

                versionText.text = shortHash;
            }
            else
            {
                versionText.text = INVALID_VERSION_TEXT;
            }
        }
    }
}