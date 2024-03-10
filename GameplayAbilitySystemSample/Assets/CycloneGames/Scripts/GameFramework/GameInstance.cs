using UnityEngine;

namespace CycloneGames.GameFramework
{
    public interface IGameInstance
    {
        void GameStart();
    }
    public class GameInstance : MonoBehaviour, IGameInstance
    {
        private const string DEBUG_FLAG = "<color=cyan>[GameInstance]</color>";

        private void Awake()
        {
            // GameStart();
        }
        
        private void Start()
        {
            GameStart();
        }

        public void GameStart()
        {
            Debug.Log($"{DEBUG_FLAG} GameStart");
        }
    }
}