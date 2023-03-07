namespace Features.Unit.Scripts.Behaviours.Battle.BattleBehaviour
{
    public interface IBattleBehaviour
    {
        void OnStageEnd();
        void Update();
        void ForceIdleState();
        void ForceBenchedState();
        bool TryRequestIdleState();
        bool TryRequestAttackState();
        bool TryRequestMovementStateByClosestUnit();
        bool TryRequestDeathState();
    }
}
