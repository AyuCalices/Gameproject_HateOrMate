namespace Features
{
    public enum RaiseEventCode
    {
        OnUnitManualInstantiation = 1,
        OnRequestUnitManualInstantiation = 2,
        OnPerformGridPositionSwap = 3,
        OnRequestGridPositionSwap = 4,
        OnPerformGridTeleport = 5,
        OnRequestGridTeleport = 6,
        OnRequestGridTeleportAndSpawn = 7,
        OnSendFloatToTarget = 8,
        OnUpdateAllClientsHealth = 9,
        OnRequestBattleState = 10,
        OnObtainLoot = 11,
        OnAllPlayerUnitsInstantiated = 12
    }
}
