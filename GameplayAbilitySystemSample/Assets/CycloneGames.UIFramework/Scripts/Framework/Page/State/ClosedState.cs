namespace CycloneGames.UIFramework
{
    public class ClosedState : IUIPageState
    {
        public void OnEnter(UIPage page)
        {
            UnityEngine.Debug.Log($"[PageState] Closed: {page.PageName}");
        }

        public void OnExit(UIPage page)
        {
            
        }

        public void Update(UIPage page)
        {
            
        }
    }
}