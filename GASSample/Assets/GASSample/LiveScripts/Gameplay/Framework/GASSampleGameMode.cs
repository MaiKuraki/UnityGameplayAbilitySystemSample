using CycloneGames.GameplayFramework.Runtime;

namespace GASSample.Gameplay
{
    /// <summary>
    /// The World is sealed by the framework: it is created and owned by <see cref="GameInstance"/>.
    /// The sample only extends the GameMode, which the World instantiates from WorldSettings.
    /// </summary>
    public class GASSampleGameMode : GameMode
    {
        public override void Initialize(World targetWorld, IGameSession session = null)
        {
            base.Initialize(targetWorld, session);
        }
    }
}
