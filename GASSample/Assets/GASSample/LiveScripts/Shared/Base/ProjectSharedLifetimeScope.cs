using VContainer;
using VContainer.Unity;
using CycloneGames.Logger;
using CycloneGames.Service;
using GASSample.APIGateway;

namespace GASSample
{
    public class ProjectSharedLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.Register<IMainCameraService, MainCameraService>(Lifetime.Singleton);
            builder.Register<ISceneManagementAPIGateway, SceneManagementAPIGateway>(Lifetime.Singleton);
            builder.Register<IGraphicsSettingService, GraphicsSettingService>(Lifetime.Singleton);

            builder.RegisterEntryPoint<MapListGlobalInitializer>();
        }

        public class MapListGlobalInitializer : IStartable
        {
            public MapListGlobalInitializer()
            {

            }

            public void Start()
            {

                CLogger.LogInfo("[ProjectSharedLifetimeScope] Initiated");
            }
        }
    }
}