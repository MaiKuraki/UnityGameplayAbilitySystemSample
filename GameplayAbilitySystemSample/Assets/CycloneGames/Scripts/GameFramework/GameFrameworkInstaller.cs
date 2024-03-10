using Zenject;

namespace CycloneGames.GameFramework
{
    public class GameFrameworkInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameInstance>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName("GameInstance")
                .AsSingle().NonLazy();
            Container.BindInterfacesTo<World>().AsSingle().NonLazy();
        }
    }
}