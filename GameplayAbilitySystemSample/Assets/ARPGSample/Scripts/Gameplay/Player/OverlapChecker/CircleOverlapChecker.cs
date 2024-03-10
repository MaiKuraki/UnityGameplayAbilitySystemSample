using ARPGSample.Gameplay;
using UnityEngine;

public class CircleOverlapChecker : OverlapChecker
{
    [SerializeField] private float checkRadius = 1;

    protected override void CheckOverlap()
    {
        base.CheckOverlap();
        
        isOverlapped = Physics2D.OverlapCircle((Vector2)transform.position + positionOffset, checkRadius, checkLayer);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        Gizmos.DrawWireSphere((Vector2)transform.position + positionOffset, checkRadius);
    }
}
