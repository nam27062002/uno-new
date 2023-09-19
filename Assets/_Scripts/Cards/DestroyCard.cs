using System;
using UnityEngine;

namespace _Scripts.Cards
{
    public class DestroyCard : MonoBehaviour
    {
        public void OnDestroy()
        {
            Destroy(gameObject);
        }
    }
}
