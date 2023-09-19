using System;
using _Scripts.Server.Gameplay;
using _Scripts.UI;
using _Scripts.Utils;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace _Scripts.Server
{
    public class ChatManager : Singleton<ChatManager>
    {
        [SerializeField] private TMP_InputField messageInput;
        [SerializeField] private GameObject messagePrefab;
        [SerializeField] private GameObject panelMessage;
 
        private void Start()
        {
            messageInput.onSubmit.AddListener( (_) =>
            {
                if (!RoomManager.Instance.IsNormalState) return;
                ServerManager.Instance.SendData(IdData.SendMessageInRoom,RpcTarget.All,PhotonNetwork.LocalPlayer,messageInput.text);
                messageInput.text = "";
                messageInput.ActivateInputField();
            });
        }

        public void ShowMessage(string nickname, string message,bool isMyMessage)
        {
            GameObject obj = Instantiate(messagePrefab, panelMessage.transform);
            obj.GetComponent<UIMessageSingleManager>().SetMessage(nickname,message,isMyMessage);
        }
    }
}
