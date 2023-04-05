namespace Features.Unit.Scripts.Behaviours.Services.BattleBehaviour
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
