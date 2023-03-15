using System.Collections;
using System.Linq;
using Features.BattleScene.Scripts.StageProgression;
using Features.BattleScene.Scripts.StateMachine;
using Features.Tiles.Scripts;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Services.BattleBehaviour;
using Features.Unit.Scripts.Behaviours.Services.UnitStats;
using Photon.Pun;
using UnityEngine;

namespace Features.BattleScene.Scripts.States
{
    [CreateAssetMenu]
    public class StageSetupState : BaseCoroutineState
    {
        [Header("Derived References")]
        [SerializeField] private BattleData_SO battleData;
        [SerializeField] private StageRandomizer_SO stageRandomizer;
        
        private BattleManager _battleManager;
        private bool _initialized;
        
        private void OnEnable()
        {
            _initialized = false;
        }

        public StageSetupState Initialize(BattleManager battleManager)
        {
            if (_initialized) return this;
            
            _battleManager = battleManager;
            
            return this;
        }

        public override IEnumerator Enter()
        {
            yield return base.Enter();

            if (!battleData.IsStageRestart)
            {
                //TODO: unit leveling is not synchronized
                battleData.Stage.Add(1);

                foreach (UnitServiceProvider unitServiceProvider in battleData.UnitsServiceProviderRuntimeSet.GetItems())
                {
                    if (unitServiceProvider.UnitClassData.levelUpOnStageClear)
                    {
                        unitServiceProvider.GetService<UnitStatsBehaviour>().SetBaseStats(unitServiceProvider.UnitClassData.baseStatsData, battleData.Stage.Get());
                    }
                }
            }
            
            foreach (UnitServiceProvider unitServiceProvider in battleData.UnitsServiceProviderRuntimeSet.GetItems())
            {
                if (unitServiceProvider.TeamTagTypes.Contains(TeamTagType.Own) || unitServiceProvider.TeamTagTypes.Contains(TeamTagType.Mate))
                {
                    Vector3Int currentCellPosition = GridPositionHelper.GetCurrentCellPosition(battleData, unitServiceProvider.transform);
                    GridPositionHelper.UpdateUnitOnRuntimeTiles(battleData, unitServiceProvider, currentCellPosition, unitServiceProvider.TeleportGridPosition);
                    unitServiceProvider.transform.position = battleData.TileRuntimeDictionary.GetCellToWorldPosition(unitServiceProvider.TeleportGridPosition);
                }
            }
            

            if (PhotonNetwork.IsMasterClient)
            {
                stageRandomizer.GenerateUnits();
            }
            
            _battleManager.RequestBattleState();
        }
    }
}
