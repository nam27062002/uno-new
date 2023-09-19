using _Scripts.Server.Gameplay;
using _Scripts.Utils;
using Photon.Pun;
using UnityEngine;

namespace _Scripts
{
    public class GameManager : Singleton<GameManager>
    {
        private int playerLoadedGame = 0;
        private int playerCreatedCard = 0;
        private enum State
        {
            WaitingForLoading,
            WaitingLoadCard,
            Play,
        }
        
        private State state;
        protected override void Awake()
        {
            base.Awake();
            state = State.WaitingForLoading;
            ServerManager.Instance.SendData(IdData.PlayerLoadedGame,RpcTarget.MasterClient);
        }
        
        public void IncreasePlayerLoadedGame(int playerAmount)
        {
            playerLoadedGame++;
            if (playerLoadedGame == playerAmount)
            {
                state = State.WaitingLoadCard;
                Desk.Instance.SetCardsForAllClient();
            }
        }
        
        public void IncreasePlayerCreateCard(int playerAmount)
        {
            playerCreatedCard++;
            if (playerCreatedCard == playerAmount)
            {
                state = State.WaitingLoadCard;
            }
        }

        
    }
}
