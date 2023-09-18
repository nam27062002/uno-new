using System;
using System.Collections.Generic;
using _Scripts.Cards;
using _Scripts.ScriptableObject;
using _Scripts.UI;
using _Scripts.Utils;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;
namespace _Scripts.Server.Gameplay
{
    public class Desk : Singleton<Desk>
    {
        [SerializeField] private CardListSo cardListSo;
        private readonly int normalCardAmount = 2;
        private readonly int specialCardAmount = 1 ;
        private List<Card> cards;
        private Dictionary<string, CardSo> cardListSoDictionary;
        public GameObject a;
        protected override void Awake()
        {
            base.Awake();
            cards = new List<Card>();
            
        }
        
        private void Start()
        {
            if (!ServerManager.Instance.IsMasterClient)
            {
                cardListSoDictionary = new Dictionary<string, CardSo>();
                SetCardListSoDictionary();
            }
            else
            {
                Initialize();
                ShuffleCard();
                ShowAllCard();
                CreateCard();
            }
            
        }
        
        private void Initialize()
        {   
            foreach (CardSo cardSo in cardListSo.cardSos)
            {
                Dictionary<ValuesCard, int> quanlityCard = ConfigQuanlityCard();
                for (int i = 0; i < quanlityCard[cardSo.value]; i++)
                {
                    if (cardSo.color != ColorsCard.BLACK)
                    {
                        if (cardSo.value == ValuesCard.PL4 || cardSo.value == ValuesCard.COL)
                        {
                            continue;
                        }
                    }
                    
                    cards.Add(new Card(cardSo));
                }
            }
        }
        private Dictionary<ValuesCard, int> ConfigQuanlityCard()
        {
            Dictionary<ValuesCard, int> quanlityCard = new Dictionary<ValuesCard, int>();
            foreach (ValuesCard card in Enum.GetValues(typeof(ValuesCard)))
            {
                if (card == ValuesCard.PL4 || card == ValuesCard.COL)
                {
                    quanlityCard[card] = specialCardAmount * (Enum.GetValues(typeof(ColorsCard)).Length - 1);
                }
                else
                {
                    quanlityCard[card] = normalCardAmount;
                }
            }

            return quanlityCard;
        }
        private void ShuffleCard()
        {
            int cardCount = cards.Count;
            for (int i = 0; i < cardCount - 1; i++)
            {
                int randomIndex = Random.Range(i, cardCount);
                (cards[i], cards[randomIndex]) = (cards[randomIndex], cards[i]);
            }
        }

        private void ShowAllCard()
        {
            foreach (Card card in cards)
            {
                Debug.Log(card.ToString());
            }
        }

        private void ShowAllCardDictionary()
        {
            foreach (string key in cardListSoDictionary.Keys)
            {
                Debug.Log($"Key: {key} Value: {cardListSoDictionary[key]}");
            }
        }

        public void SetCardsForAllClient()
        {
            foreach (Card card in cards)
            {
                ServerManager.Instance.SendData(IdData.SetCardsForAllClient,RpcTarget.All,card.Color.ToString(),card.Value.ToString());
            }
        }

        private string CreateKeyCardListSoDict(string color, string value)
        {
            return color + "-" + value;
        }
        private void SetCardListSoDictionary()
        {
            foreach (CardSo cardSo in cardListSo.cardSos)
            {
                cardListSoDictionary[CreateKeyCardListSoDict(cardSo.color.ToString(),cardSo.value.ToString())] = cardSo;
            }
        }

        private CardSo GetCardSoByColorValue(string color, string value)
        {
            return cardListSoDictionary[CreateKeyCardListSoDict(color,value)];
        }
        public void SetCards(string color,string value)
        {
            cards.Add(new Card(GetCardSoByColorValue(color,value)));
            if (cards.Count == 104) CreateCard();
        }
        
        public void CreateCard()
        {
            StartCoroutine(CreateCards.Instance.SpwanCardHandle(cards));
            
        }
    }
}
