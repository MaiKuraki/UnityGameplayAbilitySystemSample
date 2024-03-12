using CycloneGames.GameFramework;

namespace ARPGSample.Gameplay
{
    public class RPGGameMode : GameMode
    {
        private bool _isGameplayStart = false;
        public bool IsGameplayStart => _isGameplayStart;
        
        public override void LaunchGameMode()
        {
            base.LaunchGameMode();

            _isGameplayStart = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Debug.LogError("Exit Gameplay");
            _isGameplayStart = false;
        }
    }
}