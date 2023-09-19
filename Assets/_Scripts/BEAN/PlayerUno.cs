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

        public int CardsAmount => cards.Count;


    }
}
