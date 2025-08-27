using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CycloneGames.AssetManagement
{
	/// <summary>
	/// Abstraction of the asset system. Designed for DI and provider-agnostic usage.
	/// </summary>
	public interface IAssetModule
	{
		bool Initialized { get; }

		/// <summary>
		/// Initializes the module. Idempotent. Safe to call multiple times.
		/// </summary>
		/// <param name="options">Global options (time slice, concurrency, logger etc.).</param>
		void Initialize(AssetModuleOptions options = default);

		/// <summary>
		/// Destroys the module and releases all resources.
		/// </summary>
		void Destroy();

		/// <summary>
		/// Creates a new logical package. Package must be initialized via <see cref="IAssetPackage.InitializeAsync"/> before use.
		/// </summary>
		IAssetPackage CreatePackage(string packageName);

		/// <summary>
		/// Gets a created package; returns null if not found.
		/// </summary>
		IAssetPackage GetPackage(string packageName);

		/// <summary>
		/// Removes a package. Only allowed after the package has been destroyed.
		/// </summary>
		bool RemovePackage(string packageName);

		/// <summary>
		/// Returns a snapshot of existing package names.
		/// </summary>
		IReadOnlyList<string> GetAllPackageNames();
	}

	/// <summary>
	/// Abstraction of a package (catalog + bundles). Provider specific implementation should be zero-GC in hot paths.
	/// </summary>
	public interface IAssetPackage
	{
		string Name { get; }

		/// <summary>
		/// Initializes the package.
		/// Provider-specific parameters are carried in <see cref="AssetPackageInitOptions.ProviderOptions"/>.
		/// </summary>
		Task<bool> InitializeAsync(AssetPackageInitOptions options, CancellationToken cancellationToken = default);

		/// <summary>
		/// Destroys the package and releases all provider resources.
		/// </summary>
		Task DestroyAsync();

		// --- Update & Download ---
		Task<string> RequestPackageVersionAsync(bool appendTimeTicks = true, int timeoutSeconds = 60, CancellationToken cancellationToken = default);
		Task<bool> UpdatePackageManifestAsync(string packageVersion, int timeoutSeconds = 60, CancellationToken cancellationToken = default);
		Task<bool> ClearCacheFilesAsync(string clearMode, object clearParam = null, CancellationToken cancellationToken = default);

		// Downloaders based on ACTIVE manifest
		IDownloader CreateDownloaderForAll(int downloadingMaxNumber, int failedTryAgain, int timeoutSeconds = 60);
		IDownloader CreateDownloaderForTags(string[] tags, int downloadingMaxNumber, int failedTryAgain, int timeoutSeconds = 60);
		IDownloader CreateDownloaderForLocations(string[] locations, bool recursiveDownload, int downloadingMaxNumber, int failedTryAgain, int timeoutSeconds = 60);

		// Pre-download for a SPECIFIC manifest version (without switching active manifest)
		Task<IDownloader> CreatePreDownloaderForAllAsync(string packageVersion, int downloadingMaxNumber, int failedTryAgain, int timeoutSeconds = 60, CancellationToken cancellationToken = default);
		Task<IDownloader> CreatePreDownloaderForTagsAsync(string packageVersion, string[] tags, int downloadingMaxNumber, int failedTryAgain, int timeoutSeconds = 60, CancellationToken cancellationToken = default);
		Task<IDownloader> CreatePreDownloaderForLocationsAsync(string packageVersion, string[] locations, bool recursiveDownload, int downloadingMaxNumber, int failedTryAgain, int timeoutSeconds = 60, CancellationToken cancellationToken = default);

		// --- Asset Loading ---
		IAssetHandle<TAsset> LoadAssetSync<TAsset>(string location) where TAsset : UnityEngine.Object;
		IAssetHandle<TAsset> LoadAssetAsync<TAsset>(string location) where TAsset : UnityEngine.Object;

		/// <summary>
		/// Loads all sub-assets for a location (e.g., sprites in an atlas).
		/// </summary>
		IAllAssetsHandle<TAsset> LoadAllAssetsAsync<TAsset>(string location) where TAsset : UnityEngine.Object;

		/// <summary>
		/// Instantiates a prefab synchronously using a previously loaded handle. Returns null on error.
		/// </summary>
		GameObject InstantiateSync(IAssetHandle<GameObject> handle, Transform parent = null, bool worldPositionStays = false);

		/// <summary>
		/// Instantiates a prefab asynchronously using a previously loaded handle.
		/// </summary>
		IInstantiateHandle InstantiateAsync(IAssetHandle<GameObject> handle, Transform parent = null, bool worldPositionStays = false, bool setActive = true);

		// --- Scene Loading ---
		ISceneHandle LoadSceneSync(string sceneLocation, LoadSceneMode loadMode = LoadSceneMode.Single);
		ISceneHandle LoadSceneAsync(string sceneLocation, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100);
		Task UnloadSceneAsync(ISceneHandle sceneHandle);

		// --- Maintenance ---
		Task UnloadUnusedAssetsAsync();
	}

	public interface IDownloader
	{
		bool IsDone { get; }
		bool Succeed { get; }
		float Progress { get; }
		int TotalDownloadCount { get; }
		int CurrentDownloadCount { get; }
		long TotalDownloadBytes { get; }
		long CurrentDownloadBytes { get; }
		string Error { get; }

		void Begin();
		Task StartAsync(CancellationToken cancellationToken = default);
		void Pause();
		void Resume();
		void Cancel();
		void Combine(IDownloader other);
	}

	public interface IOperation
	{
		bool IsDone { get; }
		float Progress { get; }
		string Error { get; }
		void WaitForAsyncComplete();
	}

	public interface IAssetHandle<out TAsset> : IOperation, IDisposable where TAsset : UnityEngine.Object
	{
		TAsset Asset { get; }
		UnityEngine.Object AssetObject { get; }
	}

	public interface IAllAssetsHandle<out TAsset> : IOperation, IDisposable where TAsset : UnityEngine.Object
	{
		IReadOnlyList<TAsset> Assets { get; }
	}

	public interface IInstantiateHandle : IOperation, IDisposable
	{
		GameObject Instance { get; }
	}

	public interface ISceneHandle : IOperation
	{
		string ScenePath { get; }
	}

	/// <summary>
	/// Global configuration for the module.
	/// </summary>
	public readonly struct AssetModuleOptions
	{
		public readonly long OperationSystemMaxTimeSliceMs;
		public readonly int BundleLoadingMaxConcurrency;
		public readonly ILogger Logger;
		public readonly bool EnableHandleTracking;

		public AssetModuleOptions(long operationSystemMaxTimeSliceMs = 16, int bundleLoadingMaxConcurrency = int.MaxValue, ILogger logger = null, bool enableHandleTracking = true)
		{
			OperationSystemMaxTimeSliceMs = operationSystemMaxTimeSliceMs < 10 ? 10 : operationSystemMaxTimeSliceMs;
			BundleLoadingMaxConcurrency = bundleLoadingMaxConcurrency;
			Logger = logger;
			EnableHandleTracking = enableHandleTracking;
		}
	}

	/// <summary>
	/// Provider-agnostic initialization parameters for a package.
	/// </summary>
	public readonly struct AssetPackageInitOptions
	{
		public readonly AssetPlayMode PlayMode;
		public readonly object ProviderOptions;
		public readonly int? BundleLoadingMaxConcurrencyOverride;

		public AssetPackageInitOptions(AssetPlayMode playMode, object providerOptions, int? bundleLoadingMaxConcurrencyOverride = null)
		{
			PlayMode = playMode;
			ProviderOptions = providerOptions;
			BundleLoadingMaxConcurrencyOverride = bundleLoadingMaxConcurrencyOverride;
		}
	}

	public enum AssetPlayMode
	{
		EditorSimulate,
		Offline,
		Host,
		Web,
		Custom
	}
}