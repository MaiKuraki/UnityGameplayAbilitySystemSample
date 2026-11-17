using GASSample.Gameplay;
using VContainer;
using VContainer.Unity;

namespace GASSample.Scene
{
    public class LifetimeScopeSceneGameplay : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            // The spawner is a scene object; it holds the character prefab reference.
            builder.RegisterComponentInHierarchy<PlayerSpawner>();
            builder.RegisterEntryPoint<GASSampleGameplayEntryPoint>();
        }
    }
}
