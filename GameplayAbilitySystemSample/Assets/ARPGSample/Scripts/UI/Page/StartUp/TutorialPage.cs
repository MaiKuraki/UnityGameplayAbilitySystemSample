using CycloneGames.UIFramework;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ARPGSample.UI
{
    public class TutorialPage : UIPage
    {
        [Inject] private IUIService uiService;
        
        [SerializeField] private Button Btn_Back;

        protected override void Awake()
        {
            base.Awake();
            
            Btn_Back.onClick.AddListener(ClickBack);
        }

        void ClickBack()
        {
            uiService.CloseUI(UI.PageName.TutorialPage);
        }
    }
}

