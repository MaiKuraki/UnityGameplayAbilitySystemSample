using CycloneGames.UIFramework.Runtime;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace GASSample
{
    /// <summary>
    /// The single persistent main camera.
    /// <para>
    /// The UIFramework camera is authored as a URP overlay, so it renders nothing until it is pushed
    /// into this camera's stack - that is what <see cref="StackUICameraIfAvailable"/> does.
    /// </para>
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
            // The UIFramework prefab is created after this camera and scenes come and go, so the
            // stack is re-checked on every load instead of relying on creation order.
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
