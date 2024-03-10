using System;
using System.Collections.Generic;
using CycloneGames.GameFramework;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    public class SimpleMeleeAttackCollider : Actor
    {
        [SerializeField] private Actor HitFXPrefab;
        [SerializeField] private float HitEffectScale = 1;

        private Actor hitfxInst;
        private HashSet<GameObject> collisionGameobjects = new HashSet<GameObject>();
        public event Action<GameObject> OnCollisionEnter;
        protected override void Awake()
        {
            base.Awake();
            collisionGameobjects = new HashSet<GameObject>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!collisionGameobjects.Add(other.gameObject)) return;
            
            // Debug.Log($"Collision: {other.gameObject.name}, hitCount: {other.contactCount}");
            hitfxInst = Instantiate(HitFXPrefab).GetComponent<Actor>();
            hitfxInst.transform.localScale = new Vector3(HitEffectScale, HitEffectScale, 1);
            var ct = other.GetContact(0);
            hitfxInst.SetActorPosition(ct.point);
            OnCollisionEnter?.Invoke(other.gameObject);
        }
        
        private void OnDisable()
        {
            collisionGameobjects.Clear();
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (collisionGameobjects.Contains(other.gameObject))
            {
                collisionGameobjects.Remove(other.gameObject);
            }
        }
    }
}