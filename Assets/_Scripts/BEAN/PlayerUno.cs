using System.Collections.Generic;
using _Scripts.Cards;

namespace _Scripts.BEAN
{
    public class PlayerUno
    {
        private List<Card> cards;

        public PlayerUno()
        {
            cards = new List<Card>();
        }

        public List<Card> Cards => cards;
        public void AddCard(Card card)
        {
            cards.Add(card);
        }

        public void RemoveCard(Card card)
        {
            cards.Remove(card);
        }

        public int CardsAmount => cards.Count;


    }
}
