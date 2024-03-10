using UnityEngine;
using Zenject;

namespace CycloneGames.Service
{
    public interface IServiceDisplay
    {
        Transform ServiceDisplayTransform { get; }
    }

    public class ServiceDisplay : IServiceDisplay, IInitializable
    {
        public Transform ServiceDisplayTransform => serviceGameObject?.transform;

        private ServiceGameObject serviceGameObject;

        public void Initialize()
        {
            string serviceGameObjectName = "Service";
            GameObject serviceDisplayGo = GameObject.Find(serviceGameObjectName);
            if (!serviceDisplayGo) serviceDisplayGo = new GameObject(serviceGameObjectName);
            serviceGameObject = serviceDisplayGo.AddComponent<ServiceGameObject>();
        }
    }

    public class ServiceGameObject : MonoBehaviour
    {
        public Transform Transform => transform;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}