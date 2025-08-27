using UnityEngine;

namespace GASSample.Misc
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(transform);
        }
    }
}