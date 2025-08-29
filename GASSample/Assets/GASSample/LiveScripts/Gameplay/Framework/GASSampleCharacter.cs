using CycloneGames.GameplayFramework;
using CycloneGames.RPGFoundation;
using UnityEngine;

namespace GASSample.Gameplay
{
    [RequireComponent(typeof(MovementComponent))]
    public class GASSampleCharacter : Pawn
    {
        protected MovementComponent movementComponent;
        protected MovementComponent GetMovementComponent => movementComponent;

        protected Vector3 movementVelocity = Vector3.zero;

        protected override void Awake()
        {
            base.Awake();

            movementComponent = GetComponent<MovementComponent>();
        }
        override protected void Update()
        {
            base.Update();
        }
    }
}