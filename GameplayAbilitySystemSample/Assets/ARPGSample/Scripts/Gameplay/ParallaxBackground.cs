using CycloneGames.GameFramework;
using UnityEngine;
using Zenject;

namespace ARPGSample.Gameplay
{
    public class ParallaxBackground : MonoBehaviour
    {
        [Inject] private IWorld world;

        [SerializeField] private Transform Layer_1;
        [SerializeField] private Transform Layer_2;
        [SerializeField] private Transform Layer_3;
        [SerializeField] private Transform Layer_4;

        private Vector3 originPosLayer1;
        private Vector3 originPosLayer2;
        private Vector3 originPosLayer3;
        private Vector3 originPosLayer4;

        private RPGPlayerCharacter _playerPlayerCharacter;

        private Vector3 camLastPos;
        private bool isPlayerValid = false;

        private const float parallaxFactorLayer1 = 0.15f;
        private const float parallaxFactorLayer2 = -0.05f;
        private const float parallaxFactorLayer3 = 0.1f;
        private const float parallaxFactorLayer4 = 0.05f;

        private void Start()
        {
            InitBackgroundOriginPosition();
        }

        private void Update()
        {
            if (!isPlayerValid)
            {
                GameMode GM = world.GetGameMode();
                PlayerController PC = GM?.GetPlayerController();
                RPGPlayerCharacter newPlayerPlayerCharacter = (RPGPlayerCharacter)PC?.GetPawn();
                PlayerState PS = PC?.GetPlayerState();

                if (PS != null)
                {
                    PS.OnPawnSetEvent -= ResetNewPawn;
                    PS.OnPawnSetEvent += ResetNewPawn;
                }

                if (newPlayerPlayerCharacter && newPlayerPlayerCharacter != _playerPlayerCharacter)
                {
                    _playerPlayerCharacter = newPlayerPlayerCharacter;
                    isPlayerValid = true;
                    camLastPos = _playerPlayerCharacter.GetActorLocation();
                }

                return;
            }

            if (_playerPlayerCharacter)
            {
                Vector3 moveDelta = _playerPlayerCharacter.GetActorLocation() - camLastPos;

                Layer_1.position += new Vector3(moveDelta.x * parallaxFactorLayer1, moveDelta.y * 0.05f);
                Layer_2.position += new Vector3(moveDelta.x * parallaxFactorLayer2, 0);
                Layer_3.position += new Vector3(moveDelta.x * parallaxFactorLayer3, 0);
                Layer_4.position += new Vector3(moveDelta.x * parallaxFactorLayer4, 0);

                camLastPos = _playerPlayerCharacter.GetActorLocation();
            }
        }

        void ResetNewPawn(PlayerState PS, Pawn newPawn, Pawn oldPawn)
        {
            if (newPawn && newPawn != oldPawn)
            {
                isPlayerValid = false;
                ResetBackgroundPosition();
            }
        }

        void InitBackgroundOriginPosition()
        {
            originPosLayer1 = Layer_1.localPosition;
            originPosLayer2 = Layer_2.localPosition;
            originPosLayer3 = Layer_3.localPosition;
            originPosLayer4 = Layer_4.localPosition;
        }

        void ResetBackgroundPosition()
        {
            Layer_1.localPosition = originPosLayer1;
            Layer_2.localPosition = originPosLayer2;
            Layer_3.localPosition = originPosLayer3;
            Layer_4.localPosition = originPosLayer4;
        }
    }
}
