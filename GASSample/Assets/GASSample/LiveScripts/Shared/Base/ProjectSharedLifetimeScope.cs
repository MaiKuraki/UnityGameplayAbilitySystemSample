using VContainer;
using VContainer.Unity;
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

            builder.RegisterEntryPoint<ProjectGlobalInitializer>();
        }

        public class ProjectGlobalInitializer : IStartable
        {
            public ProjectGlobalInitializer()
            {

            }

            public void Start()
            {

                // CLogger.LogInfo("[ProjectSharedLifetimeScope] Initiated");
            }
        }
    }
}