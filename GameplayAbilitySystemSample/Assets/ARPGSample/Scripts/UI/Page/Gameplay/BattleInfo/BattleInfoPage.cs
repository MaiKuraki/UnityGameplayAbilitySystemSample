using System;
using System.Collections.Generic;
using CycloneGames.GameFramework;
using CycloneGames.UIFramework;
using MessagePipe;
using UnityEngine;
using Zenject;

namespace ARPGSample.UI
{
    public class BattleInfoPage : UIPage
    {
        private static readonly string DEBUG_FLAG = "[BattleInfoPage]";

        [Inject] private ISubscriber<UIMessage> uiMsgSub;
        
        [SerializeField] private SimpleEnemyHealthBar simpleEnemyHealthBarPrefab;
        private Dictionary<Pawn, SimpleEnemyHealthBar> healthBarDic = new Dictionary<Pawn, SimpleEnemyHealthBar>();
        
        IDisposable disposableSubscribe;
        
        protected override void Awake()
        {
            base.Awake();
            
            var subscribeHandler = uiMsgSub.Subscribe(msg =>
            {
                if (msg.Params != null && msg.Params.Length > 0)
                {
                    if (msg.MessageCode == RPGUIMessage.REFRESH_ENEMY_HEALTH_BAR_LOCATION)
                    {
                        if (msg.Params is { Length: > 0 } && msg.Params[0] is Pawn ownerPawn && msg.Params[1] is Vector2 screenLocation)
                        {
                            UpdatePosition(ownerPawn, screenLocation);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Invalid Params");
                }
            });
            
            disposableSubscribe = DisposableBag.Create(subscribeHandler);
        }

        public void AddEnemyHealthBar(Pawn ownerPawn, float newInitialHealthVal)
        {
            if (ownerPawn == null)
            {
                Debug.LogError($"{DEBUG_FLAG} Pawn is null.");
                return;
            }

            if (healthBarDic.ContainsKey(ownerPawn))
            {
                Debug.LogError($"{DEBUG_FLAG} Duplicated Add Pawn");
                return;
            }

            var healthBar = Instantiate(simpleEnemyHealthBarPrefab);
            healthBar.transform.SetParent(transform, false); // Set the parent of the health bar for UI organization.
            healthBarDic.Add(ownerPawn, healthBar);
            RefreshHealthBar(ownerPawn, newInitialHealthVal);
        }

        public void RefreshHealthBar(Pawn ownerPawn, float newHealthVal)
        {
            if (healthBarDic.TryGetValue(ownerPawn, out SimpleEnemyHealthBar enemyHealthBar))
            {
                enemyHealthBar.SetHealthBarVal(newHealthVal);
            }
        }

        public void RemoveHealthBar(Pawn ownerPawn)
        {
            if (ownerPawn == null)
            {
                Debug.LogError($"{DEBUG_FLAG} Pawn is null.");
                return;
            }

            if (healthBarDic.TryGetValue(ownerPawn, out SimpleEnemyHealthBar enemyHealthBar))
            {
                Destroy(enemyHealthBar.gameObject); // Destroy the health bar game object
                healthBarDic.Remove(ownerPawn); // Then remove the entry from the dictionary
            }
            else
            {
                Debug.LogWarning($"{DEBUG_FLAG} Trying to remove non-existing health bar for pawn.");
            }
        }

        public void ClearAllHealthBar()
        {
            foreach (var kvp in healthBarDic)
            {
                Destroy(kvp.Value.gameObject);
            }
            healthBarDic.Clear();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            disposableSubscribe?.Dispose();
        }

        void UpdatePosition(Pawn ownerPawn, Vector2 screenLocation)
        {
            if (healthBarDic.TryGetValue(ownerPawn, out SimpleEnemyHealthBar healthBar))
            {
                healthBar.transform.localPosition = screenLocation;
            }
        }
    }
}