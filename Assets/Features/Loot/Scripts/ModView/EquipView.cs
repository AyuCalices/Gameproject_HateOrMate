using System;
using System.Collections.Generic;
using System.Linq;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Battle;
using UniRx;
using UnityEngine;

namespace Features.Loot.Scripts.ModView
{
    public class EquipView : MonoBehaviour
    {
        [SerializeField] private NetworkedUnitRuntimeSet_SO unitRuntimeSet;
        
        [SerializeField] private UnitViewBehaviour unitViewPrefab;
        [SerializeField] private Transform instantiationParent;

        private Dictionary<NetworkedBattleBehaviour, UnitViewBehaviour> _unitViewLookup;
        private IDisposable _updateUnitViewsDisposable;

        private void Awake()
        {
            _unitViewLookup = new Dictionary<NetworkedBattleBehaviour, UnitViewBehaviour>();
        }

        private void OnDestroy()
        {
            foreach (var unitViewBehaviour in _unitViewLookup)
            {
                Destroy(unitViewBehaviour.Value);
            }
            _unitViewLookup.Clear();
        }

        private void OnEnable()
        {
            _updateUnitViewsDisposable = unitRuntimeSet.ObserveEveryValueChanged(x => x.GetItems().Count)
                .Subscribe(_ => TryAddUnitViews());
        }

        private void OnDisable()
        {
            _updateUnitViewsDisposable.Dispose();
        }

        private void TryAddUnitViews()
        {
            foreach (NetworkedBattleBehaviour networkedBattleBehaviour in unitRuntimeSet.GetItems())
            {
                if (!networkedBattleBehaviour.TeamTagTypes.Contains(TeamTagType.Own) || 
                    _unitViewLookup.ContainsKey(networkedBattleBehaviour)) continue;

                UnitViewBehaviour instantiatedView = Instantiate(unitViewPrefab, instantiationParent);
                instantiatedView.Initialize(networkedBattleBehaviour.NetworkedStatsBehaviour);
                ApplyOnUnitViewInstantiated(instantiatedView);
                
                _unitViewLookup.Add(networkedBattleBehaviour, instantiatedView);
            }
        }

        private void ApplyOnUnitViewInstantiated(UnitViewBehaviour instantiatedView)
        {
            foreach (var unitViewBehaviour in _unitViewLookup
                .Where(unitViewBehaviour => unitViewBehaviour.Value != instantiatedView))
            {
                unitViewBehaviour.Value.UnitMods.ApplyOnUnitViewInstantiated(instantiatedView);
            }
        }
    }
}
