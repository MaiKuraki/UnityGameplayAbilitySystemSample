using UnityEngine;

public class EventSystem : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
