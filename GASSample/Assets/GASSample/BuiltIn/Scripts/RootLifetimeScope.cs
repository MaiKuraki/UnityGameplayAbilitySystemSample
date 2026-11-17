using CycloneGames.AssetManagement.Runtime;
using VContainer;
using VContainer.Unity;

namespace GASSample.AOT
{
    /// <summary>
    /// Outermost scope, created with the built-in launch scene. It owns the asset module for the
    /// whole application lifetime.
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
            // Only the module lives here; the default package is initialized by the boot entry point,
            // which is the first thing that actually needs it.
            builder.Register<IAssetModule, AddressablesModule>(Lifetime.Singleton);
        }
    }
}
