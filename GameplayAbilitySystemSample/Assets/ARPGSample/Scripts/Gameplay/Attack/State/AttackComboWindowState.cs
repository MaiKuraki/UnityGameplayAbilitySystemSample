using System.Threading;
using CycloneGames.GameFramework;
using Cysharp.Threading.Tasks;

namespace ARPGSample.Gameplay
{
    public class AttackComboWindowState : AttackState
    {
        private static readonly string STATE_NAME = "[ComboWindow]";

        private int handledAttackID;
        public int HandledAttackID => handledAttackID;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public AttackComboWindowState(int handledAttackID)
        {
            this.handledAttackID = handledAttackID;
        }
        public override void OnEnter(Pawn pawn)
        {
            UnityEngine.Debug.Log($"{STATE_NAME} Enter");
            AutoFinishedComboWindowTask(pawn, cts).Forget();
        }

        public override void OnExit(Pawn pawn)
        {
            UnityEngine.Debug.Log($"{STATE_NAME} Exit");
            cts.Cancel();
        }
        
        async UniTask AutoFinishedComboWindowTask(Pawn pawn, CancellationTokenSource ct = default)
        { 
            var rpgPawn = (RPGPlayerCharacter)pawn; 
            // UnityEngine.Debug.Log($"ComboWindowState, WindowMilliSecond: {rpgPawn.ComboWindowMilliSecond}ms");
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