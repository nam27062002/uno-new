using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.BEAN;
using _Scripts.LoadScene;
using _Scripts.Server.Connect;
using _Scripts.UI;
using _Scripts.Utils;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace _Scripts.Server.Gameplay
{
    public enum IdData
    {
        PlayerAlready, // player already
        OnPlayerJoinedRoom,  // player just joined room
        SetPlayersAlready, // set only a player 
        StartGame,
        SendMessageInRoom, 
        ShowCountDownRoom,
        SetCardsForAllClient,
        PlayerLoadedGame,
        CreateCardSuccessfully,
    }
    public class ServerManager : SingletonPun<ServerManager>
    {
        
    private PhotonView _photonView;
    
    private bool isMasterClient;

    public bool IsMasterClient => isMasterClient;
    // DATA LOBBY
    private Dictionary<string, bool> playerAlready;
    private List<MessageData> messageDatas;
    protected override void Awake()
    {
        base.Awake();
        playerAlready = new Dictionary<string, bool>();
        isMasterClient = PhotonNetwork.IsMasterClient;
        _photonView = GetComponent<PhotonView>();
        messageDatas = new List<MessageData>();
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
        SetAllMessages(player);
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
        if (RoomManager.Instance.IsNormalState)
        {
            if (playerAlready.ContainsKey(nickname))
            {
                playerAlready[nickname] = !playerAlready[nickname];
                LobbyManager.Instance.UploadLobby(playerAlready);
            }
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

    [PunRPC] // Master client
    private void StartGame()
    {
        if (!isMasterClient || !RoomManager.Instance.IsNormalState) return;
        if (playerAlready.Count <= 0)
        {
            LobbyManager.Instance.SetErrorText("A minimum of 2 players is required to start the game");
        }
        
        else if (playerAlready.ContainsValue(false))
        {
            LobbyManager.Instance.SetErrorText("There are players who are not ready yet");
        }
        else
        {
            SendData(IdData.ShowCountDownRoom,RpcTarget.All);
        }
    }
    
    [PunRPC] // ALL
    private void ShowCountDownRoom()
    {
        RoomManager.Instance.ChangeStateRoom();
    }
    
    [PunRPC] // ALL 
    private void SendMessageInRoom(Player player, string message)
    {
        if (isMasterClient)
        {
            messageDatas.Add(new MessageData(player,message,DateTime.Now));
        }
        ChatManager.Instance.ShowMessage(player.NickName,message,PhotonNetwork.LocalPlayer.Equals(player));
    }
    
    [PunRPC] // ALL
    private void SetCardsForAllClient(string color,string value)
    {
        if(!isMasterClient) Desk.Instance.SetCards(color,value);
    }

    [PunRPC] // Master client
    private void PlayerLoadedGame()
    {
        GameManager.Instance.IncreasePlayerLoadedGame(playerAlready.Count);
    }
    [PunRPC]
    private void CreateCardSuccessfully()
    {
        GameManager.Instance.IncreasePlayerCreateCard(playerAlready.Count);
    }

    [PunRPC] // ALL
    private void CreateCard()
    {
        Desk.Instance.CreateCard();
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
        if (RoomManager.Instance.IsNormalState) PhotonNetwork.LeaveRoom();
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

    private void SetAllMessages(Player player)
    {
        Debug.Log($"NT - {messageDatas.Count}");
        foreach (MessageData messageData in messageDatas)
        {
            SendData(IdData.SendMessageInRoom,player,messageData.Player,messageData.Message);
        }
    }
    
    #endregion
    }
}

 