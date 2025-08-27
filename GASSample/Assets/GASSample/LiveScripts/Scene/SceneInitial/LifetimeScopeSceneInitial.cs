
using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;
using VContainer.Unity;
using CycloneGames.Service;

namespace GASSample.Scene
{
    public class LifetimeScopeSceneInitial : SceneBaseLifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterSceneLifecycle<LifecycleSceneInitial>();
            
            builder.RegisterEntryPoint<ApplicationInitialPresenter>();
        }
    }

    public class ApplicationInitialPresenter : IStartable
    {
        private readonly IGraphicsSettingService graphicsSettingService;

        public ApplicationInitialPresenter(IGraphicsSettingService graphicsSettingService)
        {
            this.graphicsSettingService = graphicsSettingService;
        }

        public void Start()
        {
            graphicsSettingService.ChangeRenderResolution(1080);
            graphicsSettingService.ChangeApplicationFrameRate(60);
        }
    }
}
