using System.Collections;
using AbilitySystem;
using AbilitySystem.Authoring;
using CycloneGames.GameFramework;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace ARPGSample.Gameplay
{
    [CreateAssetMenu(menuName = "CycloneGames/GAS/Ability/SimpleGroundMeleeAttackAbility")]
    public class SimpleGroundMeleeAttackAbility : SimpleAbilityScriptableObject
    {
        [Header("------ Attack Config ------")]
        [SerializeField] private AnimatorOverrideController ACOverride;
        [SerializeField] private SimpleMeleeAttackCollider ColliderPrefab;
        [SerializeField] private GameplayEffectScriptableObject SourceAttackingEffect;
        [SerializeField] private GameplayEffectScriptableObject SourceAttackingHitEffect;
        [SerializeField] private GameplayEffectScriptableObject SourceAttackingFinishedEffect;
        [SerializeField] private float hit_cameraShakeForce = 1f;
        [SerializeField] private float hit_cameraShakeDurationSecond = 0.1f;
        [SerializeField] private AudioClip NoHitAudio;
        public override AbstractAbilitySpec CreateSpec(AbilitySystem.AbilitySystemComponent owner)
        {
            var spec = new SimpleGroundMeleeAttackAbilitySpec(this, owner)
            {
                Level = owner.Level,
                ACOverride = ACOverride,
                ColliderPrefab = ColliderPrefab,
                SourceAttackingEffect = SourceAttackingEffect,
                SourceAttackingHitEffect = SourceAttackingHitEffect,
                SourceAttackingFinishedEffect = SourceAttackingFinishedEffect,
                HitCameraShakeForce = hit_cameraShakeForce,
                HitCameraShakeDuration = hit_cameraShakeDurationSecond,
                NoHitAudio = NoHitAudio,
            };
            return spec;
        }
        
        public class SimpleGroundMeleeAttackAbilitySpec : AbstractAbilitySpec
        {
            public AnimatorOverrideController ACOverride { get; set; }
            public SimpleMeleeAttackCollider ColliderPrefab { get; set; }
            private AbilitySystem.AbilitySystemComponent CachedOwner { get; set; }
            private SimpleMeleeAttackCollider CachedCollider { get; set; }
            public GameplayEffectScriptableObject SourceAttackingEffect { get; set; }
            public GameplayEffectScriptableObject SourceAttackingHitEffect { get; set; }
            public GameplayEffectScriptableObject SourceAttackingFinishedEffect { get; set; }
            public float HitCameraShakeDuration { get; set; }
            public float HitCameraShakeForce { get; set; }
            public AudioClip NoHitAudio { get; set; }

            public SimpleGroundMeleeAttackAbilitySpec(SimpleGroundMeleeAttackAbility ability, AbilitySystem.AbilitySystemComponent owner) : base(ability, owner)
            {
                CachedOwner = owner;
            }

            public override bool CanActivateAbility()
            {
                bool result = base.CanActivateAbility();

                if (!result)
                {
                    RPGPlayerState rpgPS = CachedOwner.GetComponent<RPGPlayerState>();
                    if (rpgPS)
                    {
                        var rpgPawn = (RPGPlayerCharacter)rpgPS.GetPawn();
                        if (rpgPawn.CurrentAttackingState is AttackingState state) rpgPawn.ChangeAttackingState(new BreakAttackState());
                    }
                }

                return result;
            }

            public override void CancelAbility()
            {
                RPGPlayerState rpgPS = CachedOwner.GetComponent<RPGPlayerState>();
                if (rpgPS)
                {
                    var rpgPawn = (RPGPlayerCharacter)rpgPS.GetPawn();
                    if (rpgPawn.CurrentAttackingState is AttackingState state) rpgPawn.ChangeAttackingState(new BreakAttackState());
                }
            }

            public override bool CheckGameplayTags()
            {
                return AscHasAllTags(Owner, this.Ability.AbilityTags.OwnerTags.RequireTags)
                       && AscHasNoneTags(Owner, this.Ability.AbilityTags.OwnerTags.IgnoreTags)
                       && AscHasAllTags(Owner, this.Ability.AbilityTags.SourceTags.RequireTags)
                       && AscHasNoneTags(Owner, this.Ability.AbilityTags.SourceTags.IgnoreTags);
            }

            protected override IEnumerator PreActivate()
            {
                RPGPlayerState rpgPS = CachedOwner.GetComponent<RPGPlayerState>();
                
                yield return null; 
            }

            protected override IEnumerator ActivateAbility()
            {
                RPGPlayerState rpgPS = CachedOwner.GetComponent<RPGPlayerState>();
                GameplayEffectSpec cdSpec = null;
                if (this.Ability.Cooldown)
                {
                    cdSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cooldown);
                }
                GameplayEffectSpec costSpec = null;
                if (this.Ability.Cost)
                {
                    costSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost);
                }
                
                if (rpgPS)
                {
                    var rpgPawn = (RPGPlayerCharacter)rpgPS.GetPawn();
                    Animator animator = rpgPawn.GetComponent<Animator>();
                    var oldAttackTrigger = rpgPawn.transform.Find("AttackingTrigger");
                    if(oldAttackTrigger)
                    {
                        Destroy(oldAttackTrigger.gameObject);
                    }
                                        
                    CachedCollider = Instantiate(ColliderPrefab, rpgPawn.transform);
                    CachedCollider.gameObject.name = "AttackingTrigger";
                    CachedCollider.OnCollisionEnter += OnAttackCollisionEnter;
                    
                    var sourceAttackingEffect = this.Owner.MakeOutgoingSpec((this.Ability as SimpleGroundMeleeAttackAbility)?.SourceAttackingEffect);
                    CachedOwner.ApplyGameplayEffectSpecToSelf(sourceAttackingEffect);
                    
                    yield return null;
                    animator.runtimeAnimatorController = null;      //  TODO: animation will not trigger collision if no reset animator
                    animator.runtimeAnimatorController = ACOverride;
                    rpgPawn.AnimationFsm.EnableAttacking();
                    if (rpgPawn.CurrentAttackingState is AttackingState)
                    {
                        if (rpgPawn.CurrentAttackingState is AttackingState state) state.OnAbilityActivated(rpgPawn);
                    }
                    
                    if(cdSpec != null) this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
                    if(costSpec != null) this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);

                    rpgPawn.PlaySoundEffect(NoHitAudio);
                    yield return null;                              //  TODO: maybe a better function to waiting effect result.
                    rpgPawn.RefreshAttributesUI();
                    yield return new WaitUntil(() => !(rpgPawn.CurrentAttackingState is AttackingState));
                }
                
                yield return null;
                EndAbility();
            }

            public override void EndAbility()
            {
                if(CachedCollider) CachedCollider.OnCollisionEnter -= OnAttackCollisionEnter;
                
                var sourceFinishedEffect = this.Owner.MakeOutgoingSpec((this.Ability as SimpleGroundMeleeAttackAbility)?.SourceAttackingFinishedEffect);
                CachedOwner.ApplyGameplayEffectSpecToSelf(sourceFinishedEffect);
                
                base.EndAbility();
            }

            void OnAttackCollisionEnter(GameObject other)
            {
                //  TODO: 
                //  slow down movement speed
                //  maybe camera shake
                //  other take damage
                
                OnCollisionEnterAsync(other).Forget();
            }

            async UniTask OnCollisionEnterAsync(GameObject other)
            {
                var sourceHitEffect = this.Owner.MakeOutgoingSpec((this.Ability as SimpleGroundMeleeAttackAbility)?.SourceAttackingHitEffect);
                CachedOwner.ApplyGameplayEffectSpecToSelf(sourceHitEffect);
                RPGPlayerState rpgPS = CachedOwner.GetComponent<RPGPlayerState>();
                if (rpgPS)
                {
                    var rpgPawn = (RPGPlayerCharacter)rpgPS.GetPawn();
                    if (rpgPawn)
                    {
                        RPGPlayerController PC = (RPGPlayerController)rpgPawn.Controller;
                        if (PC)
                        {
                            CMRPGCameraManager cameraManager = (CMRPGCameraManager)PC.GetCameraManager();
                            if (cameraManager)
                            {
                                cameraManager.CameraShake(HitCameraShakeForce, HitCameraShakeDuration);
                            }
                        }

                        rpgPawn.AnimationFsm.SimpleFreezeAnimation().Forget();
                    }
                }
                
                var targetHitEffect = this.Owner.MakeOutgoingSpec((this.Ability as SimpleGroundMeleeAttackAbility)?.GameplayEffect);
                var target = other.GetComponent<RPGAbilitySystemComponent>();
                if (target)
                {
                    target.ApplyGameplayEffectSpecToSelf(targetHitEffect);
                    
                    await UniTask.DelayFrame(1);    //  Wait for Effect Result
                    
                    var enemyAnimationFSM = other.GetComponent<EnemyAnimationFSM>();
                    enemyAnimationFSM?.TriggerHit();
                    var enemyPawn = other.GetComponent<EnemyCharacter>();
                    enemyPawn.TakeDamage();
                    
                    Debug.Log($"Success Hit target: {other.name}");
                }
                else
                {
                    Debug.LogWarning($"target {other.name} not have an ability system");
                }
            }
        }
    }
}