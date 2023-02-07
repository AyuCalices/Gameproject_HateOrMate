using System;
using System.Collections.Generic;
using Features.Unit.Scripts.Stats;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Stat
{
    public enum StatType { Damage, Health, Speed, Range, MovementSpeed, Stamina }
    public enum StatValueType { Stat, ScalingStat }

    public class NetworkedStatServiceLocator
    {
        private readonly Dictionary<string, IUnitStat> _services = new Dictionary<string, IUnitStat>();

        public NetworkedStatServiceLocator() { }
        
        public bool TryAddLocalValue(StatType statType, StatValueType statValueType, float value)
        {
            string key = nameof(LocalModificationModificationStat) + statType;
    
            if (!_services.ContainsKey(key))
            {
                return false;
            }

            ((LocalModificationModificationStat)_services[key]).AddStatModificationValue(statValueType, value);
            return true;
        }
        
        public bool TryRemoveLocalValue(StatType statType, StatValueType statValueType, float value)
        {
            string key = nameof(LocalModificationModificationStat) + statType;
    
            if (!_services.ContainsKey(key))
            {
                return false;
            }

            return ((LocalModificationModificationStat)_services[key]).TryRemoveStatModificationValue(statValueType, value);
        }

        public void RemoveAllValues()
        {
            foreach (KeyValuePair<string, IUnitStat> service in _services)
            {
                if (service.Value is LocalModificationModificationStat localStat)
                {
                    localStat.RemoveFromNetwork();
                }
            }
            
            _services.Clear();
        }

        public float GetTotalValue_CheckMin(StatType statType)
        {
            float finalBaseValue = 0;
            float finalScaleValue = 0;
    
            if (TryGetService(out NetworkModificationStat networkStat, statType))
            {
                finalBaseValue += networkStat.GetBaseStat();
                finalScaleValue += networkStat.GetMultiplierStat();
            }
            
            if (TryGetService(out LocalModificationModificationStat localStat, statType))
            {
                finalBaseValue += localStat.GetBaseStat();
                finalScaleValue += localStat.GetMultiplierStat();
            }

            if (TryGetService(out BaseStat baseStat, statType))
            {
                finalBaseValue += baseStat.GetBaseStat();
                finalScaleValue += baseStat.GetMultiplierStat();

                //make sure a stat cant get below min Stat Value
                return Mathf.Max(finalBaseValue * finalScaleValue, baseStat.GetMinValue());
            }
            
            Debug.LogWarning($"There is no {typeof(BaseStat)} Registered inside the {this}");
            return finalBaseValue * finalScaleValue;
        }

        private bool TryGetService<T>(out T service, StatType statType) where T : IUnitStat
        {
            string key = typeof(T).Name + statType;
    
            if (!_services.ContainsKey(key))
            {
                //Debug.LogWarning($"{key} not registered with {GetType().Name}");
                service = default;
                return false;
            }

            service = (T)_services[key];
            return true;
        }

        public T Get<T>(StatType statType) where T : IUnitStat
        {
            string key = typeof(T).Name + statType;
            if (!_services.ContainsKey(key))
            {
                Debug.LogWarning($"{key} not registered with {GetType().Name}");
            }

            return (T)_services[key];
        }

        public void Register<T>(T service) where T : IUnitStat
        {
            string key = typeof(T).Name + service.StatType;
            if (_services.ContainsKey(key))
            {
                Debug.LogWarning($"Attempted to register service of type {key} which is already registered with the {GetType().Name}.");
                return;
            }

            _services.Add(key, service);
        }

        public void Unregister<T>(StatType statType) where T : IUnitStat
        {
            string key = typeof(T).Name + statType;
            if (!_services.ContainsKey(key))
            {
                Debug.LogWarning($"Attempted to unregister service of type {key} which is not registered with the {GetType().Name}.");
                return;
            }

            _services.Remove(key);
        }

        public void Exchange<T>(T service) where T : IUnitStat
        {
            string key = typeof(T).Name + service.StatType;
            if (_services.ContainsKey(key))
            {
                _services.Remove(key);
            }

            if (!_services.ContainsKey(key))
            {
                _services.Add(key, service);
            }
        }
    }
}