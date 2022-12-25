using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Modding.Stat
{
    public class NetworkStat : IUnitStat
    {
        public string StatIdentity { get; }
        public StatType StatType { get; }
        public string ScalingStatIdentity { get; }
    
        public NetworkStat(StatType statType, string scalingStatIdentity, string statIdentity)
        {
            StatType = statType;
            StatIdentity = statIdentity;
            ScalingStatIdentity = scalingStatIdentity;
        }

        public float GetTotalValue()
        {
            return GetStat() * GetScalingStat();
        }

        protected virtual float GetScalingStat()
        {
            float finalValue = 0;
            
            bool found = false;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player == PhotonNetwork.LocalPlayer) continue;
                
                if (player.CustomProperties.ContainsKey(ScalingStatIdentity))
                {
                    float[] value = (float[])player.CustomProperties[ScalingStatIdentity];
                    finalValue = value.Sum();
                    found = true;
                }
            }
            
            if (!found)
            {
                Debug.LogWarning("Stat not stored at server");
            }

            return finalValue;
        }

        protected virtual float GetStat()
        {
            float finalValue = 0;

            bool found = false;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player == PhotonNetwork.LocalPlayer) continue;
                
                if (player.CustomProperties.ContainsKey(StatIdentity))
                {
                    float[] value = (float[])player.CustomProperties[StatIdentity];
                    finalValue = value.Sum();
                    found = true;
                }
            }

            if (!found)
            {
                Debug.LogWarning("Stat not stored at server");
            }

            return finalValue;
        }
    }
}
