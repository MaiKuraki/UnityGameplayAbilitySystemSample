using CycloneGames.AssetManagement.Runtime;
using Cysharp.Threading.Tasks;
using GASSample.AssetManagement;
using UnityEngine.SceneManagement;

namespace GASSample
{
    /// <summary>
    /// Switches scenes without ever showing the gap in between.
    /// <para>
    /// The caller covers the screen with <see cref="ScreenFade.FadeOutAsync"/> first, hides whatever
    /// belonged to the outgoing scene, and only then calls <see cref="LoadAsync"/>. The incoming scene
    /// fades back in itself once it is actually ready - the title window is open, or the world and its
    /// player exist - because fading in right after the load would show a few frames of empty sky.
    /// </para>
    /// </summary>
    public static class SceneFlow
    {
        public const string Title = "Assets/GASSample/LiveContent/Scenes/Scene_Title.unity";
        public const string Gameplay = "Assets/GASSample/LiveContent/Scenes/Scene_Gameplay.unity";

        private static ISceneHandle currentScene;

        /// <summary>Loads the scene while the screen is already covered.</summary>
        public static async UniTask LoadAsync(IAssetModule assetModule, string sceneLocation)
        {
            var sceneLoader = (IAssetSceneLoader)assetModule.GetPackage(AssetPackageName.DefaultPackage);

            ISceneHandle nextScene = sceneLoader.LoadSceneAsync(sceneLocation, LoadSceneMode.Single, activateOnLoad: true);
            await nextScene.Task;

            // Single mode already dropped the previous scene when this one activated; unloading through
            // the loader only retires the old handle. Disposing it directly would log a "caller
            // ownership only" warning and leave the scene for someone else to unload.
            if (currentScene != null)
            {
                await sceneLoader.UnloadSceneAsync(currentScene);
            }

            currentScene = nextScene;
        }
    }
}
