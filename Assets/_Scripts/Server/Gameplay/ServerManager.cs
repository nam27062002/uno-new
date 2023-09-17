
using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Cards;
using _Scripts.LoadScene;
using _Scripts.Server.Connect;
using _Scripts.Utils;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace _Scripts.Server.Gameplay
{
    public enum IdData
    {
        PlayerAlready, // player already
        SetCardForAllClient, 
        OnPlayerJoinedRoom,  // player just joined room
        SetPlayersAlready, // set only a player 
        AddPlayerData,// set player for all players in the room
    }
    public class ServerManager : SingletonPun<ServerManager>
    {

    #region EVENT

    public event EventHandler<OnServerSetCardEventArgs> OnServerSetCard;
    public class OnServerSetCardEventArgs : EventArgs
    {
        public ColorsCard ColorsCard;
        public ValuesCard ValuesCard;
    }
    #endregion


    private PhotonView _photonView;
    
    private bool isMasterClient;

    public bool IsMasterClient => isMasterClient;
    // DATA LOBBY
    private Dictionary<string, bool> playerAlready;

    protected override void Awake()
    {
        base.Awake();
        playerAlready = new Dictionary<string, bool>();
        isMasterClient = PhotonNetwork.IsMasterClient;
        _photonView = GetComponent<PhotonView>();
    }

    #region SEND DATA

        public void SendData(IdData data, Player playerTarget, object parameter)
        {
            _photonView.RPC(data.ToString(), playerTarget, parameter);
        }
        public void SendData(IdData data, Player playerTarget, params object[] parameters)
        {
            _photonView.RPC(data.ToString(), playerTarget, parameters);
        }   
        public void SendData(IdData data, RpcTarget rpcTarget)
        {
            _photonView.RPC(data.ToString(), rpcTarget);
        }
        public void SendData(IdData data, RpcTarget rpcTarget, object parameter)
        {
            _photonView.RPC(data.ToString(), rpcTarget, parameter);
        }
        public void SendData(IdData data, RpcTarget rpcTarget, params object[] parameters)
        {
            _photonView.RPC(data.ToString(), rpcTarget, parameters);
        }

    #endregion

    #region RECEIVE DATA

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
    
    [PunRPC] // handle by master client
    private void OnPlayerJoinedRoom(Player player,bool active)
    {
        string nickname = player.NickName;
        Debug.Log($"Player with nickname <color=green>{nickname}</color> has joined the room");
        // check nickname has been existed in the room
        if (playerAlready.ContainsKey(nickname))
        {
            Debug.LogError($"Nickname: <color=green>{nickname}</color> has been used"); 
        }
        else
        {
            AddPlayerData(player.NickName,active);
            SendData(IdData.SetPlayersAlready,RpcTarget.All,playerAlready.Keys.ToArray(),playerAlready.Values.ToArray());
        }
    }

    [PunRPC]
    private void SetPlayersAlready(string[] nicknames,bool[] actives)
    {
        if (nicknames.Length == actives.Length)
        {
            for (int i = 0; i < nicknames.Length; i++)
            {
                playerAlready[nicknames[i]] = actives[i];
            }
        }
        else
        {
            Debug.LogError("Length of nicknames and actives not same");
        }
        LobbyManager.Instance.UploadLobby(playerAlready);
    }
    
    [PunRPC]
    private void PlayerAlready(string nickname)
    {
        if (playerAlready.ContainsKey(nickname))
        {
            playerAlready[nickname] = !playerAlready[nickname];
            LobbyManager.Instance.UploadLobby(playerAlready);
        }
    }
    
    private void AddPlayerData(string nickname,bool active)
    {
        if (playerAlready.ContainsKey(nickname))
        {
            Debug.LogError($"Nickname: <color=green>{nickname}</color> has been used");
        }
        else
        {
            playerAlready[nickname] = active;
            Debug.Log($"Added successfully nickname: <color=green>{nickname}</color> active: <color=green>{active}</color>");
        }
    }
    #endregion
    
    #region EventLeftRoom
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (playerAlready.ContainsKey(otherPlayer.NickName))
        {
            playerAlready.Remove(otherPlayer.NickName);
            LobbyManager.Instance.UploadLobby(playerAlready);
        }
    }
            
    public void QuickRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
            
    public override void OnLeftRoom()
    {
        PhotonNetwork.JoinLobby();
        LoaderManager.Instance.Load(SceneName.Lobby);
    }
    #endregion
    
    #region REUSE

    private void ShowPlayerAlready()
    {
        Debug.Log($"Length of PlayerAlready: <color=green>{playerAlready.Count}</color>");
        foreach (string nickname in playerAlready.Keys)
        {
            Debug.Log($"Nickname <color=green>{nickname}</color> Status: <color=green>{playerAlready[nickname]}</color>");
        }
    }
    
    #endregion
    }
}

 