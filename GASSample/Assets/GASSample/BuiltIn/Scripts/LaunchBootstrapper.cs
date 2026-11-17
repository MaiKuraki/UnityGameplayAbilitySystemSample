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
    /// Initializes the asset package and spawns the two persistent roots of LiveContent: the UI first,
    /// then the gameplay scope. The UI has to exist before a scope asks for <c>IUIService</c>.
    /// <para>
    /// BuiltIn holds no reference to LiveContent assets so the latter can move to a hot-update
    /// assembly; the paths below are the only link between the two layers.
    /// </para>
    /// </summary>
    public class LaunchBootstrapper : IAsyncStartable
    {
        /// <summary>UI root of the LiveContent layer. Spawned before anything can request it.</summary>
        private const string UiRootPrefabPath =
            "Assets/GASSample/LiveContent/Prefabs/UI/UIFramework.prefab";

        /// <summary>Gameplay scope of the LiveContent layer.</summary>
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

            await SpawnPersistentAsync(package, UiRootPrefabPath);
            await SpawnPersistentAsync(package, LiveContentEntryPath);
        }

        private static async UniTask SpawnPersistentAsync(IAssetPackage package, string path)
        {
            var prefab = package.LoadAssetAsync<GameObject>(path);
            await prefab.Task;

            GameObject instance = UnityEngine.Object.Instantiate(prefab.AssetObject) as GameObject;
            UnityEngine.Object.DontDestroyOnLoad(instance);
        }
    }
}
