using GASSample.AssetManagement;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GASSample
{
    /// <summary>
    /// Persistent scope created with Scene_Initial. It keeps the objects that must outlive a scene
    /// change alive: debug tools, event system, main camera and the UIFramework prefab, all spawned
    /// by <see cref="AssetResolverForDontDestroy"/>.
    /// </summary>
    public class ProjectSharedLifetimeScope : LifetimeScope
    {
        [SerializeField] private AssetResolverForDontDestroy assetResolver;

        private static ProjectSharedLifetimeScope instance;

        protected override void Awake()
        {
            // Scene_Initial only contains one scope; guard against a duplicate copy anyway.
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterComponentInNewPrefab(assetResolver, Lifetime.Singleton).UnderTransform(transform);
            builder.RegisterBuildCallback(resolver => resolver.Resolve<AssetResolverForDontDestroy>());
        }
    }
}
