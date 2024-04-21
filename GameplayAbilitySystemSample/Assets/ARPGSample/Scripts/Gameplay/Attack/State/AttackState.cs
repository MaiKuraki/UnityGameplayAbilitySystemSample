using CycloneGames.GameFramework;

namespace ARPGSample.Gameplay
{
    public interface IAttackState
    {
        void OnEnter(Pawn pawn);
        void OnExit(Pawn pawn);
        void OnUpdate(Pawn pawn);
    }

    public abstract class AttackState : IAttackState
    {
        public abstract void OnEnter(Pawn pawn);

        public abstract void OnExit(Pawn pawn);

        public virtual void OnUpdate(Pawn pawn) { }
    }
}