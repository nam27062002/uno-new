using System;
using _Scripts.BEAN;
using _Scripts.Server.Gameplay;
using _Scripts.UI;
using DG.Tweening;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

namespace _Scripts.Cards
{
    public class CardSingleManager : MonoBehaviour
    {
        private Card card;
        private GameObject cardObj;
        private static bool IS_CLICKED = false;
        private static CardSingleManager SINGLE_MANAGER;
        private float dentaYHover = PlayerUno.dentaY + 15;
        private float dentaYClick = PlayerUno.dentaY + 30;
        private int index;
        public Card Card
        {
            set
            {
                card = value;
            }
        }
        public GameObject CardObj
        {
            set
            {
                cardObj = value;
            }
        }

        public int Index
        {
            set
            {
                index = value;
            }
        }
        private void OnMouseEnter()
        {
            if(!IS_CLICKED) cardObj.transform.DOLocalMoveY(dentaYHover, 0.1f);
        }

        private void OnMouseExit()
        {
            if (!IS_CLICKED) cardObj.transform.DOLocalMoveY(PlayerUno.dentaY, 0.1f);
        }

        private void OnMouseDown()
        {
            if (!IS_CLICKED)
            {
                IS_CLICKED = true;
                SINGLE_MANAGER = this;
                cardObj.transform.DOLocalMoveY(dentaYClick, 0.1f);
            }
            else
            {
                if (SINGLE_MANAGER == this)
                {
                    ServerManager.Instance.SendData(IdData.OnCardSelected,RpcTarget.All,index,PhotonNetwork.NickName);
                }
                else
                {
                    cardObj.transform.DOLocalMoveY(dentaYClick, 0.1f);
                    SINGLE_MANAGER.cardObj.transform.DOLocalMoveY(PlayerUno.dentaY, 0.1f);
                    SINGLE_MANAGER = this;
                }
            }
        }
    }
}
