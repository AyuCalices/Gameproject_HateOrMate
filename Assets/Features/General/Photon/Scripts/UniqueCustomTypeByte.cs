using System.Text;
using UnityEngine;

namespace Features.General.Photon.Scripts
{
    public static class UniqueCustomTypeByte
    {
        private static byte _acc;

        public static byte GetByteExcludingPhoton()
        {
            _acc++;
        
            //Lock Photon Bytes
            if (Encoding.ASCII.GetBytes("W")[0] == _acc)
            {
                _acc++;
            }
            if (Encoding.ASCII.GetBytes("V")[0] == _acc)
            {
                _acc++;
            }
            if (Encoding.ASCII.GetBytes("Q")[0] == _acc)
            {
                _acc++;
            }
            if (Encoding.ASCII.GetBytes("P")[0] == _acc)
            {
                _acc++;
            }

            if (_acc >= byte.MaxValue)
            {
                Debug.LogError("All Bytes Available Used!");
            }

            return _acc;
        }
    }
}