using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;

namespace GASSample.Scene
{
    /// <summary>
    /// Scope for Scene_Initial: registers the bootstrap lifecycle that starts the scene flow.
    /// <para>
    /// Order of one scene load with Navigathena + VContainer:
    /// VContainer Awake → Configure → Navigathena OnInitialize → VContainer Startable
    /// → Unity Start → Navigathena OnEnter → OnExit/OnFinalize → VContainer OnDestroy.
    /// </para>
    /// </summary>
    public class LifetimeScopeSceneInitial : SceneBaseLifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterSceneLifecycle<LifecycleSceneInitial>();
        }
    }
}
