using CycloneGames.GameplayFramework.Runtime;
using GASSample.Gameplay;
using MackySoft.Navigathena.SceneManagement.VContainer;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GASSample.Scene
{
    public class LifetimeScopeSceneGameplay : SceneBaseLifetimeScope
    {
        [SerializeField] private WorldSettings worldSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterInstance<IWorldSettings>(worldSettings);        //  Register the instance as interface, don't register as class
            builder.Register<IWorld, GASSampleWorld>(Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab<IGameMode, GASSampleGameMode>(prefab => (GASSampleGameMode)worldSettings.GameModeClass, Lifetime.Singleton);

            builder.RegisterSceneLifecycle<LifecycleSceneGameplay>();
            builder.UseEntryPoints(Lifetime.Singleton, entryPoints =>
            {
                //  Start Game Logic
                entryPoints.Add<GASSampleGameplayEntryPoint>();
            });
        }
    }
}