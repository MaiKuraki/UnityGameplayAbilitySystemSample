using CycloneGames.Service;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CycloneGames.UIFramework
{
    public class UIFramework : MonoBehaviour
    {
        [Inject] private MainCamera gameMainCamera;
        [SerializeField] private UIRoot uiRoot;
        [SerializeField] private Camera uiCamera;
        [SerializeField] private CanvasScaler canvasScaler;
        
        public CanvasScaler UICanvasScaler => canvasScaler;
        private Transform uiRootTF;
        private Transform uiCameraTF;

        private void Awake()
        {
            uiRootTF = uiRoot.transform;
            uiCameraTF = uiCamera.transform;
        }

        private void Start()
        {
            gameMainCamera.AddNewCameraToStack(uiCamera);
        }

        private void OnDestroy()
        {
            gameMainCamera.RemoveCameraFromStack(uiCamera);
        }
    }
}