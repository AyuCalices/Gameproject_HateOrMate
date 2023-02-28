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

        private Dictionary<UnitServiceProvider, UnitViewBehaviour> _unitViewLookup;
        private IDisposable _updateUnitViewsDisposable;

        private void Awake()
        {
            _unitViewLookup = new Dictionary<UnitServiceProvider, UnitViewBehaviour>();
        }

        private void OnDestroy()
        {
            foreach (var unitViewBehaviour in _unitViewLookup)
            {
                Destroy(unitViewBehaviour.Value.gameObject);
            }
            _unitViewLookup.Clear();
        }

        private void OnEnable()
        {
            _updateUnitViewsDisposable = unitRuntimeSet.ObserveEveryValueChanged(x => x.GetItems().Count)
                .Subscribe(_ =>
                {
                    TryRemoveUnitViews();
                    TryAddUnitViews();
                });
        }

        private void OnDisable()
        {
            _updateUnitViewsDisposable.Dispose();
        }

        private void TryRemoveUnitViews()
        {
            var itemsToRemove = _unitViewLookup.Where(x => !unitRuntimeSet.GetItems().Contains(x.Key)).ToArray();
            foreach (var itemToRemove in itemsToRemove)
            {
                Destroy(itemToRemove.Value.gameObject);
                _unitViewLookup.Remove(itemToRemove.Key);
            }
        }

        private void TryAddUnitViews()
        {
            foreach (UnitServiceProvider unitServiceProvider in unitRuntimeSet.GetItems())
            {
                if (!unitServiceProvider.TeamTagTypes.Contains(TeamTagType.Own) || 
                    _unitViewLookup.ContainsKey(unitServiceProvider)) continue;

                UnitViewBehaviour instantiatedView = Instantiate(unitViewPrefab, instantiationParent);
                instantiatedView.Initialize(unitServiceProvider);
                ApplyOnUnitViewInstantiated(instantiatedView);
                
                _unitViewLookup.Add(unitServiceProvider, instantiatedView);
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
