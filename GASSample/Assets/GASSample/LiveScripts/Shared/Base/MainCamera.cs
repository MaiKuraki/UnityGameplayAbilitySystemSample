using CycloneGames.UIFramework.Runtime;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace GASSample
{
    /// <summary>
    /// Project-owned main camera.
    /// <para>
    /// <c>CycloneGames.Services</c> was removed from the package set, so the sample keeps the two
    /// pieces it relied on: a singleton camera that survives scene transitions, and the URP
    /// overlay-stack helpers the UIFramework camera needs (its camera is authored as
    /// <see cref="CameraRenderType.Overlay"/> and renders nothing until it is stacked).
    /// </para>
    /// </summary>
    [DisallowMultipleComponent]
    public class MainCamera : MonoBehaviour
    {
        [SerializeField] private Camera cameraInst;
        [SerializeField] private bool singleton = true;

        public static MainCamera Instance { get; private set; }

        public Camera CameraInst
        {
            get
            {
                if (cameraInst == null)
                {
                    cameraInst = GetComponent<Camera>();
                }

                return cameraInst;
            }
        }

        private UniversalAdditionalCameraData cameraData;

        private UniversalAdditionalCameraData CameraData
        {
            get
            {
                if (cameraData == null && CameraInst != null)
                {
                    cameraData = CameraInst.GetUniversalAdditionalCameraData();
                }

                return cameraData;
            }
        }

        private void Awake()
        {
            if (singleton)
            {
                if (Instance != null && Instance != this)
                {
                    Destroy(gameObject);
                    return;
                }

                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            // The render stack only accepts overlays on a Base camera.
            if (CameraData != null)
            {
                CameraData.renderType = CameraRenderType.Base;
            }
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
            // The UIFramework prefab is created after this camera, and scenes come and go; re-checking
            // on every load keeps the UI overlay in the stack without an ordering contract.
            StackUICameraIfAvailable();
        }

        /// <summary>
        /// The UIFramework camera is authored as an URP overlay and renders nothing outside a stack.
        /// </summary>
        public void StackUICameraIfAvailable()
        {
            UIRoot uiRoot = FindObjectOfType<UIRoot>(true);
            if (uiRoot != null && uiRoot.UICamera != null)
            {
                AddCameraToStack(uiRoot.UICamera);
            }
        }

        public void AddCameraToStack(Camera overlayCamera)
        {
            if (overlayCamera == null || overlayCamera == CameraInst)
            {
                return;
            }

            UniversalAdditionalCameraData data = CameraData;
            if (data == null)
            {
                return;
            }

            UniversalAdditionalCameraData overlayData = overlayCamera.GetUniversalAdditionalCameraData();
            if (overlayData != null)
            {
                overlayData.renderType = CameraRenderType.Overlay;
            }

            if (!data.cameraStack.Contains(overlayCamera))
            {
                data.cameraStack.Add(overlayCamera);
            }
        }

        public void RemoveCameraFromStack(Camera overlayCamera)
        {
            if (!overlayCamera || CameraData == null)
            {
                return;
            }

            CameraData.cameraStack.Remove(overlayCamera);
        }

        public void ClearCameraStack()
        {
            CameraData?.cameraStack.Clear();
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
