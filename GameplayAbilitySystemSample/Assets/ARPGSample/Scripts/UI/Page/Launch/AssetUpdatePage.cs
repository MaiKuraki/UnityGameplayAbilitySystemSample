using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CycloneGames.UIFramework;

namespace ARPGSample.UI
{
    public class AssetUpdatePage : UIPage
    {
        [SerializeField] private Transform ProgressBarGroupTF;
        [SerializeField] private Slider HotUpdateProgressBar;
        [SerializeField] private TMP_Text PackageProgressText;
        [SerializeField] private TMP_Text PackageNameText;

        protected override void Awake()
        {
            base.Awake();

        }

        protected override void Start()
        {
            base.Start();

            HotUpdateProgressBar.value = 0;
            ProgressBarGroupTF.gameObject.SetActive(false);
        }
    
        protected override void OnDestroy()
        {
            base.OnDestroy();

        }
    }
}
