using UnityEngine;

namespace CycloneGames.UIFramework
{
    [CreateAssetMenu(menuName = "CycloneGames/UIFramework/UIPage")]
    [System.Serializable]
    public class UIPageConfiguration : ScriptableObject
    {
        //TODO: maybe its no need, we load prefab from other AsseteManagement service
        [SerializeField] private UIPage pagePrefab;
        [SerializeField] private UILayerConfirguration layer;

        public UIPage PagePrefab => pagePrefab;
        public UILayerConfirguration Layer => layer;
    }
}