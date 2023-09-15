using System;
using _Scripts.Cards;
using UnityEngine;
namespace _Scripts.ScriptableObject
{
    [Serializable]
    public class CardSo : UnityEngine.ScriptableObject
    {
        public GameObject cardPrefab;
        public ColorsCard color;
        public ValuesCard value;
    }
}
