using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

namespace Features.Connection
{
    public class ConnectToServer : MonoBehaviourPunCallbacks
    {
        public TMP_InputField usernameInput;
        public TMP_Text buttonText;

        public void OnClickConnect()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            SceneManager.LoadScene("Lobby");
            if (usernameInput.text.Length >= 1)
            {
                PhotonNetwork.NickName = usernameInput.text;
                buttonText.text = "Connecting...";
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                PhotonNetwork.NickName = LobbyRoomNameRandomizer.RandomString(5);
                buttonText.text = "Connecting...";
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        public override void OnConnectedToMaster()
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}
