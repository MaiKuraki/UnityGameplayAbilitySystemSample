namespace CycloneGames.UIFramework
{
    public class OpenedState : IUIPageState
    {
        public void OnEnter(UIPage page)
        {
            UnityEngine.Debug.Log($"[PageState] Opened: {page.PageName}");
        }

        public void OnExit(UIPage page)
        {
            
        }

        public void Update(UIPage page)
        {
            
        }
    }
}