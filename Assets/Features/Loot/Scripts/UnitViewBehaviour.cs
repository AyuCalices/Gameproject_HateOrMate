using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Features.Battle.Scripts.Unit.ServiceLocatorSystem;
using Features.Loot.Scripts.ModView;
using Features.UI.Scripts;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using Features.Unit.Scripts.Behaviours.Mod;
using Features.Unit.Scripts.Stats;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Loot.Scripts
{
    public class UnitViewBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] private UnitViewRuntimeSet_SO unitViewRuntimeSet;
        [SerializeField] private UnitType_SO towerUnitType;
        [SerializeField] private Image unitSprite;
        [SerializeField] protected List<StatTextUpdateBehaviour> statTextUpdateBehaviours;
        [SerializeField] protected TMP_Text nameText;
        
        public UnitMods UnitMods { get; private set; }
    
        public UnitServiceProvider UnitServiceProvider { get; private set; }
        private NetworkedBattleBehaviour _unitOwnerBattleBehaviour;
        private ModSlotBehaviour[] _modSlotBehaviour;

        private void Awake()
        {
            _modSlotBehaviour = GetComponentsInChildren<ModSlotBehaviour>();
            unitViewRuntimeSet.Add(this);
        }

        private void OnDestroy()
        {
            UnitMods.OnDestroy();
            unitViewRuntimeSet.Remove(this);
            
            UnitServiceProvider.GetService<NetworkedStatsBehaviour>().NetworkedStatServiceLocator.onUpdateStat -= UpdateSingleStatText;
        }
        
        private void UpdateSingleStatText(StatType statType)
        {
            foreach (StatTextUpdateBehaviour statTextUpdateBehaviour in statTextUpdateBehaviours
                .Where(statTextUpdateBehaviour => statTextUpdateBehaviour.StatType == statType))
            {
                statTextUpdateBehaviour.UpdateText(Mathf.Floor(UnitServiceProvider.GetService<NetworkedStatsBehaviour>().GetFinalStat(statType)).ToString());
                return;
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (Equals(targetPlayer, PhotonNetwork.LocalPlayer)) return;

            foreach (StatTextUpdateBehaviour statTextUpdateBehaviour in statTextUpdateBehaviours)
            {
                UpdateSingleStatText(statTextUpdateBehaviour.StatType);
            }
        }

        public void Initialize(UnitServiceProvider unitServiceProvider)
        {
            UnitMods = new UnitMods(unitServiceProvider, _modSlotBehaviour);
            
            UnitServiceProvider = unitServiceProvider;
            _unitOwnerBattleBehaviour = unitServiceProvider.GetComponent<NetworkedBattleBehaviour>();
            
            UnitServiceProvider.GetService<NetworkedStatsBehaviour>().NetworkedStatServiceLocator.onUpdateStat += UpdateSingleStatText;
            
            InitializeVisualization();
            InitializeAllText();
        }

        private void InitializeVisualization()
        {
            unitSprite.sprite = _unitOwnerBattleBehaviour.UnitClassData.sprite;
            nameText.text = _unitOwnerBattleBehaviour.UnitClassData.unitType.unitName;
        }
        
        private void InitializeAllText()
        {
            UnitType_SO unitType = _unitOwnerBattleBehaviour.UnitClassData.unitType;
            StatTextUpdateBehaviour staminaStatTextUpdateBehaviours = statTextUpdateBehaviours.Find(x => x.StatType == StatType.Stamina);
            staminaStatTextUpdateBehaviours.gameObject.SetActive(towerUnitType == unitType);
        }
    }
}
