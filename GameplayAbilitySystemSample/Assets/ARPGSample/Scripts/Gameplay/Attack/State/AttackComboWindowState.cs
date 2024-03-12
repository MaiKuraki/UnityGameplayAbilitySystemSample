using System.Threading;
using CycloneGames.GameFramework;
using Cysharp.Threading.Tasks;

namespace ARPGSample.Gameplay
{
    public class AttackComboWindowState : IAttackState
    {
        private static readonly string STATE_NAME = "[ComboWindow]";

        private int handledAttackID;
        public int HandledAttackID => handledAttackID;
        private CancellationTokenSource cts = new CancellationTokenSource();

        public AttackComboWindowState(int handledAttackID)
        {
            this.handledAttackID = handledAttackID;
        }
        public void OnEnter(Pawn pawn)
        {
            UnityEngine.Debug.Log($"{STATE_NAME} Enter");
            AutoFinishedComboWindowTask(pawn, cts).Forget();
        }

        public void OnExit(Pawn pawn)
        {
            UnityEngine.Debug.Log($"{STATE_NAME} Exit");
            cts.Cancel();
        }

        public void OnUpdate(Pawn pawn)
        {
            
        }

        public void Break(Pawn pawn)
        {
            
        }

        public void ComboWindow(Pawn pawn)
        {
            
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