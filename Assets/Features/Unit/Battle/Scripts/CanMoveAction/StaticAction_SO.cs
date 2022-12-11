using ExitGames.Client.Photon;
using UnityEngine;

namespace Features.Unit.Battle.Scripts.CanMoveAction
{
    [CreateAssetMenu]
    public class StaticAction_SO : MovableAction_SO
    {
        public override void OnEvent(BattleBehaviour battleBehaviour, EventData photonEvent)
        {
        }

        public override void RequestMove(BattleBehaviour battleBehaviour, Vector3Int targetTileGridPosition, int skipLastMovementsCount)
        {
        }
    }
}
