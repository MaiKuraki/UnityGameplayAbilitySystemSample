namespace CycloneGames.UIFramework
{
    // CAUTION: if you modify this interface name,
    //          don't forget modify the link.xml file located in the CycloneGames.UIFramework/Scripts/Framework folder
    public interface IUIPageState
    {
        void OnEnter(UIPage page);
        void OnExit(UIPage page);
        void Update(UIPage page);
    }
}