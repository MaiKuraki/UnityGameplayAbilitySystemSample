using UnityEngine;

namespace CycloneGames.GameFramework
{
    [RequireComponent(typeof(BoxCollider))]
    public class KillZVolume : Actor
    {
        private const string DEBUG_FLAG = "<color=#FF4B4B>[KillZ Volume]</color>";
        private BoxCollider collision;
        protected override void Awake()
        {
            base.Awake();
            collision = GetComponent<BoxCollider>();
            collision.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            //  Target Actor require a 'Collision' component and 'Rigidbody' component
            Debug.Log($"{DEBUG_FLAG} {other.gameObject.name} Enter Kill Z");
            Actor otherActor = other.GetComponent<Actor>();
            otherActor.FellOutOfWorld();
        }
    }
}