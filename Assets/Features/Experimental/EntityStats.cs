using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Features.Unit;
using Features.Unit.Modding.Stat;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Features.Experimental
{
    public class EntityStats : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private TMP_InputField inputField;
    
        private EntityStatList entityStat;
    
    
        private void Awake()
        {
            PhotonPeer.RegisterType(typeof(EntityStatListSerialized), (byte)'Z', EntityStatListSerialized.Serialize, EntityStatListSerialized.Deserialize);
        }

        public EntityStatList GetValue(string key)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key))
            {
                return entityStat.SetEntityStatList((EntityStatListSerialized)PhotonNetwork.CurrentRoom.CustomProperties[key]);
            }

            return null;
        }

        public void Set()
        {
            PhotonView photonView = GetComponent<PhotonView>();
        
            photonView.RPC("ChatMessage", RpcTarget.All, StatType.Damage, 10f, Guid.NewGuid().ToString());
        }

        public void ChangeValue()
        {
            entityStat.AddValue(float.Parse(inputField.text));
        }
    
        [PunRPC]
        void ChatMessage(StatType statType, float value, string identity)
        {
            entityStat = new EntityStatList(statType, value, identity);

            //int randomValue = UnityEngine.Random.Range(0, 100);
            //Debug.Log(randomValue);
            //entityStat.AddValue(randomValue);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            //Debug.Log("o/");
            //text.text = GetValue(entityStat.Identity).GetTotalValue().ToString();
        }
    }

    public class EntityStatListSerialized
    {
        public readonly List<float> statValues;
        public float scalingStat;

        public EntityStatListSerialized(List<float> statValues, float scalingStat)
        {
            this.statValues = statValues;
            this.scalingStat = scalingStat;
        }
    
        public static object Deserialize(byte[] data)
        {
            List<float> values = new List<float>();
            for(int n = 0; n < data.Length; n+=4)
            {
                values.Add(BitConverter.ToSingle(data, n));
            }
            var result = new EntityStatListSerialized(values.GetRange(0, values.Count - 1), values[^1]);
            return result;
        }

        public static byte[] Serialize(object customType)
        {
            EntityStatListSerialized c = (EntityStatListSerialized)customType;
        
            List<byte> bytes = new List<byte>();
            foreach (float f in c.statValues)
            {
                byte[] t = BitConverter.GetBytes(f);
                bytes.AddRange(t);
            }
            bytes.AddRange(BitConverter.GetBytes(c.scalingStat));
        
            return bytes.ToArray();
        }
    }

    public class EntityStatList : BaseEntity
    {
        public StatType StatType { get; }
        public EntityStatListSerialized entityStatListSerialized;
    
        public EntityStatList(StatType statType, float statValues, string identity, float scalingStat = 1f) : base(identity)
        {
            StatType = statType;
            entityStatListSerialized = new EntityStatListSerialized(new List<float>() {statValues}, scalingStat);
            Hashtable hash = new Hashtable {{Identity, entityStatListSerialized}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }

        public EntityStatList SetEntityStatList(EntityStatListSerialized entityStatListSerialized)
        {
            this.entityStatListSerialized = entityStatListSerialized;
            return this;
        }

        public float GetTotalValue()
        {
            float finalValue = 0;
        
            foreach (float statValue in entityStatListSerialized.statValues)
            {
                finalValue += statValue;
            }

            return finalValue * entityStatListSerialized.scalingStat;
        }

        public void AddValue(float value)
        {
            entityStatListSerialized.statValues.Add(value);
        
            Hashtable newProps = new Hashtable {{Identity, entityStatListSerialized}};
            PhotonNetwork.CurrentRoom.SetCustomPropertySafe(Identity, newProps);
        }

        public bool TryRemoveValue(float value)
        {
            bool result = this.entityStatListSerialized.statValues.Remove(value);

            if (result)
            {
                Hashtable newProps = new Hashtable(1) {{Identity, entityStatListSerialized}};
                PhotonNetwork.CurrentRoom.SetCustomProperties(newProps);
            }
            else
            {
                Debug.LogWarning($"Removing {value} failed because it is not listed in this Stat");
            }

            return result;
        }
    }

    public static class RoomExtensions 
    {
        public static bool SetCustomPropertySafe(this Room room, string key, object newValue, WebFlags webFlags = null)
        {
            if (room == null)
            {
                return false;
            }
            if (room.IsOffline)
            {
                return false;
            }
            if (!room.CustomProperties.ContainsKey(key))
            {
                return false;
            }
            Hashtable newProps = new Hashtable(1) {{key, newValue}};
            Hashtable oldProps = new Hashtable(1) {{key, room.CustomProperties[key]}};
            return room.LoadBalancingClient.OpSetCustomPropertiesOfRoom(newProps, oldProps, webFlags);
        }
    }
}