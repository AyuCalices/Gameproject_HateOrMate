using System;
using System.Linq;
using System.Text;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Features.Loot.Scripts
{
    public abstract class LootableGenerator_SO : ScriptableObjectWithId
    {
        [field: SerializeField] private string lootableName;
        [field: SerializeField] private string description;

        public string LootableName => lootableName;
        public string Description => description;
        
        /// <summary>
        /// Registers all derived Types of this class to Photon (excluding this class)
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void Register()
        {
            //https://stackoverflow.com/questions/857705/get-all-derived-types-of-a-type
            Type[] derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(LootableGenerator_SO))
                ).ToArray();

            foreach (var derivedType in derivedTypes)
            {
                PhotonPeer.RegisterType(derivedType, UniqueCustomTypeByte.GetByteExcludingPhoton(), SerializeLootableGenerator, DeserializeLootableGenerator);
            }
        }

        private static object DeserializeLootableGenerator(byte[] data)
        {
            string internalId = Encoding.ASCII.GetString(data);
            if (GetByInternalId(internalId, out LootableGenerator_SO lootableGenerator))
            {
                return lootableGenerator;
            }
            
            return null;
        }

        private static byte[] SerializeLootableGenerator(object customType)
        {
            LootableGenerator_SO singleStatMod = (LootableGenerator_SO) customType;
            return Encoding.ASCII.GetBytes(singleStatMod.InternalID);
        }

        public abstract void OnAddInstanceToPlayer();
    }
}
