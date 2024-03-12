using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Authoring;
using ARPGSample.UI;
using CycloneGames.GameFramework;
using CycloneGames.UIFramework;
using Cysharp.Threading.Tasks;
using GameplayTag.Authoring;
using MessagePipe;
using UnityEngine;
using Zenject;

namespace ARPGSample.Gameplay
{
    public class RPGPlayerCharacter : Pawn
    {
        private static readonly string DEBUG_FLAG = "[RPGPlayerCharacter]";

        public enum EAttackType
        {
            Invalid,
            Attack_0,
            Attack_1,
        }

        [System.Serializable]
        public class AbilityConfig
        {
            public int attackID;
            public AbstractAbilityScriptableObject Ability;
        }
        
        [Inject] private IWorld world;
        [Inject] private IPublisher<UIMessage> uiMsgPub;

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private OverlapChecker groundCheck;
        [SerializeField] private PlayerAnimationFSM animationFsm;
        [SerializeField] private AbilityConfig[] AbilityConfigs;
        [SerializeField] private AbstractAbilityScriptableObject[] InitialisationAbilities;
        [SerializeField] private GameplayEffectScriptableObject DeathEffect;
        [SerializeField] private CapsuleCollider2D collider;
        [SerializeField] private AudioSource audioSource;
        
        private AbstractAbilitySpec[] abilitySpecs;
        private HashSet<GameplayTagScriptableObject> AbilityTags = new HashSet<GameplayTagScriptableObject>();
        public Rigidbody2D RB => rb;
        internal bool IsInAir => !groundCheck.IsOverlapped;
        CameraManager cameraManager;
        private GameMode GM;
        private RPGPlayerState savedPS;

        public int ComboWindowMilliSecond { get; } = 150;
        private int faceDir = 1;
        private Vector2 moveInputVec;
        private static float axisDeadzone = 0.02f;

        private IAttackState currentAttackingState;
        public IAttackState CurrentAttackingState => currentAttackingState;
        public int invalidAttackID = -1;
        private Dictionary<int, int> ComboLibrary_Attack0 = new Dictionary<int, int>()
        {
            {1,2},
            {2,3}
        };
        private Dictionary<int, int> ComboLibrary_Attack1 = new Dictionary<int, int>()
        {
            {2,4},
            {3,4},
            {4,5}
        };

        public PlayerAnimationFSM AnimationFsm => animationFsm;


        private RPGAbilitySystemComponent GetAbilitySystem()
        {
            return savedPS != null ? savedPS.ASC : null;
        }

        protected override void Awake()
        {
            base.Awake();

            cameraManager = world.GetPlayerController().GetCameraManager();
            cameraManager.SetViewTarget(this);
        }

        protected override void Start()
        {
            base.Start();

            animationFsm.SetOwner(this);
        }

        protected override void Update()
        {
            base.Update();

            currentAttackingState?.OnUpdate(this);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            UpdateMove();
        }

        protected virtual void LateUpdate()
        {
            RefreshAttributesUI();
        }

        public override void PossessedBy(Controller NewController)
        {
            base.PossessedBy(NewController);

            savedPS = NewController.GetPlayerState<RPGPlayerState>();
            ActivateInitialisationAbilities().Forget();
            GrantCastableAbilities();
        }

        public override void UnPossessed()
        {
            base.UnPossessed();

            ClearAbilities();
        }

        void ClearAbilities()
        {
            foreach (GameplayTagScriptableObject abilityTag in AbilityTags)
            {
                GetAbilitySystem()?.RemoveAbilitiesWithTag(abilityTag);
            }
            
            GetAbilitySystem()?.AttributeSystem.ResetAttributeModifiers();
            GetAbilitySystem()?.AttributeSystem.ResetAll();

            // foreach (GameplayTagScriptableObject abilityTag in AbilityTags)
            // {
            //     AbilityTags.Remove(abilityTag);
            // }
        }

