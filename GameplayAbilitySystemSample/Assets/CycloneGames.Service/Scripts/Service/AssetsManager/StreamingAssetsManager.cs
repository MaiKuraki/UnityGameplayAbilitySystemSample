using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace CycloneGames.Service
{
    public class StreamingAssetsManager : MonoBehaviour
    {
        private const string DEBUG_FLAG = "[StreamingAssets Manager]";

        private void Awake()
        {
            // Initialize BetterStreamingAssets if not on the WebGL platform
#if !UNITY_WEBGL || UNITY_EDITOR
            BetterStreamingAssets.Initialize();
#endif
        }

        // Asynchronous method to load bytes with platform-specific logic
        public async UniTask<byte[]> LoadBytesAsync(string path, CancellationToken cancellationToken = default)
        {
            // The cancellationToken can be used to cancel the ongoing load operation
#if UNITY_WEBGL && !UNITY_EDITOR
            return await LoadBytesWebGLAsync(path, cancellationToken);
#else
            return await LoadBytesOtherAsync(path, cancellationToken);
#endif
        }

        // WebGL specific loading method using UnityWebRequest
        private async UniTask<byte[]> LoadBytesWebGLAsync(string path, CancellationToken cancellationToken)
        {
            string url = System.IO.Path.Combine(Application.streamingAssetsPath, path);
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Associating SendWebRequest with the cancellationToken
                await request.SendWebRequest().ToUniTask(cancellationToken: cancellationToken);

                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError($"{DEBUG_FLAG} WebGL Load Bytes Error: {request.error}");
                    throw new InvalidOperationException($"{DEBUG_FLAG} WebGL Load Bytes Error: {request.error}");
                }

                if (request.downloadHandler.data.Length == 0)
                {
                    // Ensure that data was actually received
                    throw new InvalidOperationException($"{DEBUG_FLAG} WebGL Load Bytes Error: No data received");
                }

                return request.downloadHandler.data;
            }
        }

        // Loading method for other platforms
        private async UniTask<byte[]> LoadBytesOtherAsync(string path, CancellationToken cancellationToken)
        {
            return await UniTask.RunOnThreadPool(() =>
            {
                if (!BetterStreamingAssets.FileExists(path))
                {
                    Debug.LogError($"{DEBUG_FLAG} File not found: {path}");
                    throw new System.IO.FileNotFoundException($"{DEBUG_FLAG} File not found: {path}");
                }

                return BetterStreamingAssets.ReadAllBytes(path);
            }, cancellationToken: cancellationToken);
        }

        // Asynchronous method to load a Texture2D
        public async UniTask<Texture2D> LoadTextureAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                byte[] data = await LoadBytesAsync(path, cancellationToken);
                // Initialize Texture2D with the correct dimensions (temporary size, LoadImage will replace it)
                Texture2D texture = new Texture2D(1, 1); // Temporary size, LoadImage will replace it
                if (texture.LoadImage(data))
                {
                    return texture;
                }
                else
                {
                    // If texture loading fails, release the texture and report an error
                    Destroy(texture);
                    throw new InvalidOperationException(
                        $"{DEBUG_FLAG} Load Texture Error: Failed to load texture from bytes.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"{DEBUG_FLAG} Load Texture Error: {e.Message}");
                throw; // Rethrow the exception to allow handling further up the call stack
            }
        }
    }
}
