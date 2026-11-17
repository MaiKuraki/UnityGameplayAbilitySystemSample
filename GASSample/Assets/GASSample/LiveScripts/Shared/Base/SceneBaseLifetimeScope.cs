using CycloneGames.AssetManagement.Runtime;
using CycloneGames.Factory.Runtime;
using CycloneGames.UIFramework.Runtime;
using CycloneGames.UIFramework.Runtime.Integrations;
using GASSample.AssetManagement;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GASSample.Scene
{
    /// <summary> 
    /// The base lifetime scope class for the scene, which inherits from LifetimeScope provided by VContainer. 
    /// </summary> 
    public class SceneBaseLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.Register<IAssetPathBuilderFactory, GASSampleAssetPathBuilderFactory>(Lifetime.Singleton);
            builder.Register<IUnityObjectSpawner, GASSampleObjectSpawner>(Lifetime.Singleton);
            builder.Register<IUIService>(CreateUIService, Lifetime.Singleton);
        }

        /// <summary>
        /// UIService is constructed lazily: it needs the UIRoot that lives in the persistent UI prefab
        /// and the default asset package, neither of which exists while the scope is still configuring.
        /// </summary>
        private static IUIService CreateUIService(IObjectResolver resolver)
        {
            var assetPathFactory = resolver.Resolve<IAssetPathBuilderFactory>();
            var assetModule = resolver.Resolve<IAssetModule>();
            var package = assetModule.GetPackage(AssetPackageName.DefaultPackage);

            var uiRoot = Object.FindFirstObjectByType<UIRoot>();
            if (uiRoot == null)
            {
                throw new MissingReferenceException(
                    "UIService requires a UIRoot in the loaded scenes. Add the UIFramework prefab to the initial scene.");
            }

            var assetProvider = new AssetManagementUIWindowAssetProvider(
                package,
                assetPathFactory.Create("UI"));

            // Windows are instantiated by UIService, not by the container, so their [Inject]
            // fields are filled through the explicit VContainer binder.
            var binders = new IUIWindowBinder[] { new VContainerWindowBinder(resolver) };

            return new UIService(uiRoot, assetProvider, binders: binders);
        }
    }
}