        public void MoveInput(Vector2 inputVec)
        {
            //  TODO: maybe move to PlayerController
            float inputVecX = inputVec.x;
            if (inputVecX > axisDeadzone)
            {
                inputVec.x = 1;
                
            }
            else if (inputVecX < -axisDeadzone)
            {
                inputVec.x = -1;
            }
            else
            {
                inputVec.x = 0;
            }

            moveInputVec = inputVec;
        }

        void UpdateMove()
        {
            if (savedPS)
            {
                if (!IsInAttacking)
                {
                    if (moveInputVec.x > axisDeadzone)
                    {
                        faceDir = 1;
                    }
                    else if (moveInputVec.x < -axisDeadzone)
                    {
                        faceDir = -1;
                    }
                    rb.velocity = new Vector2(moveInputVec.x * savedPS.GetMovementSpeed() * Time.fixedDeltaTime,
                        rb.velocity.y);
                    transform.localScale = new Vector3(faceDir, 1, 1);
                }
                else
                {
                    //  NOTE: we Modify the movement speed in Ability
                    rb.velocity = new Vector2(faceDir * savedPS.GetMovementSpeed() * Time.fixedDeltaTime, rb.velocity.y);
                }
            }
        }

        public void Jump()
        {
            if (savedPS)
            {
                if (groundCheck.IsOverlapped)
                {
                    ChangeAttackingState(new BreakAttackState());
                    RB.AddForce(transform.up * savedPS.GetJumpForce(), ForceMode2D.Impulse);
                }
            }
        }

        public void UpdatePhysicsMaterial()
        {
            
        }

        bool CanAttack()
        {
            return !IsInAttacking;  // TODO: sometimes, break state also return false
        }
        public void Attack_0()
        {
            if (!IsInAir)
            {
                if (!IsInComboWindow && !IsInAttacking)
                {
                    var firstEntry = ComboLibrary_Attack0.ElementAt(0);
                    int defaultID = firstEntry.Key;
                    ChangeAttackingState(new AttackingState(defaultID, EAttackType.Attack_0));
                }
                else if (CanAttack())
                {
                    var comboState = (AttackComboWindowState)currentAttackingState;
                    int nextAttackID = GetNextAttackID(comboState.HandledAttackID, EAttackType.Attack_0);
                    ChangeAttackingState(new AttackingState(nextAttackID, EAttackType.Attack_0));
                }
            }
            else
            {
                //  TODO: Trigger Air Attack
            }
        }
        public void Attack_1()
        {
            if (!IsInAir)
            {
                if (!IsInComboWindow)
                {
                    var firstEntry = ComboLibrary_Attack1.ElementAt(0);
                    int defaultID = firstEntry.Key;
                    ChangeAttackingState(new AttackingState(defaultID, EAttackType.Attack_1));
                }
                else if (CanAttack())
                {
                    var comboState = (AttackComboWindowState)currentAttackingState;
                    int nextAttackID = GetNextAttackID(comboState.HandledAttackID, EAttackType.Attack_1);
                    ChangeAttackingState(new AttackingState(nextAttackID, EAttackType.Attack_1));
                }
            }
            else
            {
                //  TODO: Trigger Air Attack
            }
        }

        public bool IsInComboWindow => currentAttackingState is AttackComboWindowState;
        public bool IsInAttacking => currentAttackingState is AttackingState;

        public void ChangeAttackingState(IAttackState newState)
        {
            if (!ReferenceEquals(currentAttackingState, newState))
            {
                currentAttackingState?.OnExit(this);
                currentAttackingState = newState;
                currentAttackingState.OnEnter(this);
            }
        }

        public void ChangeToComboWindowState()
        {
            if (currentAttackingState is AttackingState attacking)
            {
                ChangeAttackingState(new AttackComboWindowState(attacking.HandledAttackID));
            }
        }

