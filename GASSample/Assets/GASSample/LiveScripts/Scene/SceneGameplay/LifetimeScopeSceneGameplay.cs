using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;

namespace GASSample.Scene
{
    public class LifetimeScopeSceneGameplay : SceneBaseLifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterSceneLifecycle<LifecycleSceneGameplay>();
        }
    }
}