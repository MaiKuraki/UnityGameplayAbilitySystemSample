using CycloneGames.AssetManagement.Runtime;
using CycloneGames.GameplayFramework.Runtime;
using GASSample.Gameplay;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GASSample.Scene
{
    public class LifetimeScopeSceneGameplay : LifetimeScope
    {
        [SerializeField] private WorldSettings worldSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            // The framework creates the World from WorldSettings; register the asset and its instance.
            builder.RegisterInstance(worldSettings);
            builder.Register<IActorLifetime, UnityActorLifetime>(Lifetime.Singleton);
            builder.Register<GameInstance>(
                resolver => new GameInstance(resolver.Resolve<IActorLifetime>(), localPlayerCount: 1),
                Lifetime.Singleton);

            builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
            {
                entryPoints.Add<GameInstanceTickDriver>();
                entryPoints.Add<GASSampleGameplayEntryPoint>();
            });
        }
    }
}
