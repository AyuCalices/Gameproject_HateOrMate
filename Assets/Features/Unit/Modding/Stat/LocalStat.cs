using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Modding.Stat
{
    /// <summary>
    /// Edge Case Speed:
    /// pushing values to statValues will change speed by seconds (1f == 1 second)
    /// you need to add negative values to scaling Stat to perform this (scale 1f - 0.3f = 0,7f attack speed)
    /// </summary>
    public class LocalStat : NetworkStat
    {
        private float _baseStatValue;
        private readonly float _baseScalingStatValue;
        
        private readonly List<float> _statModificationValues;
        private readonly List<float> _scalingStatModificationValues;
    
        public LocalStat(StatType statType, string scalingStatIdentity, string statIdentity) : base(statType, scalingStatIdentity, statIdentity)
        {
            _baseScalingStatValue = 1;
            
            _statModificationValues = new List<float>() {};
            UpdateStat(statIdentity, _statModificationValues.ToArray());
        
            _scalingStatModificationValues = new List<float>() {};
            UpdateStat(scalingStatIdentity, _scalingStatModificationValues.ToArray());
        }

        public void SetBaseStatValue(float newBaseValue)
        {
            _baseStatValue = newBaseValue;
        }
    
        private void UpdateStat(string identity, float[] value)
        {
            Hashtable hash = new Hashtable {{identity, value}};
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    
        protected override float GetScalingStat()
        {
            float finalValue = _baseScalingStatValue;

            foreach (var statValue in _statModificationValues)
            {
                finalValue += statValue;
            }

            return finalValue;
        }

        protected override float GetStat()
        {
            float finalValue = _baseStatValue;
        
            foreach (var scalingStatValue in _scalingStatModificationValues)
            {
                finalValue += scalingStatValue;
            }

            return finalValue;
        }

        public void AddStatModificationValue(StatValueType statValueType, float value)
        {
            switch (statValueType)
            {
                case StatValueType.Stat:
                    _statModificationValues.Add(value);
                    UpdateStat(StatIdentity, _statModificationValues.ToArray());
                    break;
                case StatValueType.ScalingStat:
                    _scalingStatModificationValues.Add(value);
                    UpdateStat(ScalingStatIdentity, _scalingStatModificationValues.ToArray());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statValueType), statValueType, null);
            }
        }

        public bool TryRemoveStatModificationValue(StatValueType statValueType, float value)
        {
            return statValueType switch
            {
                StatValueType.Stat => TryRemoveStatValueInternal(value),
                StatValueType.ScalingStat => TryRemoveScalingStatValueInternal(value),
                _ => throw new ArgumentOutOfRangeException(nameof(statValueType), statValueType, null)
            };
        }

        public void RemoveAll()
        {
            _statModificationValues.Clear();
            _scalingStatModificationValues.Clear();
            
            UpdateStat(StatIdentity, _statModificationValues.ToArray());
            UpdateStat(ScalingStatIdentity, _scalingStatModificationValues.ToArray());
        }
    
        private bool TryRemoveStatValueInternal(float value)
        {
            bool result = this._statModificationValues.Remove(value);

            if (!result)
            {
                Debug.LogWarning($"Removing {value} failed because it is not listed in this Stat");
            }
            else
            {
                UpdateStat(StatIdentity, _statModificationValues.ToArray());
            }

            return result;
        }
    
        private bool TryRemoveScalingStatValueInternal(float value)
        {
            bool result = this._scalingStatModificationValues.Remove(value);

            if (!result)
            {
                Debug.LogWarning($"Removing {value} failed because it is not listed in this Stat");
            }
            else
            {
                UpdateStat(ScalingStatIdentity, _scalingStatModificationValues.ToArray());
            }

            return result;
        }
    }
}
