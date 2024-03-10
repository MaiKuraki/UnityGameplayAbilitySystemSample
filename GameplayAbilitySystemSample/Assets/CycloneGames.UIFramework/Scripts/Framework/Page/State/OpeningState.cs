namespace CycloneGames.UIFramework
{
    public class OpeningState : IUIPageState
    {
        public void OnEnter(UIPage page)
        {
            UnityEngine.Debug.Log($"[PageState] Opening: {page.PageName}");
        }

        public void OnExit(UIPage page)
        {
            
        }

        public void Update(UIPage page)
        {
            
        }
    }
}