        public void ActivateComboAbility(int attackID)
        {
            Debug.Log($"AttackID: {attackID}");
            foreach (var config in AbilityConfigs)
            {
                if (attackID == config.attackID)
                {
                    SimpleGroundMeleeAttackAbility.SimpleGroundMeleeAttackAbilitySpec spec = (SimpleGroundMeleeAttackAbility.SimpleGroundMeleeAttackAbilitySpec)config.Ability.CreateSpec(GetAbilitySystem());
                    StartCoroutine(spec.TryActivateAbility());
                }
            }
        }

        public void PlaySoundEffect(AudioClip newClip)
        {
            audioSource.clip = newClip;
            audioSource.Play();
        }

        public void RefreshAttributesUI()
        {
            uiMsgPub.Publish(new UIMessage()
            {
                MessageCode = RPGUIMessage.REFRESH_PLAYER_HEALTH_VALUE,
                Params = new object[] { savedPS.GetHealthMax() != 0 ? savedPS.GetHealth() / savedPS.GetHealthMax() : 0 }
            });
            
            uiMsgPub.Publish(new UIMessage()
            {
                MessageCode = RPGUIMessage.REFRESH_PLAYER_STAMINA_VALUE,
                Params = new object[] { savedPS.GetStaminaMax() != 0 ? savedPS.GetStamina() / savedPS.GetStaminaMax() : 0 }
            });
        }
        
        public int GetNextAttackID(int lastAttackID, EAttackType AttackInput)
        {
            //  From Library Get the 
            int nextAttackID = invalidAttackID;
            switch (AttackInput)
            {
                case EAttackType.Invalid:
                    break;
                case EAttackType.Attack_0:
                    if (!ComboLibrary_Attack0.TryGetValue(lastAttackID, out nextAttackID))
                    {
                        nextAttackID = invalidAttackID;
                    }
                    break;
                case EAttackType.Attack_1:
                    if (!ComboLibrary_Attack1.TryGetValue(lastAttackID, out nextAttackID))
                    {
                        nextAttackID = invalidAttackID;
                    }
                    break;
            }
            return nextAttackID;
        }
        
        public void Die()
        {
            SpectatorPawn spectator = world.GetPlayerController().GetSpectatorPawn();
            spectator?.SetActorPosition(transform.position);
            cameraManager.SetViewTarget(spectator);

            var deathEffect = GetAbilitySystem().MakeOutgoingSpec(DeathEffect);
            GetAbilitySystem().ApplyGameplayEffectSpecToSelf(deathEffect);
            
            Destroy(gameObject);
        }

        async UniTask ActivateInitialisationAbilities()
        {
            if (!GetAbilitySystem())
            {
                Debug.LogError($"{DEBUG_FLAG} Invalid AbilitySystem");
                return;
            }

            foreach (var ability in InitialisationAbilities)
            {
                AbilityTags.Add(ability.AbilityTags.AssetTag);
                var spec = ability.CreateSpec(GetAbilitySystem());
                GetAbilitySystem().GrantAbility(spec);
                StartCoroutine(spec.TryActivateAbility());
            }

            await UniTask.DelayFrame(1);
            //  Refresh UI after Grant abilities
            // RefreshAttributesUI();
        }
        
        void GrantCastableAbilities()
        {
            this.abilitySpecs = new AbstractAbilitySpec[AbilityConfigs.Length];
            for (var i = 0; i < AbilityConfigs.Length; i++)
            {
                AbilityTags.Add(AbilityConfigs[i].Ability.AbilityTags.AssetTag);
                var spec = AbilityConfigs[i].Ability.CreateSpec(this. GetAbilitySystem());
                this.GetAbilitySystem().GrantAbility(spec);
                this.abilitySpecs[i] = spec;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ClearAbilities();
            RestartPlayerAsync().Forget();
        }

        async UniTask RestartPlayerAsync()
        {
            await UniTask.DelayFrame(1);
            GM = world.GetGameMode();
            PlayerController PC = GM.GetPlayerController();
            if (PC) GM.RestartPlayer(PC);
        }
    }
}