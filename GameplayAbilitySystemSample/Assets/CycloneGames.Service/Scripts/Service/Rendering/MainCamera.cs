using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CycloneGames.Service
{
    public class MainCamera : MonoBehaviour
    {
        private const string DEBUG_FLAG = "[Main Camera]";
        private Camera _camera;
        private UniversalAdditionalCameraData _urpCameraData;
        public Camera GameCamera => _camera;
        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _urpCameraData = _camera?.GetUniversalAdditionalCameraData();
            
            if (CheckCameraValid())
            {
                // 可以进行其他的初始化
            }
        }

        private bool CheckCameraValid()
        {
            int mainCameraAmount = GameObject.FindGameObjectsWithTag("MainCamera").Length;
            if (mainCameraAmount != 1)
            {
                Debug.LogError($"{DEBUG_FLAG} There are multiple MainCamera in the scene, please check.");
                return false;
            }
            
            if (!_camera)
            {
                Debug.LogError($"{DEBUG_FLAG} Invalid Camera, please ensure this script is attached to a GameObject that contains a Camera component.");
                return false;
            }

            if (!_urpCameraData)
            {
                Debug.LogError($"{DEBUG_FLAG} Invalid URPCameraData");
                return false;
            }

            if (_camera != Camera.main)
            {
                Debug.LogError($"{DEBUG_FLAG} This is not the Main Camera, GameObject name: {gameObject.name}");
                return false;
            }
            
            return true;
        }

        public void AddNewCameraToStack(Camera newCamera)
        {
            if (newCamera != null)
            {
                if (!_urpCameraData.cameraStack.Contains(newCamera))
                {
                    _urpCameraData.cameraStack.Add(newCamera);
                    Debug.Log($"{DEBUG_FLAG} add new camera to CameraStack, name: {newCamera.name}");
                }
                else
                {
                    Debug.LogWarning($"{DEBUG_FLAG} Camera already in stack, name: {newCamera.name}");
                }
            }
        }

        public void RemoveCameraFromStack(Camera cameraToRemove)
        {
            if (cameraToRemove && _urpCameraData.cameraStack.Contains(cameraToRemove))
            {
                _urpCameraData.cameraStack.Remove(cameraToRemove);
            }
        }
    }
}
