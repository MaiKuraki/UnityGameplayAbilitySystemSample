using CycloneGames.AssetManagement.Runtime;
using VContainer;
using VContainer.Unity;

namespace GASSample.AOT
{
    public class RootLifetimeScope : LifetimeScope
    {
        public static RootLifetimeScope Instance { get; private set; }
        private const string DefaultPackage = "DefaultPackage";

        protected override void Awake()
        {
            // Singleton check
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            // Safety check
            if (Instance != null && Instance != this) return;

            builder.Register<IAssetModule, AddressablesModule>(Lifetime.Singleton);

            builder.RegisterBuildCallback(async resolver =>
            {
                var assetModule = resolver.Resolve<IAssetModule>();
                await assetModule.InitializeAsync(new AssetManagementOptions(operationSystemMaxTimeSliceMs: 16));
                await AssetPackageFactory.CreateAndInitializePackageAsync(
                    module: assetModule,
                    packageName: DefaultPackage,
                    options: new AssetPackageInitOptions(AssetPlayMode.Offline, null, bundleLoadingMaxConcurrencyOverride: 8));
            });
        }
    }
}