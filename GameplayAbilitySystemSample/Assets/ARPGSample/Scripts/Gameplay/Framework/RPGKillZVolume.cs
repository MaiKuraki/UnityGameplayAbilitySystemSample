using CycloneGames.GameFramework;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    public class RPGKillZVolume : Actor
    {
        private readonly string DEBUG_FLAG = "<color=#FF4B4B>[KillZ Volume]</color>";
        [SerializeField] private Collider2D collision;
        protected override void Awake()
        {
            base.Awake();
            
            collision.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //  Target Actor require a 'Collision' component and 'Rigidbody' component
            Debug.Log($"{DEBUG_FLAG} {other.gameObject.name} Enter Kill Z");
            Actor otherActor = other.GetComponent<Actor>();
            otherActor.FellOutOfWorld();
        }
    }
}