using _Scripts.ScriptableObject;
using UnityEngine;

namespace _Scripts.Cards
{
    public class Card
    {
        private readonly ColorsCard color;
        private readonly ValuesCard value;
        private readonly GameObject prefab;

        public Card(CardSo cardSo)
        {
            color = cardSo.color;
            value = cardSo.value;
            prefab = cardSo.cardPrefab;
        }
    
        public ColorsCard Color => color;
        public ValuesCard Value => value;
        public GameObject Prefab => prefab;
        public override string ToString()
        {
            return $"Color: {color.ToString()} Value: {value.ToString()}";
        }
    }
}   

