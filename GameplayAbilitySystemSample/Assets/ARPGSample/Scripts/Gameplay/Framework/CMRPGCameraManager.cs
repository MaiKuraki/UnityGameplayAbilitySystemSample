using Cinemachine;
using CycloneGames.GameFramework;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    public class CMRPGCameraManager : CameraManager
    {
        [SerializeField] private NoiseSettings shakeProfile;
        
        private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = null;

        public void CameraShake(float intensity, float shakeTime)
        {
            if (!VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>())
            {
                cinemachineBasicMultiChannelPerlin =
                    VirtualCamera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }

            cinemachineBasicMultiChannelPerlin =
                VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_NoiseProfile = shakeProfile;
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
            DelayResetCameraShake((int)(shakeTime * 1000)).Forget();
        }

        async UniTask DelayResetCameraShake(int milliSecond)
        {
            await UniTask.Delay(milliSecond);
            if (cinemachineBasicMultiChannelPerlin)
            {
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            }
        }
    }
}