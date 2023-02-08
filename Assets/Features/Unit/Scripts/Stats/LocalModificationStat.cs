using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.Stats
{
    /// <summary>
    /// Edge Case Speed:
    /// pushing values to statValues will change speed by seconds (1f == 1 second)
    /// you need to add negative values to scaling Stat to perform this (scale 1f - 0.3f = 0,7f attack speed)
    /// </summary>
    public class LocalModificationStat : NetworkModificationStat, IChangeableStat
    {
        private readonly List<float> _statModificationValues;
        private readonly List<float> _multiplierStatModificationValues;
    
        public LocalModificationStat(StatType statType, string multiplierStatIdentity, string statIdentity) : base(statType, multiplierStatIdentity, statIdentity)
        {
            _statModificationValues = new List<float>();
            UpdateStat(statIdentity, _statModificationValues.ToArray());
        
            _multiplierStatModificationValues = new List<float>();
            UpdateStat(multiplierStatIdentity, _multiplierStatModificationValues.ToArray());
        }
    
        private void UpdateStat(string identity, float[] value)
        {
            Hashtable hash = new Hashtable {{identity, value}};
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    
        public override float GetMultiplierStat()
        {
            float finalValue = 0;

            foreach (var scalingStatValue in _multiplierStatModificationValues)
            {
                finalValue += scalingStatValue;
            }
            
            return finalValue;
        }

        public override float GetBaseStat()
        {
            float finalValue = 0;
        
            foreach (var statValue in _statModificationValues)
            {
                finalValue += statValue;
            }

            return finalValue;
        }

        public void SetStatValue(StatValueType statValueType, float value)
        {
            switch (statValueType)
            {
                case StatValueType.Stat:
                    _statModificationValues.Add(value);
                    UpdateStat(StatIdentity, _statModificationValues.ToArray());
                    break;
                case StatValueType.ScalingStat:
                    _multiplierStatModificationValues.Add(value);
                    UpdateStat(MultiplierStatIdentity, _multiplierStatModificationValues.ToArray());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(statValueType), statValueType, null);
            }
        }

        public bool TryRemoveStatValue(StatValueType statValueType, float value)
        {
            return statValueType switch
            {
                StatValueType.Stat => TryRemoveStatValueInternal(value),
                StatValueType.ScalingStat => TryRemoveScalingStatValueInternal(value),
                _ => throw new ArgumentOutOfRangeException(nameof(statValueType), statValueType, null)
            };
        }

        public void RemoveFromNetwork()
        {
            _statModificationValues.Clear();
            _multiplierStatModificationValues.Clear();

            PhotonNetwork.LocalPlayer.CustomProperties.Remove(StatIdentity);
            PhotonNetwork.LocalPlayer.CustomProperties.Remove(MultiplierStatIdentity);
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
            bool result = this._multiplierStatModificationValues.Remove(value);

            if (!result)
            {
                Debug.LogWarning($"Removing {value} failed because it is not listed in this Stat");
            }
            else
            {
                UpdateStat(MultiplierStatIdentity, _multiplierStatModificationValues.ToArray());
            }

            return result;
        }
    }
}
