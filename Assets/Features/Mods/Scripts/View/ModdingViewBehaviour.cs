using System;
using System.Collections.Generic;
using System.Linq;
using Features.Unit.Scripts.Behaviours;
using Features.Unit.Scripts.Behaviours.Services.BattleBehaviour;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Features.Mods.Scripts.View
{
    public class ModdingViewBehaviour : MonoBehaviour
    {
        [FormerlySerializedAs("unitRuntimeSet")] [SerializeField] private UnitServiceProviderRuntimeSet_SO unitServiceProviderRuntimeSet;
        
        [FormerlySerializedAs("unitViewPrefab")] [SerializeField] private UnitDisplayBehaviour unitDisplayBehaviourPrefab;
        [SerializeField] private Transform instantiationParent;

        private Dictionary<UnitServiceProvider, UnitDisplayBehaviour> _unitViewLookup;
        private IDisposable _updateUnitViewsDisposable;

        private void Awake()
        {
            _unitViewLookup = new Dictionary<UnitServiceProvider, UnitDisplayBehaviour>();
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
            _updateUnitViewsDisposable = unitServiceProviderRuntimeSet.ObserveEveryValueChanged(x => x.GetItems().Count)
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
            var itemsToRemove = _unitViewLookup.Where(x => !unitServiceProviderRuntimeSet.GetItems().Contains(x.Key)).ToArray();
            foreach (var itemToRemove in itemsToRemove)
            {
                Destroy(itemToRemove.Value.gameObject);
                _unitViewLookup.Remove(itemToRemove.Key);
            }
        }

        private void TryAddUnitViews()
        {
            foreach (UnitServiceProvider unitServiceProvider in unitServiceProviderRuntimeSet.GetItems())
            {
                if (!unitServiceProvider.TeamTagTypes.Contains(TeamTagType.Own) || 
                    _unitViewLookup.ContainsKey(unitServiceProvider)) continue;

                UnitDisplayBehaviour unitDisplayBehaviour = Instantiate(unitDisplayBehaviourPrefab, instantiationParent);
                unitDisplayBehaviour.Initialize(unitServiceProvider);
                ApplyOnUnitViewInstantiated(unitDisplayBehaviour);
                
                _unitViewLookup.Add(unitServiceProvider, unitDisplayBehaviour);
            }
        }

        private void ApplyOnUnitViewInstantiated(UnitDisplayBehaviour unitDisplayBehaviour)
        {
            foreach (var unitViewBehaviour in _unitViewLookup
                .Where(unitViewBehaviour => unitViewBehaviour.Value != unitDisplayBehaviour))
            {
                unitViewBehaviour.Value.UnitDisplayMods.ApplyOnUnitViewInstantiated(unitDisplayBehaviour);
            }
        }
    }
}
