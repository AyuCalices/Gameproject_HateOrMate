namespace Features.Connection
{
    public enum RaiseEventCode
    {
        OnPerformUnitInstantiation = 1,
        OnRequestUnitInstantiation = 2,
        OnPerformGridStep = 3,
        OnRequestGridStep = 4,
        OnRequestSpawnThenTeleport = 5,
        OnPerformSpawnThenTeleport = 6,
        OnRequestTeleport = 7,
        OnPerformTeleportThenSpawn = 8,
        OnSendFloatToTarget = 9,
        OnUpdateAllClientsHealth = 10,
        OnRequestBattleState = 11,
        OnEnterUnitIdleState = 12,
        OnObtainLoot = 13,
        OnPlayerSynchronizedDespawn = 14
    }
}
