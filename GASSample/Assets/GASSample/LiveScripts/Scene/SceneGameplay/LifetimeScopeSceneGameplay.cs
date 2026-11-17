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

            // The framework owns World creation: register the WorldSettings asset and let the
            // GameInstance build the sealed World plus its GameMode/PlayerController/Pawn prefabs.
            builder.RegisterInstance(worldSettings);
            builder.Register<IActorLifetime, UnityActorLifetime>(Lifetime.Singleton);
            builder.Register<GameInstance>(
                resolver => new GameInstance(resolver.Resolve<IActorLifetime>(), localPlayerCount: 1),
                Lifetime.Singleton);

            builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
            {
                // Drives the Unity PlayerLoop phases into the manually composed GameInstance.
                entryPoints.Add<GameInstanceTickDriver>();
                // Starts the world and opens the HUD.
                entryPoints.Add<GASSampleGameplayEntryPoint>();
            });
        }
    }
}
