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
    /// Child of the root scope that survives scene changes. Owns the services shared by every scene
    /// and the prefabs that must outlive it (camera, UI, debug tools).
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

            builder.RegisterEntryPoint<BootEntryPoint>();
        }

        /// <summary>
        /// Lazy: the UIRoot it needs is spawned by <see cref="AssetResolverForDontDestroy"/> after this
        /// scope is configured.
        /// </summary>
        private static IUIService CreateUIService(IObjectResolver resolver)
        {
            var assetPathFactory = resolver.Resolve<IAssetPathBuilderFactory>();
            var package = resolver.Resolve<IAssetModule>().GetPackage(AssetPackageName.DefaultPackage);

            var assetProvider = new AssetManagementUIWindowAssetProvider(
                package,
                assetPathFactory.Create("UI"));

            // Windows are created by UIService, so injection goes through the binder.
            var binders = new IUIWindowBinder[] { new VContainerWindowBinder(resolver) };

            return new UIService(Object.FindFirstObjectByType<UIRoot>(), assetProvider, binders: binders);
        }
    }
}
