using UnityEngine;

namespace CycloneGames.GameFramework
{
    public class PlayerStart : Actor
    {
        [SerializeField] private Transform Arrow;

        protected override void Awake()
        {
            base.Awake();
            
            if(Arrow && Arrow.gameObject) Arrow.gameObject.SetActive(false);
        }
    }
}