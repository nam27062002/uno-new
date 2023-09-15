using System;
using System.Collections.Generic;
using _Scripts.Cards;
using _Scripts.ScriptableObject;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;
namespace _Scripts.Server.Gameplay
{
    public class Desk : MonoBehaviour
    {
        [SerializeField] private CardListSo cardListSo;
        private readonly int normalCardAmount = 2;
        private readonly int specialCardAmount = 1;
        private List<Card> cards;
        private void Awake()
        {
            cards = new List<Card>();
        }
        
        private void Start()
        {
            ServerManager.Instance.OnServerSetCard += InstanceOnOnServerSetCard;
            if (!ServerManager.Instance.IsMasterClient) return;
            Initialize();
            ShuffleCard();
            SetCardForAllClient();
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

        private void ClientSignCard(ColorsCard colorsCard,ValuesCard valuesCard)
        {
            Debug.Log($"Color: {colorsCard} Value: {valuesCard}");
        }
        
        private void SetCardForAllClient()
        {
            foreach (Card card in cards)
            {
                ServerManager.Instance.SendData(IdData.SetCardForAllClient, RpcTarget.All,card.Color,card.Value);
            }
        }
        
        private void InstanceOnOnServerSetCard(object sender, ServerManager.OnServerSetCardEventArgs e)
        {
            ClientSignCard(e.ColorsCard,e.ValuesCard);
        }
    }
}
