using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Stat
{
    public class NetworkStat : IUnitStat
    {
        public string StatIdentity { get; }
        public StatType StatType { get; }
        public string ScalingStatIdentity { get; }
    
        public NetworkStat(StatType statType, string scalingStatIdentity, string statIdentity)
        {
            StatType = statType;
            StatIdentity = scalingStatIdentity;
            ScalingStatIdentity = statIdentity;
        }

        public virtual float GetTotalValue()
        {
            return GetStat() * GetScalingStat();
        }

        protected virtual float GetScalingStat()
        {
            float finalValue = 0;

            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey(ScalingStatIdentity))
                {
                    finalValue *= (float)PhotonNetwork.LocalPlayer.CustomProperties[ScalingStatIdentity];
                }
                else
                {
                    Debug.LogWarning("Stat not stored at server");
                }
            }

            return finalValue;
        }

        protected virtual float GetStat()
        {
            float finalValue = 0;

            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.ContainsKey(StatIdentity))
                {
                    finalValue = (float) PhotonNetwork.LocalPlayer.CustomProperties[StatIdentity];
                }
                else
                {
                    Debug.LogWarning("Stat not stored at server");
                }
            }

            return finalValue;
        }
    }
}
