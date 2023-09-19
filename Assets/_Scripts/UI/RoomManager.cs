using System.Collections;
using _Scripts.LoadScene;
using _Scripts.Server.Gameplay;
using _Scripts.Utils;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class RoomManager : Singleton<RoomManager>
    {
        [SerializeField] private GameObject serverManagerObject;
        [SerializeField] private GameObject countdownObject;
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private int timeCountdown = 3;
        private enum StateRoom
        {
            Normal,
            Countdown,
        }

        private StateRoom state;
        protected override void Awake()
        {
            base.Awake();
            state = StateRoom.Normal;
        }

        public void ChangeStateRoom()
        {
            state = StateRoom.Countdown;
            StartCoroutine(CountDown());
        }
        public bool IsNormalState => state == StateRoom.Normal;

        private void LoadSceneGame()
        {    
            DontDestroyOnLoad(serverManagerObject);
            PhotonNetwork.AutomaticallySyncScene = true;
            if (ServerManager.Instance.IsMasterClient)
            {
                LoaderManager.Instance.Load(SceneName.Game, true);
                
            }
        }
        IEnumerator CountDown()
        {
            countdownObject.SetActive(true);
            while (timeCountdown >= 0)
            {
                countdownText.text = timeCountdown.ToString();
                timeCountdown--;
                yield return new WaitForSeconds(1f);
            }

            LoadSceneGame();
            yield return null;
        }
    }
}
