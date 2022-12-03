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
        private readonly List<float> statValues;
        private readonly List<float> scalingStatValues;
    
        public LocalStat(StatType statType, string scalingStatIdentity, string statIdentity) : base(statType, scalingStatIdentity, statIdentity)
        {
            statValues = new List<float>() {};
            UpdateStat(statIdentity, statValues.ToArray());
        
            scalingStatValues = new List<float>() {};
            UpdateStat(scalingStatIdentity, scalingStatValues.ToArray());
        }
    
        private void UpdateStat(string identity, float[] value)
        {
            Hashtable hash = new Hashtable {{identity, value}};
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    
        protected override float GetScalingStat()
        {
            float finalValue = 0;

            foreach (var statValue in statValues)
            {
                finalValue += statValue;
            }

            return finalValue;
        }

        protected override float GetStat()
        {
            float finalValue = 0;
        
            foreach (var scalingStatValue in scalingStatValues)
            {
                finalValue += scalingStatValue;
            }

            return finalValue;
        }

        public void AddStatValue(StatValueType statValueType, float value)
        {
            switch (statValueType)
            {
                case StatValueType.Stat:
                    statValues.Add(value);
                    UpdateStat(StatIdentity, statValues.ToArray());
                    break;
                case StatValueType.ScalingStat:
                    scalingStatValues.Add(value);
                    UpdateStat(ScalingStatIdentity, scalingStatValues.ToArray());
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

        public void RemoveAll()
        {
            statValues.Clear();
            scalingStatValues.Clear();
            
            UpdateStat(StatIdentity, statValues.ToArray());
            UpdateStat(ScalingStatIdentity, scalingStatValues.ToArray());
        }
    
        private bool TryRemoveStatValueInternal(float value)
        {
            bool result = this.statValues.Remove(value);

            if (!result)
            {
                Debug.LogWarning($"Removing {value} failed because it is not listed in this Stat");
            }
            else
            {
                UpdateStat(StatIdentity, statValues.ToArray());
            }

            return result;
        }
    
        private bool TryRemoveScalingStatValueInternal(float value)
        {
            bool result = this.scalingStatValues.Remove(value);

            if (!result)
            {
                Debug.LogWarning($"Removing {value} failed because it is not listed in this Stat");
            }
            else
            {
                UpdateStat(ScalingStatIdentity, scalingStatValues.ToArray());
            }

            return result;
        }
    }
}
