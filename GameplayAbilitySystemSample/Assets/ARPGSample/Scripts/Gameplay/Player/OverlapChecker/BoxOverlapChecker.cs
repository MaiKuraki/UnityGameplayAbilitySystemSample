using ARPGSample.Gameplay;
using UnityEngine;

public class BoxOverlapChecker : OverlapChecker
{
    [SerializeField] private Vector2 boxSize = Vector2.one;
    protected override void CheckOverlap()
    {
        base.CheckOverlap();
        
        isOverlapped  = Physics2D.OverlapBox((Vector2)transform.position + positionOffset, boxSize, 0, checkLayer);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        
        Gizmos.DrawWireCube((Vector2)transform.position + positionOffset,boxSize);
    }
}
