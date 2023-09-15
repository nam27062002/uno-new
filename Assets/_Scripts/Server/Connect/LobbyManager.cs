using System;
using System.Collections.Generic;
using _Scripts.BEAN;
using _Scripts.Server.Gameplay;
using _Scripts.UI;
using _Scripts.Utils;
using Photon.Pun;
using UnityEngine;

namespace _Scripts.Server.Connect
{
    public class LobbyManager : Singleton<LobbyManager>
    {
        [SerializeField] private GameObject playerPrefab;
        private List<UiPlayerSingleManager> uiPlayerSingleManagers;

        protected override void Awake()
        {
            uiPlayerSingleManagers = new List<UiPlayerSingleManager>();
        }
        private void Start()
        {   
            playerPrefab.SetActive(false);
            DestroyAllChild();
            ServerManager.Instance.SendData(IdData.OnPlayerJoinRoom,RpcTarget.MasterClient,PhotonNetwork.LocalPlayer);
        }

        public void UploadLobby()
        {
            DestroyAllChild();
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                GameObject player = (i == 0) ? playerPrefab : Instantiate(playerPrefab, transform);
                UiPlayerSingleManager playerSingleManager = player.GetComponent<UiPlayerSingleManager>();
                uiPlayerSingleManagers.Add(playerSingleManager);
                playerSingleManager.SetNickName(PhotonNetwork.PlayerList[i].NickName,PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.NickName);
                player.SetActive(true);
            }
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
 