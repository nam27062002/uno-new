using System;
using _Scripts.LoadScene;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Server.Connect
{
    public class CreateOrJoinRoom : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_InputField nicknameInput;
        [SerializeField] private TMP_InputField idRoomInput;
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private TextMeshProUGUI roomIdText;
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button joinRoomButton;
        private void Start()
        {
            createRoomButton.onClick.AddListener(CreateRoom);
            joinRoomButton.onClick.AddListener(JoinRoom);
            nicknameInput.ActivateInputField();
        }
        private void Update()
        {
            if (nicknameInput.isFocused && Input.GetKeyDown(KeyCode.Tab))
            {
                idRoomInput.ActivateInputField();
            }
        }
        
        private void CreateRoom()
        {
            PhotonNetwork.NickName = nicknameText.text;
            PhotonNetwork.CreateRoom(roomIdText.text, new RoomOptions { MaxPlayers = 4 });
        }

        private void JoinRoom()
        {
            PhotonNetwork.NickName = nicknameText.text;
            PhotonNetwork.JoinRoom(roomIdText.text);
        }

        public override void OnJoinedRoom()
        {
            LoaderManager.Instance.Load(SceneName.Room,true);
        }

        public override void OnCreatedRoom()
        {
            LoaderManager.Instance.Load(SceneName.Room,true);
        }
    }
}
