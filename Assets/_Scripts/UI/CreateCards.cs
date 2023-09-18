using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Cards;
using _Scripts.Server.Gameplay;
using _Scripts.Utils;
using Photon.Pun;
using UnityEngine;

namespace _Scripts.UI
{
    public class CreateCards : Singleton<CreateCards>
    {
        private float depthCardUi = 2f;
        [SerializeField] private float heightLocal = 300f;
        private const float RatioSizeCard = 2 / 3f;
        private const float RatioSizeUiPrefab = 14000f / 300f;
        private float heightCardUi;
        private float widthCardUi;
        public float HeightLocal => heightLocal;
        public float WitdhLocal => WitdhLocal;
        private Dictionary<Card, GameObject> gameObjectCard;
        private List<Card> cards;
        protected override void Awake()
        {
            base.Awake();
            heightCardUi = heightLocal * RatioSizeUiPrefab;
            widthCardUi = heightCardUi * RatioSizeCard;
            gameObjectCard = new Dictionary<Card, GameObject>();
            cards = new List<Card>();
        }
        public IEnumerator SpwanCardHandle(List<Card> cardsList)
        {
            for (int i = 0; i < cardsList.Count; i++)
            {
                cards.Add(cardsList[i]);
                GameObject cardObject = Instantiate(cardsList[i].Prefab,transform);
                cardObject.transform.localPosition = new Vector3(0.2f*i,-0.3f*i,-0.5f*i);
                cardObject.transform.localRotation = Quaternion.Euler(0f,180f,180f);
                cardObject.transform.localScale = new Vector3(widthCardUi, heightCardUi,depthCardUi);
                gameObjectCard[cardsList[i]] = cardObject;
                yield return new WaitForSeconds(0.01f);
            }
            ServerManager.Instance.SendData(IdData.CreateCardSuccessfully,RpcTarget.MasterClient);
        }

        private void DisCard()
        {
            Card card = cards[cards.Count - 1];
            CardUiHorizontal.Instance.AddCard(card,gameObjectCard[card]);
            cards.Remove(card);
            gameObjectCard.Remove(card);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                DisCard();
            }
        }
    }
    
}
