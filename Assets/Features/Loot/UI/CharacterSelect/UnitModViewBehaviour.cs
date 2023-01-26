using System;
using System.Globalization;
using ExitGames.Client.Photon;
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
        [SerializeField] private Image unitSprite;
        [SerializeField] private TMP_Text unitName;
        [SerializeField] private TMP_Text damageText;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text speedText;
        [SerializeField] private TMP_Text movementText;
    
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
            damageText.text = _unitOwnerStats.NetworkedStatServiceLocator.Get<LocalStat>(StatType.Damage).GetTotalValue().ToString(CultureInfo.CurrentCulture);
            healthText.text = _unitOwnerStats.NetworkedStatServiceLocator.Get<LocalStat>(StatType.Health).GetTotalValue().ToString(CultureInfo.CurrentCulture);
            speedText.text = _unitOwnerStats.NetworkedStatServiceLocator.Get<LocalStat>(StatType.Speed).GetTotalValue().ToString(CultureInfo.CurrentCulture);
            movementText.text = _unitOwnerStats.NetworkedStatServiceLocator.Get<LocalStat>(StatType.MovementSpeed).GetTotalValue().ToString(CultureInfo.CurrentCulture);
        }
    }
}
