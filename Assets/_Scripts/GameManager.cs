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
            WaitingDistributeCard,
        }
        
        protected override void Awake()
        {
            base.Awake();
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
                    Desk.Instance.SetCardsForAllClient();
                }

                if (stateGame == StateGame.WaitingLoadCard)
                {
                    ServerManager.Instance.StartDistributeCard();
                }

                if (stateGame == StateGame.WaitingDistributeCard)
                {
                    ServerManager.Instance.SetPlayerCanPlay();
                }
            }
        }
    }
}
