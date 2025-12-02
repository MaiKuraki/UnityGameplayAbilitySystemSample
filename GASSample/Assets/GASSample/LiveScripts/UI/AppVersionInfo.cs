using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using TMPro;
using CycloneGames.AssetManagement.Runtime;
using GASSample.AssetManagement;

namespace GASSample.UI
{
    public class AppVersionInfo : MonoBehaviour
    {
        [SerializeField] TMP_Text Text_VersionInfoValue;
        private const string EDITOR_VERSION_TEXT = "EDITOR_MODE";
        private const string INVALID = "INVALID";

        public Func<IAssetModule, UniTask> UpdateVersionDisplayEvent;

        private void Awake()
        {
#if UNITY_EDITOR
            if (Text_VersionInfoValue != null) Text_VersionInfoValue.text = EDITOR_VERSION_TEXT;
#else
            UpdateVersionDisplayEvent += UpdateVersionDisplay;
#endif   
        }

        private void OnDestroy()
        {
            UpdateVersionDisplayEvent -= UpdateVersionDisplay;
        }

        private async UniTask UpdateVersionDisplay(IAssetModule assetModule)
        {
            string appVersion = Application.version;
            string resVersion = "Unknown";

            if (assetModule != null)
            {
                var pkg = assetModule.GetPackage(AssetPackageName.DefaultPackage);
                if (pkg != null)
                {
                    resVersion = await pkg.RequestPackageVersionAsync(cancellationToken: destroyCancellationToken);
                }
            }

            SetVersion(appVersion, resVersion);
        }

        public void SetVersion(string appVersion, string resVersion)
        {
            if (Text_VersionInfoValue != null)
            {
                Text_VersionInfoValue.text = $"App:{appVersion} Res:{resVersion}";
            }
        }
    }
}