using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.Unit.Modding.Stat
{
    public enum StatType { Damage, Health, Speed }
    public enum StatValueType { Stat, ScalingStat }

    public class NetworkedStatServiceLocator
    {
        private readonly Dictionary<string, IUnitStat> _services = new Dictionary<string, IUnitStat>();

        public NetworkedStatServiceLocator() { }

        public bool TryAddLocalValue(StatType statType, StatValueType statValueType, float value)
        {
            string key = nameof(LocalStat) + statType;
    
            if (!_services.ContainsKey(key))
            {
                return false;
            }

            ((LocalStat)_services[key]).AddStatValue(statValueType, value);
            return true;
        }
        
        public bool TryRemoveLocalValue(StatType statType, StatValueType statValueType, float value)
        {
            string key = nameof(LocalStat) + statType;
    
            if (!_services.ContainsKey(key))
            {
                return false;
            }

            return ((LocalStat)_services[key]).TryRemoveStatValue(statValueType, value);
        }

        public void RemoveAllValues()
        {
            foreach (KeyValuePair<string, IUnitStat> service in _services)
            {
                if (service.Value is LocalStat localStat)
                {
                    localStat.RemoveAll();
                }
            }
        }

        public float GetTotalValue(StatType statType)
        {
            float finalValue = 0;
    
            if (TryGetService(out NetworkStat networkStat, statType))
            {
                finalValue += networkStat.GetTotalValue();
            }
            if (TryGetService(out LocalStat localStat, statType))
            {
                finalValue += localStat.GetTotalValue();
            }

            return finalValue;
        }

        public bool TryGetService<T>(out T service, StatType statType) where T : IUnitStat
        {
            string key = typeof(T).Name + statType;
    
            if (!_services.ContainsKey(key))
            {
                Debug.LogWarning($"{key} not registered with {GetType().Name}");
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
                throw new InvalidOperationException();
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
            string oldKey = typeof(T).Name + service.StatType;
            if (_services.ContainsKey(oldKey))
            {
                _services.Remove(oldKey);
            }

            string newKey = typeof(T).Name;
            if (!_services.ContainsKey(newKey))
            {
                _services.Add(newKey, service);
            }
        }
    }
}