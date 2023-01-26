using System;
using System.Globalization;
using ExitGames.Client.Photon;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Stat;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Loot.UI.CharacterSelect
{
    public class UnitModViewBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] private UnitType_SO towerUnitType;
        [SerializeField] private Image unitSprite;
        [SerializeField] private TMP_Text unitName;

        [SerializeField] private TextUpdateBehaviour damageTextUpdateBehaviour;
        [SerializeField] private TextUpdateBehaviour healthTextUpdateBehaviour;
        [SerializeField] private TextUpdateBehaviour rangeTextUpdateBehaviour;
        [SerializeField] private TextUpdateBehaviour movementSpeedTextUpdateBehaviour;
        [SerializeField] private TextUpdateBehaviour staminaTextUpdateBehaviour;
        [SerializeField] private TextUpdateBehaviour speedTextUpdateBehaviour;
    
        private NetworkedStatsBehaviour _unitOwnerStats;
        private BattleBehaviour _unitOwnerBattleBehaviour;

        public void Initialize(NetworkedStatsBehaviour unitOwnerStats)
        {
            _unitOwnerStats = unitOwnerStats;
            _unitOwnerBattleBehaviour = unitOwnerStats.GetComponent<BattleBehaviour>();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            if (_unitOwnerStats == null) return;
            
            SetUnitVisualization();
            SetAllValues();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (!Equals(targetPlayer, PhotonNetwork.LocalPlayer) || _unitOwnerStats == null) return;
            
            SetAllValues();
        }
        
        private void SetUnitVisualization()
        {
            unitSprite.sprite = _unitOwnerBattleBehaviour.UnitClassData.sprite;
            unitName.text = _unitOwnerBattleBehaviour.UnitClassData.unitType.unitName;
        }
    
        private void SetAllValues()
        {
            UnitType_SO unitType = _unitOwnerBattleBehaviour.UnitClassData.unitType;
            
            damageTextUpdateBehaviour.UpdateText(_unitOwnerStats.NetworkedStatServiceLocator.Get<LocalStat>(StatType.Damage).GetTotalValue().ToString(CultureInfo.CurrentCulture));
            healthTextUpdateBehaviour.UpdateText(_unitOwnerStats.NetworkedStatServiceLocator.Get<LocalStat>(StatType.Health).GetTotalValue().ToString(CultureInfo.CurrentCulture));
            rangeTextUpdateBehaviour.UpdateText(_unitOwnerStats.NetworkedStatServiceLocator.Get<LocalStat>(StatType.Range).GetTotalValue().ToString(CultureInfo.CurrentCulture));
            movementSpeedTextUpdateBehaviour.UpdateText(_unitOwnerStats.NetworkedStatServiceLocator.Get<LocalStat>(StatType.MovementSpeed).GetTotalValue().ToString(CultureInfo.CurrentCulture));

            if (towerUnitType == unitType)
            {
                staminaTextUpdateBehaviour.gameObject.SetActive(true);
                staminaTextUpdateBehaviour.UpdateText(_unitOwnerStats.NetworkedStatServiceLocator.Get<LocalStat>(StatType.Stamina).GetTotalValue().ToString(CultureInfo.CurrentCulture));
                speedTextUpdateBehaviour.UpdateText(_unitOwnerStats.NetworkedStatServiceLocator.Get<LocalStat>(StatType.StaminaRefreshTime).GetTotalValue().ToString(CultureInfo.CurrentCulture));
            }
            else
            {
                staminaTextUpdateBehaviour.gameObject.SetActive(false);
                speedTextUpdateBehaviour.UpdateText(_unitOwnerStats.NetworkedStatServiceLocator.Get<LocalStat>(StatType.Speed).GetTotalValue().ToString(CultureInfo.CurrentCulture));
            }
        }
    }
}
