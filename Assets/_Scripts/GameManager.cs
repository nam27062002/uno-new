using _Scripts.Server.Gameplay;
using _Scripts.Utils;
using Photon.Pun;
using UnityEngine;

namespace _Scripts
{
    public class GameManager : Singleton<GameManager>
    {
        private int playerFinished = 0;
        public const int CardAmountDefault = 8;
        public enum StateGame
        {
            WaitingForLoading,
            WaitingLoadCard,
            Discard,
        }
        
        private StateGame state;
        protected override void Awake()
        {
            base.Awake();
            state = StateGame.WaitingForLoading;
            ServerManager.Instance.SetPlayerOrder();
            ServerManager.Instance.SendData(IdData.PlayerLoadedGame,RpcTarget.MasterClient);
        }
        
        public void OnFinishLoadGame(int playerAmount,StateGame stateGame)
        {
            playerFinished++;
            if (playerFinished == playerAmount)
            {
                playerFinished = 0;
                if (stateGame == StateGame.WaitingForLoading)
                {
                    state = StateGame.WaitingLoadCard;
                    Desk.Instance.SetCardsForAllClient();
                }

                if (stateGame == StateGame.WaitingLoadCard)
                {
                    state = StateGame.Discard;
                    StartCoroutine(ServerManager.Instance.StartDistributeCard());
                }
            }
        }
    }
}
