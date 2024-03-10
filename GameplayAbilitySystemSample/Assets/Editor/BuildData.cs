using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace CycloneGames.Editor.Build
{
    [CreateAssetMenu(menuName = "CycloneGames/Build/BuildData")]
    public class BuildData : ScriptableObject
    {
        [FormerlySerializedAs("buildSceneBaseFolder")]
        [Header("------ Build Scene Config ------")]
        [SerializeField] private string buildSceneBasePath;

        [SerializeField] private SceneAsset launchScene;
    
        public SceneAsset LaunchScene => launchScene;
        public string BuildSceneBasePath => buildSceneBasePath;
    }
}