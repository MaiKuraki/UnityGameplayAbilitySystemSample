using System;
using ARPGSample.GameSubSystem;
using CycloneGames.UIFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ARPGSample.UI
{
    public class StartUpPage : UIPage
    {
        [Inject] private ILocalSave localSaveService;
        [Inject] private IUIService uiService;
        
        [SerializeField] private TMP_Text Text_Version;
        [SerializeField] private Button Btn_NewGame;
        [SerializeField] private Button Btn_ContinuePlay;
        [SerializeField] private Button Btn_Tutorial;
        [SerializeField] private Button Btn_Credit;
        [SerializeField] private Button Btn_Exit;
        public event Action OnClickNewGame;
        public event Action OnClickContinuePlay;
        public event Action OnClickTutorial;
        public event Action OnClickCredit;
        public event Action OnClickExitGame;

        private Transform ContinuePlayBtnTF;

        protected override void Awake()
        {
            base.Awake();

            ContinuePlayBtnTF = Btn_ContinuePlay.transform;
            Btn_NewGame.onClick.AddListener(ClickNewGame);
            Btn_ContinuePlay.onClick.AddListener(ClickContinuePlay);
            Btn_Tutorial.onClick.AddListener(ClickTutorial);
            Btn_Credit.onClick.AddListener(ClickCredit);
            Btn_Exit.onClick.AddListener(ClickExitGame);
        }

        protected override void Start()
        {
            base.Start();

            if (!localSaveService.SaveDataValid)
            {
                ContinuePlayBtnTF.gameObject.SetActive(false);
            }
        }

        void ClickNewGame()
        {
            OnClickNewGame?.Invoke();
        }

        void ClickContinuePlay()
        {
            OnClickContinuePlay?.Invoke();
        }

        void ClickTutorial()
        {
            uiService.OpenUI(UI.PageName.TutorialPage);
            OnClickTutorial?.Invoke();
        }

        void ClickCredit()
        {
            OnClickCredit?.Invoke();
        }

        void ClickExitGame()
        {
            OnClickExitGame?.Invoke();
            
#if UNITY_EDITOR
            // 如果我们在Unity编辑器中，停止播放模式
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // 在构建的游戏中，退出应用程序
            Application.Quit();
#endif
        }
    }
}