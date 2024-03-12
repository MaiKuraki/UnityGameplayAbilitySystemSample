using CycloneGames.GameFramework;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class DeadZone : Actor
    {
        private BoxCollider2D collision;
        protected override void Awake()
        {
            base.Awake();

            collision = GetComponent<BoxCollider2D>();
            collision.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Actor otherActor = other.GetComponent<Actor>();
            RPGPlayerCharacter playerCharacter = (RPGPlayerCharacter)otherActor;
            playerCharacter?.Die();
        }
    }
}