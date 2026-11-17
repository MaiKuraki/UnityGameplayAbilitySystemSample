using System;
using System.Threading;
using CycloneGames.AssetManagement.Runtime;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GASSample.AOT
{
    /// <summary>
    /// Initializes the asset package and spawns the LiveContent entry prefab.
    /// <para>
    /// BuiltIn holds no reference to LiveContent assets so the latter can move to a hot-update
    /// assembly; the path below is the only link between the two layers.
    /// </para>
    /// </summary>
    public class LaunchBootstrapper : IAsyncStartable
    {
        /// <summary>Entry prefab of the LiveContent layer.</summary>
        private const string LiveContentEntryPath =
            "Assets/GASSample/LiveContent/Prefabs/Core/ProjectSharedLifetimeScope.prefab";

        private const string DefaultPackage = "DefaultPackage";

        private readonly IAssetModule assetModule;

        public LaunchBootstrapper(IAssetModule assetModule)
        {
            this.assetModule = assetModule;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            await AssetManager.InitializeDefaultPackageAsync(
                assetModule,
                DefaultPackage,
                new AssetManagementOptions(),
                new AssetPackageInitOptions(),
                cancellation);

            IAssetPackage package = assetModule.GetPackage(DefaultPackage);
            var prefab = package.LoadAssetAsync<GameObject>(LiveContentEntryPath);
            await prefab.Task;

            GameObject liveContent = UnityEngine.Object.Instantiate(prefab.AssetObject) as GameObject;
            UnityEngine.Object.DontDestroyOnLoad(liveContent);
        }
    }
}
