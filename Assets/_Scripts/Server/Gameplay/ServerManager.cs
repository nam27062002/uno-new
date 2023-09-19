using System;
using System.Collections;
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
        DiscardToPlayer,
        FinishedDistributeCard,
    }
    public class ServerManager : SingletonPun<ServerManager>
    {
        
    private new PhotonView photonView;
    
    private bool isMasterClient;

    public bool IsMasterClient => isMasterClient;
    
    // DATA LOBBY
    private Dictionary<string, bool> playerAlready;
    private List<MessageData> messageDatas;
    private Dictionary<string, int> playerOrder;
    private Dictionary<int, string> playerOderReverse;
    public Dictionary<string, int> PlayerOrder => playerOrder;
    
    private int currentIndex = 0;
    protected override void Awake()
    {
        base.Awake();
        playerOrder = new Dictionary<string, int>();
        playerOderReverse = new Dictionary<int, string>();
        playerAlready = new Dictionary<string, bool>();
        isMasterClient = PhotonNetwork.IsMasterClient;
        photonView = GetComponent<PhotonView>();
        messageDatas = new List<MessageData>();
    }
    
    #region SEND DATA

        public void SendData(IdData data, Player playerTarget, object parameter)
        {
            photonView.RPC(data.ToString(), playerTarget, parameter);
        }
        public void SendData(IdData data, Player playerTarget, params object[] parameters)
        {
            photonView.RPC(data.ToString(), playerTarget, parameters);
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
        GameManager.Instance.OnFinishLoadGame(playerAlready.Count,GameManager.StateGame.WaitingForLoading);
    }
    
    [PunRPC]
    private void CreateCardSuccessfully()
    {
        GameManager.Instance.OnFinishLoadGame(playerAlready.Count,GameManager.StateGame.WaitingLoadCard);
    }

    [PunRPC] // ALL
    private void CreateCard()
    {
        Desk.Instance.CreateCard();
    }
    
    
    
    [PunRPC] // ALL
    private IEnumerator DiscardToPlayer(string[] nicknames)
    {
        foreach (string nickname in nicknames)
        {
            CardUiManager.Instance.DisCard(nickname);
            yield return new WaitForSeconds(CardUiManager.Instance.TimeDiscard);
        }
        yield return new WaitForSeconds(CardUiManager.Instance.TimeDiscard);
        SendData(IdData.FinishedDistributeCard,RpcTarget.MasterClient);
    }

    [PunRPC] // Master client
    private void FinishedDistributeCard()
    {
        GameManager.Instance.OnFinishLoadGame(playerAlready.Count,GameManager.StateGame.WaitingDistributeCard);
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

    
    private void SetAllMessages(Player player)
    {
        Debug.Log($"NT - {messageDatas.Count}");
        foreach (MessageData messageData in messageDatas)
        {
            SendData(IdData.SendMessageInRoom,player,messageData.Player,messageData.Message);
        }
    }

    public void StartDistributeCard()
    {
        List<string> nicknames = new List<string>();
        for (int i = 0; i < playerOrder.Count * GameManager.CardAmountDefault; i++)
        {
            nicknames.Add(playerOderReverse[currentIndex]);
            currentIndex = (currentIndex + 1) % playerOrder.Count;
        }
        SendData(IdData.DiscardToPlayer,RpcTarget.All,nicknames.ToArray());
        currentIndex = 0;
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
    
    public void SetPlayerOrder()
    { 
        string myNickname = PhotonNetwork.NickName;
        int playerCount = PhotonNetwork.PlayerList.Length;
        int myIndex = -1;
        for (int i = 0; i < playerCount; i++)
        {
            if (PhotonNetwork.PlayerList[i].NickName == myNickname)
            {
                myIndex = i;
                break;
            }
        }

        if (myIndex == -1)
        {
            Debug.LogError("don't find nickname on listPlayer");
            return;
        }
    
        for (int i = 0; i < playerCount; i++)
        {
            int relativeIndex = (i - myIndex + playerCount) % playerCount;
            playerOrder[PhotonNetwork.PlayerList[i].NickName] = relativeIndex;
            playerOderReverse[relativeIndex] = PhotonNetwork.PlayerList[i].NickName;
        }
        foreach (string nickname in playerOrder.Keys)
        {
            Debug.Log($"Nickname: {nickname}  Index: {playerOrder[nickname]}");
        }
    }
    #endregion
    }
}

 