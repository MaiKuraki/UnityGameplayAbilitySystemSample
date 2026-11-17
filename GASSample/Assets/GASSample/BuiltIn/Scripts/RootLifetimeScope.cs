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
        // Kept local on purpose: AOT cannot reference the HotUpdate assembly that owns
        // GASSample.AssetManagement.AssetPackageName.
        private const string DefaultPackage = "DefaultPackage";

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

            builder.RegisterBuildCallback(async resolver =>
            {
                await AssetManager.InitializeDefaultPackageAsync(
                    resolver.Resolve<IAssetModule>(),
                    DefaultPackage,
                    new AssetManagementOptions(),
                    new AssetPackageInitOptions());
            });
        }
    }
}
