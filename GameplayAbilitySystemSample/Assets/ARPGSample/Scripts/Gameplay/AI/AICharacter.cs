using System.Collections.Generic;
using AbilitySystem.Authoring;
using CycloneGames.GameFramework;
using CycloneGames.Service;
using CycloneGames.UIFramework;
using Cysharp.Threading.Tasks;
using GameplayTag.Authoring;
using MessagePipe;
using ARPGSample.UI;
using UnityEngine;
using Zenject;

namespace ARPGSample.Gameplay
{
    public class AICharacter : Pawn
    {
        private static readonly string DEBUG_FLAG = "[AICharacter]";

        [Inject] private IPublisher<UIMessage> uiMsgPub;
        [Inject] private MainCamera gameMainCamera;
        [Inject] private UIFramework uiFramework;
        
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private CapsuleCollider2D collider;
        [SerializeField] private Transform uiAttachNode;
        [SerializeField] protected AnimationFSM animationFSM;
        [SerializeField] private RPGAbilitySystemComponent abilitySystemComponent;
        [SerializeField] private CharacterAttributeSet attributeSet;
        [SerializeField] private AbstractAbilityScriptableObject[] InitialisationAbilities;
        private HashSet<GameplayTagScriptableObject> AIAbilityTags = new HashSet<GameplayTagScriptableObject>();

        public RPGAbilitySystemComponent RPGAbilitySystemComponent => abilitySystemComponent;
        public CharacterAttributeSet AttributeSet => attributeSet;
        public Rigidbody2D RB => rb;
        public CapsuleCollider2D CharacterCollider => collider;
        public bool IsInAir => isInAir;
        private bool isInAir;
        
        private Vector2 UIScreenOffset;
        private float uiScaleX = 1;
        
        protected override void Start()
        {
            base.Start();
            
            ActivateInitialisationAbilities().Forget();
            
            UIScreenOffset = new Vector2(-Screen.width / 2.0f, -Screen.height / 2.0f);
            uiScaleX = uiFramework.UICanvasScaler.referenceResolution.x / (float)Screen.width;
        }

        protected override void Update()
        {
            base.Update();
            
            uiMsgPub.Publish(new UIMessage()
            {
                MessageCode = RPGUIMessage.REFRESH_ENEMY_HEALTH_BAR_LOCATION,
                Params = new object[]
                {
                    this, 
                    ((Vector2)gameMainCamera.GameCamera.WorldToScreenPoint(uiAttachNode.position) + UIScreenOffset) * uiScaleX
                }
            });
        }

        public float GetHealth()
        {
            if (abilitySystemComponent.AttributeSystem.GetAttributeValue(AttributeSet.Health, out var val))
            {
                return val.CurrentValue;
            }

            return 0;
        }

        public float GetHealthMax()
        {
            if (abilitySystemComponent.AttributeSystem.GetAttributeValue(AttributeSet.HealthMax, out var val))
            {
                return val.CurrentValue;
            }

            return 0;
        }
        
        public float GetMovementSpeed()
        {
            if (abilitySystemComponent.AttributeSystem.GetAttributeValue(AttributeSet.MovementSpeed, out var val))
            {
                return val.CurrentValue;
            }

            return 0;
        }
        
        async UniTask ActivateInitialisationAbilities()
        {
            if (!RPGAbilitySystemComponent)
            {
                Debug.LogError($"{DEBUG_FLAG} Invalid AbilitySystem");
                return;
            }

            foreach (var ability in InitialisationAbilities)
            {
                AIAbilityTags.Add(ability.AbilityTags.AssetTag);
                var spec = ability.CreateSpec(RPGAbilitySystemComponent);
                RPGAbilitySystemComponent.GrantAbility(spec);
                StartCoroutine(spec.TryActivateAbility());
            }

            await UniTask.DelayFrame(1);
        }
        
        void ClearAbilities()
        {
            foreach (GameplayTagScriptableObject abilityTag in AIAbilityTags)
            {
                RPGAbilitySystemComponent?.RemoveAbilitiesWithTag(abilityTag);
            }
            
            RPGAbilitySystemComponent?.AttributeSystem.ResetAttributeModifiers();
            RPGAbilitySystemComponent?.AttributeSystem.ResetAll();
        }

        protected virtual void Die()
        {
            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            ClearAbilities();
        }
    }
}

