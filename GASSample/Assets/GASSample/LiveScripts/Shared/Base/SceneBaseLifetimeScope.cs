using CycloneGames.AssetManagement.Runtime;
using CycloneGames.Factory.Runtime;
using CycloneGames.Service.Runtime;
using CycloneGames.UIFramework.Runtime;
using GASSample.AssetManagement;
using GASSample.UI;
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
            builder.Register<IUIService, GASSampleUIService>(Lifetime.Singleton);

            builder.RegisterBuildCallback(resolver =>
            {
                var uiService = resolver.Resolve<IUIService>();
                var assetPathFactory = resolver.Resolve<IAssetPathBuilderFactory>();
                var objectSpawner = resolver.Resolve<IUnityObjectSpawner>();
                var cameraService = resolver.Resolve<IMainCameraService>();
                var assetModule = resolver.Resolve<IAssetModule>();
                var pkg = assetModule.GetPackage(AssetPackageName.DefaultPackage);

                uiService.Initialize(assetPathFactory, objectSpawner, cameraService, pkg);
            });
        }
    }
}