using CycloneGames.GameFramework;

namespace ARPGSample.Gameplay
{
    public interface IAttackState
    {
        void OnEnter(Pawn pawn);
        void OnExit(Pawn pawn);
        void OnUpdate(Pawn pawn);
        void Break(Pawn pawn);
        void ComboWindow(Pawn pawn);
    }
}