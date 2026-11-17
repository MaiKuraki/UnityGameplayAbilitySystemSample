using CycloneGames.AssetManagement.Runtime;
using Cysharp.Threading.Tasks;
using GASSample.AssetManagement;
using UnityEngine.SceneManagement;

namespace GASSample
{
    /// <summary>
    /// Loads the next scene while the screen is covered.
    /// <para>
    /// The caller fades out and closes its own UI first. The loaded scene fades back in when it is
    /// ready, so the swap never shows an empty frame.
    /// </para>
    /// </summary>
    public static class SceneFlow
    {
        public const string Title = "Assets/GASSample/LiveContent/Scenes/Scene_Title.unity";
        public const string Gameplay = "Assets/GASSample/LiveContent/Scenes/Scene_Gameplay.unity";

        private static ISceneHandle currentScene;

        public static async UniTask LoadAsync(IAssetModule assetModule, string sceneLocation)
        {
            var sceneLoader = (IAssetSceneLoader)assetModule.GetPackage(AssetPackageName.DefaultPackage);

            ISceneHandle nextScene = sceneLoader.LoadSceneAsync(sceneLocation, LoadSceneMode.Single, activateOnLoad: true);
            await nextScene.Task;

            // Single mode already unloaded the previous scene; this call only retires its handle.
            // Disposing the handle directly logs an ownership warning.
            if (currentScene != null)
            {
                await sceneLoader.UnloadSceneAsync(currentScene);
            }

            currentScene = nextScene;
        }
    }
}
