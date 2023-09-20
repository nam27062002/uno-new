using System.Collections.Generic;
using _Scripts.Cards;
using DG.Tweening;
using UnityEngine;

namespace _Scripts.BEAN
{
    public class PlayerUno
    {
        private readonly List<Card> cards;
        private readonly List<GameObject> cardsObject;
        public PlayerUno()
        {
            cards = new List<Card>();
            cardsObject = new List<GameObject>();
        }

        public GameObject GetCardObjectByIndex(int index)
        {
            return cardsObject[index];
        }
        public static float dentaY = 20f;
        private float dentaTime = 0.1f;
        public List<Card> Cards => cards;
        public List<GameObject> CardsObject => cardsObject;
        public void AddCard(Card card,GameObject cardObject)
        {
            cards.Add(card);
            cardsObject.Add(cardObject);
        }

        public void RemoveCard(Card card,GameObject cardObject)
        {
            cards.Remove(card);
            cardsObject.Remove(cardObject);
        }

        private List<GameObject> ListObjectCardValid(Card card)
        {
            
            List<GameObject> gameObjects = new List<GameObject>();
            int count = 0;
            foreach (Card c in cards)
            {
                if (card == null || c.Color == card.Color || c.Value == card.Value || c.Value == ValuesCard.COL ||
                    c.Value == ValuesCard.PL4)
                {
                    GameObject cardObject = cardsObject[count];
                    CardSingleManager cardSingleManager = cardObject.AddComponent<CardSingleManager>();
                    cardObject.AddComponent<BoxCollider>();
                    cardSingleManager.Card = c;
                    cardSingleManager.CardObj = cardObject;
                    cardSingleManager.Index = count;
                    gameObjects.Add(cardsObject[count]);
                }

                count++;
            }
            return gameObjects; 
        }
        public int CardsAmount => cards.Count;
        
        public void SetCardCanPlay(Card card)
        {
            List<GameObject> cardObjectCanPlay = ListObjectCardValid(card);
            foreach (GameObject obj in cardObjectCanPlay)
            {
                Vector3 target = obj.transform.position;
                target.y += dentaY;
                obj.transform.DOLocalMoveY(dentaY,dentaTime);
            }
        }
        
    }
}
