using CycloneGames.GameFramework;
using CycloneGames.UIFramework;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace ARPGSample.Gameplay
{
    public class EnemyCharacter : AICharacter
    {
        [Inject] private IBattleInfoService battleInfoService;
        [Inject] private IWorld world;
        [Inject] private IUIService uiService;

        public EnemyAnimationFSM AnimationFSM => (EnemyAnimationFSM)animationFSM;
        private bool isHealthBarInit = false;

        protected override void Start()
        {
            base.Start();
            
            InitHealthBarAsync().Forget();
        }

        protected override void Update()
        {
            base.Update();
            
            // Debug.Log($"Health: {GetHealth()}, MaxHealth: {GetHealthMax()}");
        }
        public void TakeDamage()
        {
            RefreshHealthUI();
            if (GetHealth() <= 0)
            {
                Die();
            }
        }

        private void RefreshHealthUI()
        {
            if (!isHealthBarInit)
            {
                Debug.Log($"Health bar not Init");
                return;
            }
            battleInfoService.RefreshHealthBar(this, GetHealthMax() > 0 ? GetHealth() / GetHealthMax() : 0);
        }

        async UniTask InitHealthBarAsync()
        {
            RPGGameMode rpgGameMode = world.GetGameMode() as RPGGameMode;

            await UniTask.WaitUntil(() => rpgGameMode.IsGameplayStart);
            await UniTask.WaitUntil(() => uiService.IsUIPageValid(UI.PageName.BattleInfoPage));
            battleInfoService.AddEnemyHealthBar(this,  GetHealthMax() > 0 ? GetHealth() / GetHealthMax() : 0);
            isHealthBarInit = true;
        }

        protected override void Die()
        {
            base.Die();

            DeathTask().Forget();
        }

        async UniTask DeathTask()
        {
            AnimationFSM.Dead();
            RB.simulated = false;
            CharacterCollider.enabled = false;
            battleInfoService?.RemoveEnemyHealthBar(this);
            
            await UniTask.Delay(1500);
            Destroy(gameObject);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
        }
    }
}