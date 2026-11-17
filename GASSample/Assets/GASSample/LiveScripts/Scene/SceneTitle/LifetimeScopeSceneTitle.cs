using VContainer;
using VContainer.Unity;

namespace GASSample.Scene
{
    public class LifetimeScopeSceneTitle : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterEntryPoint<TitleEntryPoint>();
        }
    }
}
