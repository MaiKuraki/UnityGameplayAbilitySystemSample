using CycloneGames.GameplayAbilities.Runtime;
using CycloneGames.GameplayFramework;
using CycloneGames.Logger;
using UnityEngine;
using VContainer;

namespace GASSample.Gameplay
{
    public class GASSamplePlayerCharacter : GASSampleCharacter
    {
        [Inject] IWorld world;

        [SerializeField] private Transform CameraFocusTF;

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

        }
        override protected void Update()
        {
            GetMovementComponent?.MoveWithVelocity(movementVelocity);

            CLogger.LogInfo($"HealthVal: {AttributeSet.GetCurrentValue(AttributeSet.Health)}");
        }

        public override void PossessedBy(Controller NewController)
        {
            base.PossessedBy(NewController);

            PlayerController pc = NewController as PlayerController;

            ApplyInitialEffects();
            GrandInitialAbilities();

            var cameraManager = pc.GetCameraManager();
            cameraManager.SetViewTarget(CameraFocusTF);
        }

        private void ApplyInitialEffects()
        {
            if (InitialAttributes != null && AbilitySystemComponent != null)
            {
                var ge = InitialAttributes.CreateGameplayEffect();
                var spec = GameplayEffectSpec.Create(ge, AbilitySystemComponent);
                AbilitySystemComponent.ApplyGameplayEffectSpecToSelf(spec);
            }

            if (InitialPassiveEffects != null)
            {
                foreach (var passiveEffectSO in InitialPassiveEffects)
                {
                    if (passiveEffectSO != null)
                    {
                        var ge = passiveEffectSO.CreateGameplayEffect();
                        var spec = GameplayEffectSpec.Create(ge, AbilitySystemComponent);
                        AbilitySystemComponent.ApplyGameplayEffectSpecToSelf(spec);
                    }
                }
            }
        }

        private void GrandInitialAbilities()
        {
            if (AbilitySystemComponent == null) return;
            foreach (var abilitySO in InitialAbilities)
            {
                if (abilitySO != null)
                {
                    AbilitySystemComponent.GrantAbility(abilitySO.CreateAbility());
                }
            }
        }
    }
}
