using System;
using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;
using VContainer.Unity;

namespace GASSample.AOT
{
    public class LifetimeScopeSceneLaunch : LifetimeScope
    {
        private const string DEBUG_FLAG = "[LifecycleSceneLaunch]";
        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            UnityEngine.Debug.Log($"{DEBUG_FLAG} Configure");
            builder.RegisterSceneLifecycle<LifecycleSceneLaunch>();
            builder.RegisterEntryPoint<ApplicationLaunchPresenter>();
        }

        override protected void Awake()
        {
            base.Awake();
            UnityEngine.Debug.Log($"{DEBUG_FLAG} Awake");
        }

        void Start()
        {
            UnityEngine.Debug.Log($"{DEBUG_FLAG} UnityStart");
        }

        override protected void OnDestroy()
        {
            base.OnDestroy();
            UnityEngine.Debug.Log($"{DEBUG_FLAG} OnDestroy");
        }
    }

    public class ApplicationLaunchPresenter : IStartable, IDisposable
    {
        private const string DEBUG_FLAG = "[LifecycleSceneLaunch]";

        public ApplicationLaunchPresenter()
        {
            UnityEngine.Debug.Log($"{DEBUG_FLAG} Construct");
        }

        public void Dispose()
        {
            UnityEngine.Debug.Log($"{DEBUG_FLAG} Dispose");
        }

        public void Start()
        {
            UnityEngine.Debug.Log($"{DEBUG_FLAG} PresenterStart");
        }
    }
}