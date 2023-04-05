using System;
using System.Collections.Generic;
using System.Linq;
using Features.Unit.Scripts.Behaviours.Services.UnitStats.StatTypes;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Services.UnitStats
{
    public enum StatType { Damage, Health, Speed, Range, MovementSpeed, Stamina }
    public enum StatValueType { Stat, ScalingStat, MinStat, MinScalingStat }

    //TODO: some methods are not generic enough
    public class StatServiceLocator
    {
        public Action<StatType> onUpdateStat;
        
        private readonly Dictionary<string, IUnitStat> _services = new Dictionary<string, IUnitStat>();

        public StatServiceLocator() { }
        
        public bool TrySetStatValue<T>(StatType statType, StatValueType statValueType, float value = 0) where T : IChangeableStat
        {
            string key = typeof(T).Name + statType;
    
            if (!_services.ContainsKey(key))
            {
                return false;
            }

            ((T)_services[key]).SetStatValue(statValueType, value);
            onUpdateStat?.Invoke(statType);
            return true;
        }
        
        public bool TryRemoveStatValue<T>(StatType statType, StatValueType statValueType, float value = 0) where T : IChangeableStat
        {
            string key = typeof(T).Name + statType;
    
            if (!_services.ContainsKey(key))
            {
                return false;
            }

            bool result = ((T)_services[key]).TryRemoveStatValue(statValueType, value);
            
            if (!result) return false;
            
            onUpdateStat?.Invoke(statType);
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
        
        public void UnregisterAll()
        {
            foreach (KeyValuePair<string, IUnitStat> service in _services)
            {
                if (service.Value is LocalModificationStat localStat)
                {
                    localStat.RemoveFromNetwork();
                }
            }
            
            _services.Clear();
        }

        public float GetTotalBaseValue(StatType statType)
        {
            return _services.Select(keyValuePair => keyValuePair.Value)
                .Where(unitStat => unitStat.StatType == statType)
                .Sum(unitStat => unitStat.GetBaseStat());
        }
        
        public float GetTotalMultiplierValue(StatType statType)
        {
            return _services.Select(keyValuePair => keyValuePair.Value)
                .Where(unitStat => unitStat.StatType == statType)
                .Sum(unitStat => unitStat.GetMultiplierStat());
        }

        public bool TryGetService<T>(out T service, StatType statType) where T : IUnitStat
        {
            string key = typeof(T).Name + statType;
    
            if (!_services.ContainsKey(key))
            {
                service = default;
                return false;
            }

            service = (T)_services[key];
            return true;
        }
    }
}