using ExitGames.Client.Photon;
using Features.Connection;
using Features.Connection.Scripts.Utils;
using UnityEngine;

namespace Features.Unit.Scripts.Behaviours.Stat
{
    public class BaseStats
    {
        private float _baseAttack;
        private float _baseHealth;
        private float _baseSpeed;

        public BaseStats(float baseAttack, float baseHealth, float baseSpeed)
        {
            _baseAttack = baseAttack;
            _baseHealth = baseHealth;
            _baseSpeed = baseSpeed;
        }

        public void ApplyBaseStats(NetworkedStatsBehaviour networkedStatsBehaviour)
        {
            networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Damage, _baseAttack);
            networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Health, _baseHealth);
            networkedStatsBehaviour.NetworkedStatServiceLocator.SetBaseValue(StatType.Speed, _baseSpeed);
        }
        
        #region Custom De/Serializer Methods
        
        [RuntimeInitializeOnLoadMethod]
        private static void Register()
        {
            PhotonPeer.RegisterType(typeof(BaseStats), UniqueCustomTypeByte.GetByteExcludingPhoton(), SerializeBaseStats, DeserializeBaseStats);
        }

        private const int SizeSynchronizedBaseStats = 3 * 4;
        public static readonly byte[] memVector3Int = new byte[SizeSynchronizedBaseStats];

        private static short SerializeBaseStats(StreamBuffer outStream, object customObject)
        {
            BaseStats vo = (BaseStats) customObject;

            int index = 0;
            lock (memVector3Int)
            {
                byte[] bytes = memVector3Int;
                Protocol.Serialize(vo._baseAttack, bytes, ref index);
                Protocol.Serialize(vo._baseHealth, bytes, ref index);
                Protocol.Serialize(vo._baseSpeed, bytes, ref index);
                outStream.Write(bytes, 0, SizeSynchronizedBaseStats);
            }

            return SizeSynchronizedBaseStats;
        }

        private static object DeserializeBaseStats(StreamBuffer inStream, short length)
        {
            BaseStats vo = new BaseStats(0, 0, 0);
            if (length != SizeSynchronizedBaseStats)
            {
                return vo;
            }

            lock (memVector3Int)
            {
                inStream.Read(memVector3Int, 0, SizeSynchronizedBaseStats);
                int index = 0;
                Protocol.Deserialize(out vo._baseAttack, memVector3Int, ref index);
                Protocol.Deserialize(out vo._baseHealth, memVector3Int, ref index);
                Protocol.Deserialize(out vo._baseSpeed, memVector3Int, ref index);
            }

            return vo;
        }

        #endregion
    }
}
