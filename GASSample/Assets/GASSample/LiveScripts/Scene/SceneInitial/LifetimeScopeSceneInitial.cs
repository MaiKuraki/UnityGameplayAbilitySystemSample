using MackySoft.Navigathena.SceneManagement.VContainer;
using VContainer;
using VContainer.Unity;
using CycloneGames.Service;
using System;
using CycloneGames.Logger;

/// Pipeline:  VContainer   Awake
///         -> VContainer   OnEnable
///         -> VContainer   Configure
///         -> VContainer   Presenter Construct     //  May not triggered correct on Navigathena switch scene directly in OnEnter
///         -> Navigathena  ITransitionHandle.Start (Last Scene)   Navigathena.Exit(LastScene) -> Navigathena.ITransitionHandle.Start(LastScene) -> Navigathena.Finialize(LastScene)
///         -> Navigathena  OnInitialize
///         -> Navigathena  ITransitionHandle.End   (Current Scene)
///         -> Navigathena  OnEnter
///         -> VContainer   Startable Start         //  May not triggered correct on Navigathena switch scene directly in OnEnter
///         -> Unity        Start                   //  May not triggered correct on Navigathena switch scene directly in OnEnter
///         -> Unity        First Update            //  May not triggered correct on Navigathena switch scene directly in OnEnter
///         -> Navigathena  OnExit
///         -> Navigathena  OnFinalize
///         -> Unity        OnDisable
///         -> VContainer   Dispose
///         -> VContainer   OnDestroy

namespace GASSample.Scene
{
    public class LifetimeScopeSceneInitial : SceneBaseLifetimeScope
    {
        private const string DEBUG_FLAG = "[SceneInitial] VContainer";
        private int updateIdx = 0;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            CLogger.LogInfo($"{DEBUG_FLAG} Configure");

            builder.RegisterSceneLifecycle<LifecycleSceneInitial>();
            builder.RegisterEntryPoint<ApplicationInitialPresenter>();
        }

        override protected void Awake()
        {
            base.Awake();

            CLogger.LogInfo($"{DEBUG_FLAG} Awake");
        }

        void Update()
        {
            if (updateIdx == 0)
            {
                updateIdx++;
                CLogger.LogInfo($"{DEBUG_FLAG} First Update");
            }
        }

        void OnEnable()
        {
            CLogger.LogInfo($"{DEBUG_FLAG} OnEnable");
        }

        void OnDisable()
        {
            CLogger.LogInfo($"{DEBUG_FLAG} OnDisable");
        }

        void Start()
        {
            CLogger.LogInfo($"{DEBUG_FLAG} Unity Start");
        }

        override protected void OnDestroy()
        {
            base.OnDestroy();

            CLogger.LogInfo($"{DEBUG_FLAG} OnDestroy");
        }
    }

    //  Caution: because of Navigathena, if developer trigger Navigathena's SceneManagement(Push), we may not Trigger VContainer.Start form IStartable even Construct, this class not recommand be child of IStartable
    public class ApplicationInitialPresenter : IDisposable, IStartable
    {
        private const string DEBUG_FLAG = "[SceneInitial] VContainer Presenter";

        public ApplicationInitialPresenter(IGraphicsSettingService graphicsSettingService)
        {
            CLogger.LogInfo($"{DEBUG_FLAG} Construct");
        }

        public void Dispose()
        {
            CLogger.LogInfo($"{DEBUG_FLAG} Dispose");
        }

        public void Start()
        {
            CLogger.LogInfo($"{DEBUG_FLAG} Start");
        }
    }
}
