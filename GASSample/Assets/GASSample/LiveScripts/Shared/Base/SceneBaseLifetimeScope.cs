using CycloneGames.AssetManagement.Runtime;
using CycloneGames.UIFramework.Runtime;
using CycloneGames.UIFramework.Runtime.Integrations;
using GASSample.AssetManagement;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GASSample.Scene
{
    /// <summary>
    /// Services every scene shares. A scene scope derives from this and adds its own lifecycle
    /// and presenters on top.
    /// </summary>
    public class SceneBaseLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.Register<IAssetPathBuilderFactory, GASSampleAssetPathBuilderFactory>(Lifetime.Singleton);
            builder.Register<IUIService>(CreateUIService, Lifetime.Singleton);
        }

        /// <summary>
        /// Built lazily because it needs the UIRoot from the persistent UIFramework prefab, which
        /// does not exist yet while the scope is still configuring.
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
