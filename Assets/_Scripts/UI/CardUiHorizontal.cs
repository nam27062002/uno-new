using System;
using System.Collections.Generic;
using _Scripts.BEAN;
using _Scripts.Cards;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.UI
{
    public class CardUiHorizontal : Singleton<CardUiHorizontal>
    {
        private PlayerUno player;
        public PlayerUno Player => player;
        [SerializeField] private float maxWidth = 1000f;
        protected override void Awake()
        {
            base.Awake();
            player = new PlayerUno();
        }

        public void AddCard(Card card,GameObject cardObject)
        {
            player.AddCard(card);
            cardObject.transform.SetParent(transform);
            // Instantiate(card.Prefab,card.Prefab.transform.position,card.Prefab.transform.rotation,transform);
            // float marginHozirontal = (maxWidth - CreateCards.Instance.WitdhLocal * player.CardsAmount) / 2;
            // if (marginHozirontal >= 0f)
            // {
            //    
            // }
        }
        // private void SpwanCardHandle()
        // {
        //     
        //     float dentaDepth = 1f;
        //     for (int i = 0; i < cards.Count; i++)
        //     {
        //         GameObject cardObject = Instantiate(cards[i],transform);
        //         if (marginHozirontal >= 0f)
        //         {
        //             cardObject.transform.localPosition = new Vector3(-maxWidth/2 + marginHozirontal + widthLocal * i + widthLocal/2,0f,0f);
        //         }
        //         else
        //         {
        //             cardObject.transform.localPosition = new Vector3(-maxWidth/2 + (widthLocal + marginHozirontal * 2/cards.Count)*i+ widthLocal/2,0f,0f);
        //         }
        //         cardObject.transform.localRotation = Quaternion.Euler(0f,0f,180f);
        //         cardObject.transform.localScale = new Vector3(widthCardUi, heightCardUi, depthCardUi + dentaDepth * i);
        //     }
        // }
    }
}
