namespace Features.Connection.Scripts.Utils
{
    public enum RaiseEventCode
    {
        OnRequestUnitInstantiation = 2,
        OnPerformGridStep = 3,
        OnRequestGridStep = 4,
        OnRequestSpawnThenTeleport = 5,
        OnPerformGridTeleport = 6,
        OnRequestTeleport = 7,
        OnPerformTeleportThenSpawn = 8,
        OnSendFloatToTarget = 9,
        OnUpdateAllClientsHealth = 10,
        OnRequestBattleState = 11,
        OnEnterUnitIdleState = 12,
        OnNextStage = 13,
        OnRestartStage = 15,
        OnEndGame = 16
    }
}
