using System.Collections.Generic;
using CycloneGames.GameplayAbilities.Runtime;
using CycloneGames.GameplayFramework;
using CycloneGames.RPGFoundation;
using CycloneGames.Utility.Runtime;
using UnityEngine;

namespace GASSample.Gameplay
{
    [RequireComponent(typeof(MovementComponent))]
    public class GASSampleCharacter : Pawn
    {
        protected MovementComponent movementComponent;
        protected MovementComponent GetMovementComponent => movementComponent;
        public AbilitySystemComponent AbilitySystemComponent { get; protected set; }
        public CharacterAttributeSet AttributeSet { get; protected set; }

        [PropertyGroup("Setup", true, GroupColorIndex = 3)]
        [SerializeField] protected GameplayEffectSO InitialAttributes;
        [SerializeField] protected List<GameplayAbilitySO> InitialAbilities;
        [SerializeField] protected List<GameplayEffectSO> InitialPassiveEffects;
        [SerializeField] protected LevelUpDataSO LevelUpData;
        [EndPropertyGroup]

        protected Vector3 movementVelocity = Vector3.zero;

        protected override void Awake()
        {
            base.Awake();

            movementComponent = GetComponent<MovementComponent>();
            var effectContextFactory = new GameplayEffectContextFactory();
            this.AbilitySystemComponent = new AbilitySystemComponent(effectContextFactory);
            AbilitySystemComponent.InitAbilityActorInfo(this, gameObject);
            AttributeSet = new CharacterAttributeSet();
            AbilitySystemComponent.AddAttributeSet(AttributeSet);
        }
        override protected void Update()
        {
            base.Update();
        }
    }
}