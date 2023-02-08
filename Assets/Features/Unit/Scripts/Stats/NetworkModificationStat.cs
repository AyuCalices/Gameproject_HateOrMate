using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace Features.Unit.Scripts.Stats
{
    public class NetworkModificationStat : IUnitStat
    {
        public string StatIdentity { get; }
        public StatType StatType { get; }
        public string MultiplierStatIdentity { get; }
    
        public NetworkModificationStat(StatType statType, string multiplierStatIdentity, string statIdentity)
        {
            StatType = statType;
            StatIdentity = statIdentity;
            MultiplierStatIdentity = multiplierStatIdentity;
        }

        public virtual float GetMultiplierStat()
        {
            float finalValue = 0;
            
            bool found = false;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player == PhotonNetwork.LocalPlayer) continue;
                
                if (player.CustomProperties.ContainsKey(MultiplierStatIdentity))
                {
                    float[] value = (float[])player.CustomProperties[MultiplierStatIdentity];
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

        public virtual float GetBaseStat()
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
