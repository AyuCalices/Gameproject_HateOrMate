using ExitGames.Client.Photon;
using UnityEngine;

namespace Features.Unit.Battle.Scripts.CanMoveAction
{
    public abstract class IsMovable_SO : ScriptableObject
    {
        public abstract void OnEvent(BattleBehaviour battleBehaviour, EventData photonEvent);

        public abstract void RequestMove(BattleBehaviour battleBehaviour, Vector3Int targetTileGridPosition, int skipLastMovementsCount);
    }
}