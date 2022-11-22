using Photon.Pun;
using UnityEditor;
using UnityEngine;

namespace Features.Unit
{
    public class UnitSpawner : MonoBehaviourPunCallbacks
    {
        public void Spawn()
        {
            object byteArray = GUID.Generate().ToString();
            object[] myData = new object[2];
            myData[0] = byteArray;
            myData[1] = PhotonNetwork.LocalPlayer;
        
            Debug.Log(myData[0]);
            PhotonNetwork.Instantiate("Unit", new Vector3(Random.Range(0, 5), Random.Range(0, 5), 0), Quaternion.identity, 0, myData);
        }
    }
}
