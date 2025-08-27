using UnityEngine;

namespace GASSample.UI
{
    public class UIFramework : MonoBehaviour
    {
        public static UIFramework Instance { get; private set; }
        [SerializeField] private bool _singleton = true;

        void Awake()
        {
            if (_singleton)
            {
                if (Instance != null && Instance != this)
                {
                    Destroy(gameObject);
                    return;
                }

                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}