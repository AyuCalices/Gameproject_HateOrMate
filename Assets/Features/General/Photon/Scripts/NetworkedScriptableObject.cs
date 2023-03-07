using System;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Features.General.Photon.Scripts
{
    public class NetworkedScriptableObject : ScriptableObjectWithId
    {
        /// <summary>
        /// Registers all derived Types of this class to Photon (excluding this class)
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void Register()
        {
            //https://stackoverflow.com/questions/857705/get-all-derived-types-of-a-type
            Type[] derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(NetworkedScriptableObject))
                ).ToArray();

            foreach (var derivedType in derivedTypes)
            {
                PhotonPeer.RegisterType(derivedType, UniqueCustomTypeByte.GetByteExcludingPhoton(), SerializeLootableGenerator, DeserializeLootableGenerator);
            }
        }

        private static object DeserializeLootableGenerator(byte[] data)
        {
            string internalId = Encoding.ASCII.GetString(data);
            if (GetByInternalId(internalId, out NetworkedScriptableObject lootableGenerator))
            {
                return lootableGenerator;
            }
            
            return null;
        }

        private static byte[] SerializeLootableGenerator(object customType)
        {
            NetworkedScriptableObject singleStatMod = (NetworkedScriptableObject) customType;
            return Encoding.ASCII.GetBytes(singleStatMod.InternalID);
        }
    }
}
