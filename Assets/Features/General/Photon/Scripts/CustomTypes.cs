using ExitGames.Client.Photon;
using UnityEngine;

namespace Features.General.Photon.Scripts
{
    internal static class CustomTypesGame
    {
        private const int SizeV3Int = 3 * 4;


        /// <summary>Register de/serializer methods for Game specific types. Makes the types usable in RaiseEvent and PUN.</summary>
        [RuntimeInitializeOnLoadMethod]
        private static void Register()
        {
            PhotonPeer.RegisterType(typeof(Vector3Int), UniqueCustomTypeByte.GetByteExcludingPhoton(), SerializeVector3Int, DeserializeVector3Int);
        }


        #region Custom De/Serializer Methods

        public static readonly byte[] memVector3Int = new byte[SizeV3Int];

        private static short SerializeVector3Int(StreamBuffer outStream, object customobject)
        {
            Vector3Int vo = (Vector3Int) customobject;

            int index = 0;
            lock (memVector3Int)
            {
                byte[] bytes = memVector3Int;
                Protocol.Serialize(vo.x, bytes, ref index);
                Protocol.Serialize(vo.y, bytes, ref index);
                Protocol.Serialize(vo.z, bytes, ref index);
                outStream.Write(bytes, 0, SizeV3Int);
            }

            return SizeV3Int;
        }

        private static object DeserializeVector3Int(StreamBuffer inStream, short length)
        {
            Vector3Int vo = new Vector3Int();
            if (length != SizeV3Int)
            {
                return vo;
            }

            lock (memVector3Int)
            {
                inStream.Read(memVector3Int, 0, SizeV3Int);
                int index = 0;
                Protocol.Deserialize(out int x, memVector3Int, ref index);
                Protocol.Deserialize(out int y, memVector3Int, ref index);
                Protocol.Deserialize(out int z, memVector3Int, ref index);
                vo.x = x;
                vo.y = y;
                vo.z = z;
            }

            return vo;
        }

        #endregion
    }
}