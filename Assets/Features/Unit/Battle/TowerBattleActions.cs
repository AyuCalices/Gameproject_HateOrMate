using Features.GlobalReferences;
using Features.Unit.Battle;
using Features.Unit.Modding;
using Features.Unit.Modding.Stat;
using Photon.Pun;
using UnityEngine;

public class TowerBattleActions : BattleActions
{
    private readonly int _totalStamina;
    private int _currentStamina;
    private readonly float _staminaRefreshTime;
    private float _staminaRefreshTimeDelta;
    
    public TowerBattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet, UnitView unitView, int totalStamina, float staminaRefreshTime) : base(ownerNetworkingUnitBehaviour, opponentNetworkedUnitRuntimeSet, unitView)
    {
        _totalStamina = totalStamina;
        _currentStamina = totalStamina;
        _staminaRefreshTime = staminaRefreshTime;
        _staminaRefreshTimeDelta = staminaRefreshTime;
    }
    
    protected override void InternalUpdateBattleActions(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour, UnitView unitView)
    {
        _staminaRefreshTimeDelta -= Time.deltaTime;
        
        if (_staminaRefreshTimeDelta > 0) return;
        
        _staminaRefreshTimeDelta = _staminaRefreshTime;
        if (_currentStamina <= _totalStamina)
        {
            _currentStamina++;
            unitView.SetStaminaSlider(_currentStamina, _totalStamina);
        }
    }

    protected override void InternalOnPerformAction(NetworkedUnitBehaviour ownerNetworkingUnitBehaviour,
        NetworkedUnitRuntimeSet_SO opponentNetworkedUnitRuntimeSet)
    {
        if (_currentStamina <= 0) return;

        _currentStamina--;
        unitView.SetStaminaSlider(_currentStamina, _totalStamina);
        
        RaiseSendAttackEvent(
            opponentNetworkedUnitRuntimeSet.GetClosestByWorldPosition(ownerNetworkingUnitBehaviour.transform.position).Key.ViewID, 
            ownerNetworkingUnitBehaviour.NetworkedStatServiceLocator.GetTotalValue(StatType.Damage)
            );
    }

    public override void OnSendAttackActionCallback(BattleBehaviour targetBattleBehaviour, float value)
    {
        RaiseSendHealthEvent(
            targetBattleBehaviour.GetComponent<PhotonView>().ViewID,
            targetBattleBehaviour.RemovedHealth + value,
            targetBattleBehaviour.GetComponent<NetworkedUnitBehaviour>().NetworkedStatServiceLocator.GetTotalValue(StatType.Health)
            );
    }

    public override void OnSendHealthActionCallback(BattleBehaviour updateUnit, float newRemovedHealth, float totalHealth)
    {
        if (newRemovedHealth >= totalHealth)
        {
            Debug.LogWarning("DEAD!");
        }
        else
        {
            updateUnit.RemovedHealth = newRemovedHealth;
        }
    }
}
