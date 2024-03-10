namespace CycloneGames.UIFramework
{
    public class ClosingState : IUIPageState
    {
        public void OnEnter(UIPage page)
        {
            UnityEngine.Debug.Log($"[PageState] Closing: {page.PageName}");
        }

        public void OnExit(UIPage page)
        {
            
        }

        public void Update(UIPage page)
        {
            
        }
    }
}