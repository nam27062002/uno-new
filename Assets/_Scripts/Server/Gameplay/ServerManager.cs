
using System;
using System.Collections.Generic;
using _Scripts.BEAN;
using _Scripts.Cards;
using _Scripts.Server.Connect;
using _Scripts.Utils;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace _Scripts.Server.Gameplay
{
    public enum IdData
    {
        PlayerLoadSuccessful,
        SetCardForAllClient,
        OnPlayerJoinRoom,
        SetListPlayerForAllClient,
        UploadLobby,
        SetListUserForOtherPlayer,
        SetUserJustJoined,
        
    }
    public class ServerManager : SingletonPun<ServerManager>
    {
        public event EventHandler<OnServerSetCardEventArgs> OnServerSetCard;

        public class OnServerSetCardEventArgs : EventArgs
        {
            public ColorsCard ColorsCard;
            public ValuesCard ValuesCard;
        }
        
        private new PhotonView photonView;
        
        private List<String> users;
        
        [SerializeField] private bool isMasterClient;
        public bool IsMasterClient => isMasterClient;
        protected override void Awake()
        {
            base.Awake();
            users = new List<string>();
            isMasterClient = PhotonNetwork.IsMasterClient;
            photonView = GetComponent<PhotonView>();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            LobbyManager.Instance.UploadLobby();
        }
        
        public void SendData(IdData data, Player playerTarget, object parameter)
        {
            photonView.RPC(data.ToString(), playerTarget, parameter);
        }
        
        public void SendData(IdData data, RpcTarget rpcTarget)
        {
            photonView.RPC(data.ToString(), rpcTarget);
        }
        public void SendData(IdData data, RpcTarget rpcTarget, object parameter)
        {
            photonView.RPC(data.ToString(), rpcTarget, parameter);
        }
        public void SendData(IdData data, RpcTarget rpcTarget, params object[] parameters)
        {
            photonView.RPC(data.ToString(), rpcTarget, parameters);
        }
        
        [PunRPC]
        private void SetCardForAllClient(params object[] parameters)
        {
            if (parameters == null || parameters.Length != 2)
            {
                Debug.LogError("Invalid parameters for SetCardForAllPlayer");
                return;
            }
            ColorsCard colorsCard = parameters[0] is ColorsCard ? (ColorsCard)parameters[0] : ColorsCard.BLACK;
            ValuesCard valueCard = parameters[1] is ValuesCard ? (ValuesCard)parameters[1] : ValuesCard.NUM0;
            OnServerSetCard?.Invoke(this,new OnServerSetCardEventArgs()
            {
                ColorsCard = colorsCard,
                ValuesCard = valueCard,
            });
        }

        [PunRPC]
        private void OnPlayerJoinRoom(Player player)
        {
            foreach (string user in users)
            {
                SendData(IdData.SetListUserForOtherPlayer,player,user);
            }
            SendData(IdData.SetUserJustJoined,RpcTarget.All,player.NickName);
        }

        [PunRPC]
        private void SetListUserForOtherPlayer(string user)
        {
            users.Add(user);
        }

        [PunRPC]
        private void SetUserJustJoined(string user)
        {
            users.Add(user);
            UploadLobby();
        }
        [PunRPC]
        private void UploadLobby()
        {
            LobbyManager.Instance.UploadLobby();
        }
    }
}

 