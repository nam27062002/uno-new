using _Scripts.LoadScene;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace _Scripts.Server.Connect
{
    public class ConnectToServer : MonoBehaviourPunCallbacks
    {
        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }
        
        public override void OnJoinedLobby()
        {
            LoaderManager.Instance.Load(SceneName.Lobby);
        }
    }
}
