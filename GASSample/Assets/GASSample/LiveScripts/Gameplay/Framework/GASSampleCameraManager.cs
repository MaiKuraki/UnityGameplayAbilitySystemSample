using CycloneGames.GameplayFramework;
using UnityEngine;

namespace GASSample.Gameplay
{
    public class GASSampleCameraManager : CameraManager
    {
        [SerializeField] private Animator animator;
        public Animator GetAnimator
        {
            get { return animator; }
        }
    }
}
