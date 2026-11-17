using CycloneGames.UIFramework.Runtime;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace GASSample
{
    /// <summary>
    /// Persistent main camera. The UIFramework camera is a URP overlay and only renders once it is
    /// pushed into this camera's stack.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public class MainCamera : MonoBehaviour
    {
        public static MainCamera Instance { get; private set; }

        public Camera CameraInst { get; private set; }

        private void Awake()
        {
            CameraInst = GetComponent<Camera>();

            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            StackUICameraIfAvailable();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            // The UIFramework prefab can be created after this camera, so re-check on every load.
            StackUICameraIfAvailable();
        }

        public void StackUICameraIfAvailable()
        {
            UIRoot uiRoot = FindObjectOfType<UIRoot>(true);
            Camera overlay = uiRoot != null ? uiRoot.UICamera : null;
            if (overlay == null || overlay == CameraInst)
            {
                return;
            }

            overlay.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;

            var data = CameraInst.GetUniversalAdditionalCameraData();
            if (!data.cameraStack.Contains(overlay))
            {
                data.cameraStack.Add(overlay);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
