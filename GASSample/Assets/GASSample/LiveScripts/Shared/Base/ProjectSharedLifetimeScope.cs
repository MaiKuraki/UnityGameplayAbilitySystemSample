using VContainer;
using VContainer.Unity;
using UnityEngine;
using CycloneGames.AssetManagement.Runtime;
using CycloneGames.Factory.Runtime;
using CycloneGames.UIFramework.Runtime;
using CycloneGames.Service.Runtime;
using GASSample.APIGateway;
using GASSample.AssetManagement;
using GASSample.UI;

namespace GASSample
{
    public class ProjectSharedLifetimeScope : LifetimeScope
    {
        public static ProjectSharedLifetimeScope Instance { get; private set; }
        [SerializeField] private AssetResolverForDontDestroy assetResolver;

        protected override void Awake()
        {
            // Singleton check must happen before base.Awake() or initialization logic
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            base.Awake();
        }
        protected override void Configure(IContainerBuilder builder)
        {
            // Safety: If this instance is a duplicate scheduled for destruction, skip registration.
            if (Instance != null && Instance != this)
            {
                UnityEngine.Debug.LogWarning($"[ProjectSharedLifetimeScope] Skipping registration for duplicate instance: {name}. Existing Instance: {Instance.name}");
                return;
            }

            base.Configure(builder);

            builder.Register<ISceneManagementAPIGateway, SceneManagementAPIGateway>(Lifetime.Singleton);
            builder.Register<IMainCameraService, MainCameraService>(Lifetime.Singleton);
            builder.Register<IGraphicsSettingService, GraphicsSettingService>(Lifetime.Singleton);

            builder.RegisterComponentInNewPrefab(assetResolver, Lifetime.Singleton).UnderTransform(transform);
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Resolve<AssetResolverForDontDestroy>();
            });
            
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