using UnityEngine;

namespace GASSample.Misc
{
    public class GASSampleEventSystem : MonoBehaviour
    {
        public static GASSampleEventSystem Instance { get; private set; }

        private bool _singleton = true;
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