using System.Collections.Generic;
using _Scripts.Cards;
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

        public List<GameObject> ListObjectCardValid(Card card)
        {
            
            List<GameObject> gameObjects = new List<GameObject>();
            int count = 0;
            foreach (Card c in cards)
            {
                if (c.Color == card.Color || c.Value == card.Value || c.Value == ValuesCard.COL ||
                    c.Value == ValuesCard.PL4 || card == null)
                {
                    gameObjects.Add(cardsObject[count]);
                }

                count++;
            }
            return gameObjects; 
        }
        public int CardsAmount => cards.Count;
        

    }
}
