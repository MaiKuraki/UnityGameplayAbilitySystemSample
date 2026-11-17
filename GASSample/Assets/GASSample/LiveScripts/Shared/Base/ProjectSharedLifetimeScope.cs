using CycloneGames.AssetManagement.Runtime;
using CycloneGames.UIFramework.Runtime;
using CycloneGames.UIFramework.Runtime.Integrations;
using GASSample.AssetManagement;
using GASSample.Scene;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GASSample
{
    /// <summary>
    /// The persistent scope: created by VContainerSettings as a child of the root scope, it survives
    /// every scene change and owns the services shared by all scenes - the asset path resolver, the
    /// UI service, and the prefabs that must not be destroyed with a scene (camera, UI, debug tools).
    /// </summary>
    public class ProjectSharedLifetimeScope : LifetimeScope
    {
        [SerializeField] private AssetResolverForDontDestroy assetResolver;

        private static ProjectSharedLifetimeScope instance;

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
            base.Configure(builder);

            builder.Register<IAssetPathBuilderFactory, GASSampleAssetPathBuilderFactory>(Lifetime.Singleton);
            builder.Register<IUIService>(CreateUIService, Lifetime.Singleton);

            builder.RegisterComponentInNewPrefab(assetResolver, Lifetime.Singleton).UnderTransform(transform);
            builder.RegisterBuildCallback(resolver => resolver.Resolve<AssetResolverForDontDestroy>());

            // Runs once, from the launch scene: creates the persistent objects, initializes input and
            // starts the scene flow.
            builder.RegisterEntryPoint<BootEntryPoint>();
        }

        /// <summary>
        /// Built lazily because it needs the UIRoot from the UIFramework prefab, which is spawned by
        /// <see cref="AssetResolverForDontDestroy"/> after the scope has been configured.
        /// </summary>
        private static IUIService CreateUIService(IObjectResolver resolver)
        {
            var assetPathFactory = resolver.Resolve<IAssetPathBuilderFactory>();
            var package = resolver.Resolve<IAssetModule>().GetPackage(AssetPackageName.DefaultPackage);

            var assetProvider = new AssetManagementUIWindowAssetProvider(
                package,
                assetPathFactory.Create("UI"));

            // Windows are instantiated by UIService, not by the container, so the binder is what
            // fills their [Inject] fields.
            var binders = new IUIWindowBinder[] { new VContainerWindowBinder(resolver) };

            return new UIService(Object.FindFirstObjectByType<UIRoot>(), assetProvider, binders: binders);
        }
    }
}
