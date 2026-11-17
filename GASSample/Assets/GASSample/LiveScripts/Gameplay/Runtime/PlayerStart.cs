using UnityEngine;

namespace GASSample.Gameplay
{
    /// <summary>Where the player character spawns.</summary>
    public class PlayerStart : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.9f, 1f, 0.7f);
            Vector3 pivot = transform.position + Vector3.up * 0.9f;
            Gizmos.DrawWireSphere(pivot, 0.3f);
            Gizmos.DrawRay(pivot, transform.forward * 1.5f);
        }
    }
}
