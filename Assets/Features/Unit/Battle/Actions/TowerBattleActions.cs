using Features.GlobalReferences;
using Features.Unit.Battle;
using Features.Unit.Modding;
using UnityEngine;

public class TowerBattleActions : BattleActions
{
    private readonly int _totalStamina;
    private int _currentStamina;
    private readonly float _staminaRefreshTime;
    private float _staminaRefreshTimeDelta;

    public TowerBattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, BattleBehaviour ownerBattleBehaviour,
        UnitView ownerUnitView, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet, int totalStamina,
        float staminaRefreshTime) : base(ownerNetworkingUnitBehaviour, ownerBattleBehaviour, ownerUnitView,
        opponentNetworkedUnitRuntimeSet)
    {
        _totalStamina = totalStamina;
        _currentStamina = totalStamina;
        _staminaRefreshTime = staminaRefreshTime;
        _staminaRefreshTimeDelta = staminaRefreshTime;
    }
    
    protected override void InternalUpdateBattleActions()
    {
        _staminaRefreshTimeDelta -= Time.deltaTime;

        if (_staminaRefreshTimeDelta > 0) return;
        
        _staminaRefreshTimeDelta = _staminaRefreshTime;
        if (_currentStamina <= _totalStamina)
        {
            _currentStamina++;
            ownerUnitView.SetStaminaSlider(_currentStamina, _totalStamina);
        }
    }

    protected override void InternalOnPerformAction()
    {
        if (_currentStamina <= 0) return;

        _currentStamina--;
        ownerUnitView.SetStaminaSlider(_currentStamina, _totalStamina);
        
        PerformAttack(ownerNetworkingUnitBehaviour.ControlType);
    }
}
