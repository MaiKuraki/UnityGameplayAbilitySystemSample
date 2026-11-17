using CycloneGames.AssetManagement.Runtime;
using VContainer;
using VContainer.Unity;

namespace GASSample.AOT
{
    /// <summary>
    /// Outermost scope, created at startup by VContainerSettings. Owns the asset module for the
    /// application lifetime.
    /// </summary>
    public class RootLifetimeScope : LifetimeScope
    {
        private static RootLifetimeScope instance;

        protected override void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IAssetModule, AddressablesModule>(Lifetime.Singleton);

            // Initializes the asset package and spawns the LiveContent entry prefab by path.
            builder.RegisterEntryPoint<LaunchBootstrapper>();
        }
    }
}
