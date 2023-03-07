using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Features.Mods.Scripts.View.ModContainer;
using Features.Unit.Scripts;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Services.UnitStats;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Features.Mods.Scripts.View
{
    public class UnitDisplayBehaviour : MonoBehaviourPunCallbacks
    {
        [FormerlySerializedAs("unitViewRuntimeSet")] [SerializeField] private UnitDisplayRuntimeSet_SO unitDisplayRuntimeSet;
        [SerializeField] private UnitType_SO towerUnitType;
        [SerializeField] private Image unitSprite;
        [SerializeField] protected List<StatTextUpdateBehaviour> statTextUpdateBehaviours;
        [SerializeField] protected TMP_Text nameText;
        
        public UnitDisplayMods UnitDisplayMods { get; private set; }
    
        public UnitServiceProvider UnitServiceProvider { get; private set; }
        private UnitModContainerBehaviour[] _modSlotBehaviour;

        private void Awake()
        {
            _modSlotBehaviour = GetComponentsInChildren<UnitModContainerBehaviour>();
            unitDisplayRuntimeSet.Add(this);
        }

        private void OnDestroy()
        {
            UnitDisplayMods.OnDestroy();
            unitDisplayRuntimeSet.Remove(this);
            
            UnitServiceProvider.GetService<UnitStatsBehaviour>().StatServiceLocator.onUpdateStat -= UpdateSingleStatText;
        }
        
        private void UpdateSingleStatText(StatType statType)
        {
            foreach (StatTextUpdateBehaviour statTextUpdateBehaviour in statTextUpdateBehaviours
                .Where(statTextUpdateBehaviour => statTextUpdateBehaviour.StatType == statType))
            {
                statTextUpdateBehaviour.UpdateText(Mathf.Floor(UnitServiceProvider.GetService<UnitStatsBehaviour>().GetFinalStat(statType)).ToString());
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
            UnitDisplayMods = new UnitDisplayMods(unitServiceProvider, _modSlotBehaviour);
            
            UnitServiceProvider = unitServiceProvider;
            
            UnitServiceProvider.GetService<UnitStatsBehaviour>().StatServiceLocator.onUpdateStat += UpdateSingleStatText;
            
            InitializeVisualization(UnitServiceProvider.UnitClassData);
            InitializeAllText(UnitServiceProvider.UnitClassData);
        }

        private void InitializeVisualization(UnitClassData_SO unitClassData)
        {
            unitSprite.sprite = unitClassData.sprite;
            nameText.text = unitClassData.unitType.unitName;
        }
        
        private void InitializeAllText(UnitClassData_SO unitClassData)
        {
            UnitType_SO unitType = unitClassData.unitType;
            StatTextUpdateBehaviour staminaStatTextUpdateBehaviours = statTextUpdateBehaviours.Find(x => x.StatType == StatType.Stamina);
            staminaStatTextUpdateBehaviours.gameObject.SetActive(towerUnitType == unitType);
        }
    }
}
