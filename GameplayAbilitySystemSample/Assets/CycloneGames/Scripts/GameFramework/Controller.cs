using UnityEngine;
using Zenject;

namespace CycloneGames.GameFramework
{
    public class Controller : Actor
    {
        [Inject] private DiContainer DiContainer;
        [Inject] private IWorldSettings WorldSettings;
        
        private Actor StartSpot;
        private Pawn pawn;
        private PlayerState playerState;
        private Quaternion controlRotation = Quaternion.identity;

        public Pawn GetDefaultPawnPrefab() => WorldSettings.PawnClass;
        public void SetInitialLocationAndRotation(Vector3 NewLocation, Quaternion NewRotation)
        {
            transform.position = NewLocation;
            SetControlRotation(NewRotation);
        }
        public void SetStartSpot(Actor NewStartSpot)
        {
            StartSpot = NewStartSpot;
        }
        
        public Pawn GetPawn()
        {
            return pawn;
        }
        public void SetPawn(Pawn InPawn)
        {
            pawn = InPawn;
        }

        public virtual void Possess(Pawn InPawn)
        {
            OnPossess(InPawn);
        }

        public virtual void OnPossess(Pawn InPawn)
        {
            bool bNewPawn = GetPawn() != InPawn;

            if (bNewPawn && GetPawn() != null)
            {
                UnPossess();
            }

            if (InPawn.Controller != null)
            {
                InPawn.Controller.UnPossess();
            }
            
            InPawn.PossessedBy(this);
            SetPawn(InPawn);
            
            SetControlRotation(GetPawn().GetActorRotation());
            GetPawn().DispatchRestart();
        }

        public virtual void UnPossess()
        {
            if (!GetPawn())
            {
                return;
            }
            
            OnUnPossess();
        }
        
        public virtual void OnUnPossess()
        {
            if (GetPawn())
            {
                GetPawn().UnPossessed();
                SetPawn(null);
            }
        }

        public PlayerState GetPlayerState()
        {
            return playerState;
        }

        public T GetPlayerState<T>() where T : PlayerState
        {
            return playerState is T ps ? ps : null;
        }

        protected void InitPlayerState()
        {
            var psGo = DiContainer.InstantiatePrefab(WorldSettings.PlayerStateClass);
            if (psGo == null)
            {
                Debug.LogError("Spawn PlayerState Failed, please check prefab");
            }
            playerState = psGo.GetComponent<PlayerState>();
            DiContainer.BindInstance(playerState).AsCached();
        }

        public void SetControlRotation(Quaternion NewRotation)
        {
            if (!controlRotation.Equals(NewRotation))
            {
                controlRotation = NewRotation;
            }
        }

        public Quaternion ControlRotation() => controlRotation;
    }
}