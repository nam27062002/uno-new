using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.UI
{
    public class NewBehaviourScript : MonoBehaviour
    {
        [SerializeField] private List<GameObject> cards;

        
        [SerializeField] private float depthCardUi = 2f;
        [SerializeField] private float maxWidth = 1000f;
        [SerializeField] private float heightLocal = 300f;
        
        
        private const float RatioSizeCard = 2 / 3f;
        private const float RatioSizeUiPrefab = 14000f / 300f;
        private float heightCardUi;
        private float widthCardUi;
        private float widthLocal;
        private void Awake()
        {
            heightCardUi = heightLocal * RatioSizeUiPrefab;
            widthCardUi = heightCardUi * RatioSizeCard;
            widthLocal = heightLocal * RatioSizeCard;
        }

        private void Start()
        {
            SpwanCardHandle();
        }

        private void SpwanCardHandle()
        {
            float marginHozirontal = (maxWidth - widthLocal * cards.Count) / 2;
            float dentaDepth = 1f;
            for (int i = 0; i < cards.Count; i++)
            {
                GameObject cardObject = Instantiate(cards[i],transform);
                if (marginHozirontal >= 0f)
                {
                    cardObject.transform.localPosition = new Vector3(-maxWidth/2 + marginHozirontal + widthLocal * i + widthLocal/2,0f,0f);
                }
                else
                {
                    cardObject.transform.localPosition = new Vector3(-maxWidth/2 + (widthLocal + marginHozirontal * 2/cards.Count)*i+ widthLocal/2,0f,0f);
                }
                cardObject.transform.localRotation = Quaternion.Euler(0f,0f,180f);
                cardObject.transform.localScale = new Vector3(widthCardUi, heightCardUi, depthCardUi + dentaDepth * i);
            }
        }
    }
}
