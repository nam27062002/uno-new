using System.Collections.Generic;
using _Scripts.Server.Gameplay;
using _Scripts.UI;
using _Scripts.Utils;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Server.Connect
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Button quickButton;
        [SerializeField] private Button readyButton;
        private List<UiPlayerSingleManager> uiPlayerSingleManagers;
        
        protected override void Awake()
        {
            uiPlayerSingleManagers = new List<UiPlayerSingleManager>();
        }
        private void Start()
        {   
            playerPrefab.SetActive(false);
            DestroyAllChild();
            ServerManager.Instance.SendData(IdData.OnPlayerJoinedRoom,RpcTarget.MasterClient,PhotonNetwork.LocalPlayer,false);
            
            quickButton.onClick.AddListener(() =>
            {
                ServerManager.Instance.QuickRoom();
            });
            readyButton.onClick.AddListener(() =>
            {
                ServerManager.Instance.SendData(IdData.PlayerAlready,RpcTarget.All,PhotonNetwork.NickName);
            });
        }

        public void UploadLobby(Dictionary<string,bool> playerAlready)
        {
            DestroyAllChild();
            int index = 0;
            foreach (string nickname in playerAlready.Keys)
            {
                GameObject player = (index == 0) ? playerPrefab : Instantiate(playerPrefab, transform);
                UiPlayerSingleManager playerSingleManager = player.GetComponent<UiPlayerSingleManager>();
                uiPlayerSingleManagers.Add(playerSingleManager);
                playerSingleManager.SetNickName(nickname,nickname == PhotonNetwork.NickName);
                playerSingleManager.SetActiveReadyPrefab(playerAlready[nickname]);
                player.SetActive(true);
                index++;
            }
            Debug.Log("Uploaded Lobby");
        }

        private void DestroyAllChild()
        {
            foreach (Transform child in transform) 
            {
                if (child.gameObject != playerPrefab)
                {
                    Destroy(child.gameObject);
                }
            }
            uiPlayerSingleManagers.Clear();
        }
    }
}
 