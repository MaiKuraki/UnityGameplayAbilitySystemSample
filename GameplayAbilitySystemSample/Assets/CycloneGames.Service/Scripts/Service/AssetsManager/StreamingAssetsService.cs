using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

namespace CycloneGames.Service
{
    public interface IStreamingAssetsService
    {
        UniTask<byte[]> LoadBytesAsync(string path);
        UniTask<Texture2D> LoadTextureAsync(string path);
        bool IsServiceReady();
    }
    
    public class StreamingAssetsService : IInitializable, IStreamingAssetsService
    {
        private const string DEBUG_FLAG = "[StreamingAssetsService]";
        [Inject] private IServiceDisplay serviceDisplay;
        
        private GameObject streamingAssetManagerGO;
        private StreamingAssetsManager _streamingAssetsManager;
        
        public void Initialize()
        {
            streamingAssetManagerGO = new GameObject("StreamingAssetsManager");
            streamingAssetManagerGO.transform.SetParent(serviceDisplay.ServiceDisplayTransform);
            _streamingAssetsManager = streamingAssetManagerGO.AddComponent<StreamingAssetsManager>();
        }


        public async UniTask<byte[]> LoadBytesAsync(string path)
        {
            if (_streamingAssetsManager == null)
            {
                throw new System.InvalidOperationException($"{DEBUG_FLAG} StreamingAssetsManager is not initialized.");
            }

            try
            {
                return await _streamingAssetsManager.LoadBytesAsync(path);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"{DEBUG_FLAG} Failed to load bytes from path {path}: {ex}");
                throw; // Rethrows the current exception
            }
        }

        public async UniTask<Texture2D> LoadTextureAsync(string path)
        {
            if (_streamingAssetsManager == null)
            {
                throw new System.InvalidOperationException($"{DEBUG_FLAG} StreamingAssetsManager is not initialized.");
            }

            try
            {
                return await _streamingAssetsManager.LoadTextureAsync(path);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"{DEBUG_FLAG} Failed to load texture from path {path}: {ex}");
                throw; // Rethrows the current exception
            }
        }
        
        public bool IsServiceReady()
        {
            return _streamingAssetsManager != null;
        }
    }
}
