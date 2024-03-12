namespace ARPGSample.UI
{
    /// <summary>
    /// Note: The name is derived from the AddressablesAddress. 
    /// Make sure the PageName is Unique.
    /// </summary>
    public static class PageName
    {
        //  Launch
        public static readonly string TitlePage = "TitlePage";  // SceneLogic_LaunchScene require this Page, if you want to modify this value, please sync to SceneLogic_LaunchScene.cs file
        
        //  Loading
        public static readonly string SimpleLoadingPage = "SimpleLoadingPage";  // SceneLogic_LaunchScene require this Page, if you want to modify this value, please sync to SceneLogic_LaunchScene.cs file
        
        //  Login
        public static readonly string StartUpPage = "StartUpPage";
        public static readonly string TutorialPage = "TutorialPage";
        
        //  Gameplay
        public static readonly string GameplayMenuPage = "GameplayMenuPage";
        public static readonly string DialoguePage = "DialoguePage";
        public static readonly string HUDPage = "HUDPage";
        public static readonly string BattleInfoPage = "BattleInfoPage";
        
        //  TouchInput
        public static readonly string GameplayTouchInputPage = "GameplayTouchInputPage";
    }
}