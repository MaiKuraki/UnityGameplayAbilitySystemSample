using MessagePipe;
using Zenject;

namespace CycloneGames.UIFramework
{
    public class UIMessage
    {
        public string MessageCode;
        public object[] Params;
    }
    public interface IUIService
    {
        void PublishUIMessage(UIMessage uiMsg);
        void OpenUI(string PageName, System.Action<UIPage> OnPageCreated = null);
        void CloseUI(string PageName);
        bool IsUIPageValid(string PageName);
        UIPage GetUIPage(string PageName);
    }
    public class UIService : IUIService, IInitializable
    {
        private const string DEBUG_FLAG = "[UIService]";
        
        [Inject] private IPublisher<UIMessage> uiMsgPub;
        [Inject] private DiContainer diContainer;
        
        private UIManager uiManager;
        
        public void Initialize()
        {
            uiManager = diContainer.InstantiateComponentOnNewGameObject<UIManager>("UIService");
            //UnityEngine.GameObject.DontDestroyOnLoad(uiManager);
        }

        public void PublishUIMessage(UIMessage uiMsg)
        {
            uiMsgPub.Publish(uiMsg);
        }

        public void OpenUI(string PageName, System.Action<UIPage> OnPageCreated = null)
        {
            if (uiManager == null)
            {
                UnityEngine.Debug.Log($"{DEBUG_FLAG} Invalid UIManager");
            }
            
            uiManager.OpenUI(PageName, OnPageCreated);
        }

        public void CloseUI(string PageName)
        {
            if (uiManager == null)
            {
                UnityEngine.Debug.Log($"{DEBUG_FLAG} Invalid UIManager");
            }
            
            uiManager.CloseUI(PageName);
        }

        public bool IsUIPageValid(string PageName)
        {
            return uiManager.IsUIPageValid(PageName);
        }

        public UIPage GetUIPage(string PageName)
        {
            return uiManager.GetUIPage(PageName);
        }
    }
}