using System.Collections.Generic;
using _Scripts.Server.Gameplay;
using _Scripts.UI;
using _Scripts.Utils;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Server.Connect
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Button quickButton;
        [SerializeField] private Button readyButton;
        [SerializeField] private TextMeshProUGUI readyTextButton;
        [SerializeField] private TextMeshProUGUI errorText;
        protected override void Awake()
        {
            readyTextButton.text = ServerManager.Instance.IsMasterClient ? "START" : "READY";
        }
        private void Start()
        {   
            DestroyAllChild();
            SetErrorText("");
            ServerManager.Instance.SendData(IdData.OnPlayerJoinedRoom,RpcTarget.MasterClient,PhotonNetwork.LocalPlayer,ServerManager.Instance.IsMasterClient);
            
            quickButton.onClick.AddListener(() =>
            {
                ServerManager.Instance.QuickRoom();
            });
            readyButton.onClick.AddListener(() =>
            {
                if (!ServerManager.Instance.IsMasterClient) ServerManager.Instance.SendData(IdData.PlayerAlready,RpcTarget.All,PhotonNetwork.NickName);
                else ServerManager.Instance.SendData(IdData.StartGame,RpcTarget.MasterClient);
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
        }

        public void SetErrorText(string message)
        {
            errorText.text = message;
        }
    }
}
 