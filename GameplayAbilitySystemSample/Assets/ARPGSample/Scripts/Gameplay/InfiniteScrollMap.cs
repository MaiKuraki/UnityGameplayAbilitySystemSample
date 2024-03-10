using CycloneGames.GameFramework;
using UnityEngine;
using Zenject;

namespace ARPGSample.Gameplay
{
    public class InfiniteScrollMap : MonoBehaviour
    {
        [Inject] private IWorld world;

        private float ScaleX = 1;
        private float mapWidth;
        private float totalWidth;
        private bool isPlayerValid = false;
        private float halfWidth;

        private Vector3 originPos;
        private RPGPawn playerPawn;
        
        private void Start()
        {
            InitBackgroundOriginPosition();
            mapWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;

            ScaleX = transform.localScale.x;
            totalWidth = mapWidth * ScaleX;
            halfWidth = totalWidth * 0.5f;
        }

        private void Update()
        {
            if (!isPlayerValid)
            {
                GameMode GM = world.GetGameMode();
                PlayerController PC = GM?.GetPlayerController();
                RPGPawn newPlayerPawn = (RPGPawn)PC?.GetPawn();
                PlayerState PS = PC?.GetPlayerState();

                if (PS != null)
                {
                    PS.OnPawnSetEvent -= ResetNewPawn;
                    PS.OnPawnSetEvent += ResetNewPawn;
                }

                if (newPlayerPawn && newPlayerPawn != playerPawn)
                {
                    playerPawn = newPlayerPawn;
                    isPlayerValid = true;
                }

                return;
            }

            if (playerPawn)
            {
                float delta = playerPawn.GetActorLocation().x - transform.position.x;
                if (Mathf.Abs(delta) > halfWidth)
                {
                    Vector3 tempPos = transform.position + new Vector3(Mathf.Sign(delta) * totalWidth, 0, 0);
                    transform.position = tempPos;
                }
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

        void ResetBackgroundPosition()
        {
            transform.position = originPos;
        }

        void InitBackgroundOriginPosition()
        {
            originPos = transform.position;
        }
    }
}
