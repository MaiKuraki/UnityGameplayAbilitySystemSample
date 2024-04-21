using System.Threading;
using CycloneGames.GameFramework;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ARPGSample.Gameplay
{
    public class AttackingState : AttackState
    {
        private static readonly string STATE_NAME = "[Attacking]";
        private int handledAttackID;
        private RPGPlayerCharacter.EAttackType handledAttackType;
        public int HandledAttackID => handledAttackID;
        private CancellationTokenSource cts_ComboWindow = new CancellationTokenSource();
        private CancellationTokenSource cts_Attacking = new CancellationTokenSource();
        private RPGPlayerCharacter _cachedRpgPlayerCharacter;

        public AttackingState(int newAttackID, RPGPlayerCharacter.EAttackType newAttackType)
        {
            handledAttackID = newAttackID;
            handledAttackType = newAttackType;
        }
        public override void OnEnter(Pawn pawn)
        {
            var rpgPawn = (RPGPlayerCharacter)pawn;
            _cachedRpgPlayerCharacter = (RPGPlayerCharacter)pawn;
            //rpgPawn.attack

            rpgPawn.ActivateComboAbility(handledAttackID);
            UnityEngine.Debug.Log($"{STATE_NAME} Enter");
        }

        public override void OnExit(Pawn pawn)
        {
            UnityEngine.Debug.Log($"{STATE_NAME} Exit");
            
            cts_ComboWindow.Cancel();
            cts_Attacking.Cancel();

            //  TODO: if pawnAttackState is AttackCombo, do nothing.
            
            // var rpgPawn = (CMRPGPlayerPawn)pawn;
            // if(!rpgPawn.IsInComboWindow) rpgPawn.ChangeAttackingState(new AttackFinishedState());
        }

        private void ComboWindow(Pawn pawn)
        {
            var rpgPawn = (RPGPlayerCharacter)pawn;
            int nextAttackID = rpgPawn.GetNextAttackID(handledAttackID, handledAttackType);
            
            if (nextAttackID == rpgPawn.invalidAttackID)
            {
                rpgPawn.ChangeAttackingState(new AttackFinishedState());
            }
            else
            {
                rpgPawn.ChangeAttackingState(new AttackComboWindowState(handledAttackID));
            }
        }

        public void OnAbilityActivated(Pawn handledPawn)
        {
            if (!cts_Attacking.IsCancellationRequested)
            {
                cts_Attacking.Cancel();
                cts_Attacking.Dispose();
                cts_Attacking = new CancellationTokenSource();
            }
            EnterComboAsync(handledPawn, cts_Attacking).Forget();
        }

        async UniTask EnterComboAsync(Pawn handledPawn, CancellationTokenSource ct = default)
        {
            await UniTask.DelayFrame(1); // NOTE: Here Must Wait One Frame to return the correct State Info, The Animation is start playing no delay, dont worry
            if (ct.IsCancellationRequested) return;
            var rpgPawn = (RPGPlayerCharacter)handledPawn;
            var animator = rpgPawn.GetComponent<Animator>();
            int layerIdx = animator.GetLayerIndex("BattleLayer");
            var stateInfo = animator.GetCurrentAnimatorStateInfo(layerIdx);
            await UniTask.WaitUntil(() => stateInfo.IsName("Attack"), PlayerLoopTiming.Update, ct.Token);
            if (ct.IsCancellationRequested) return;
            float delayTime = stateInfo.length * 1000;
            // Debug.Log($"AttackingTask, AC: {animator.runtimeAnimatorController.name}, Length: {stateInfo.length}");
            await UniTask.Delay((int)delayTime, DelayType.Realtime, PlayerLoopTiming.Update, ct.Token);
            if (ct.IsCancellationRequested) return;
            ComboWindow(handledPawn); // Here we enter the combo window on Attack animation finished.
            if (!cts_ComboWindow.IsCancellationRequested)
            {
                cts_ComboWindow.Cancel();
                cts_ComboWindow.Dispose();
                cts_ComboWindow = new CancellationTokenSource();
            }
            AutoFinishedAttackTask(handledPawn, cts_ComboWindow).Forget();
        }
        
        async UniTask AutoFinishedAttackTask(Pawn pawn, CancellationTokenSource ct = default)
        {
            var rpgPawn = (RPGPlayerCharacter)pawn;
            // UnityEngine.Debug.Log($"AutoFinishedAfter: {rpgPawn.ComboWindowMilliSecond}ms");
            await UniTask.Delay(rpgPawn.ComboWindowMilliSecond, DelayType.Realtime, PlayerLoopTiming.Update, ct.Token);
            if (ct.IsCancellationRequested)
            {
                UnityEngine.Debug.Log($"{STATE_NAME} Cancelled ResetAttack");
                return;
            }
            rpgPawn.ChangeAttackingState(new AttackFinishedState());
        }
    }
}