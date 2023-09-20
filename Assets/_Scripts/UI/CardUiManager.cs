using System.Collections;
using System.Collections.Generic;
using _Scripts.BEAN;
using _Scripts.Cards;
using _Scripts.Server.Gameplay;
using _Scripts.Utils;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;

namespace _Scripts.UI
{
    public class CardUiManager : Singleton<CardUiManager>
    {

        [SerializeField] private float heightLocal = 200f;
        [SerializeField] private float maxWidthHorizontal = 1000f;
        [SerializeField] private float maxWidthVertical = 600f;
        [SerializeField] private List<GameObject> playerUi;
        [SerializeField] private GameObject spawnCardObject;
        [SerializeField] private float timeDiscard = 0.2f;
        [SerializeField] private GameObject tableObject;
        public float TimeDiscard => timeDiscard;
        private float widthLocal;
        private float heightCardUi;
        private float widthCardUi;
        private const float RatioSizeCard = 2 / 3f;
        private const float RatioSizeUiPrefab = 14000f / 300f;
        private Vector3 cardTargetPosition = Vector3.zero;
        private int index = 0;
        private List<PlayerUno> players;
        private List<GameObject> gameObjectCard;
        private List<Card> cards;
        private int countDiscard = 0;
        protected override void Awake()
        {
            base.Awake();
            heightCardUi = heightLocal * RatioSizeUiPrefab;
            widthLocal = heightLocal * RatioSizeCard;
            widthCardUi = heightCardUi * RatioSizeCard;
            players = new List<PlayerUno>();
            gameObjectCard = new List<GameObject>();
            cards = new List<Card>();
            for (int i = 0; i < playerUi.Count; i++)
            {
                players.Add(new PlayerUno());
            }
        }
        public IEnumerator SpwanCardHandle(List<Card> cardsList)
        {
            for (int i = 0; i < cardsList.Count; i++)
            {
                cards.Add(cardsList[i]);
                GameObject cardObject = Instantiate(cardsList[i].Prefab,spawnCardObject.transform);
                cardObject.name = SetNameObject(cardsList[i]);
                cardObject.transform.localPosition = new Vector3(0.2f*i,-0.3f*i,-0.5f*i);
                cardObject.transform.localRotation = Quaternion.Euler(0f,180f,180f);
                cardObject.transform.localScale = new Vector3(widthCardUi, heightCardUi,2f);
                gameObjectCard.Add(cardObject);
                yield return new WaitForSeconds(0.01f);
            }
            ServerManager.Instance.SendData(IdData.CreateCardSuccessfully,RpcTarget.MasterClient);
        }
        
        public void AddCard(Card card, GameObject cardObject)
        { 
            players[index].AddCard(card, cardObject);
            cardObject.transform.SetParent(playerUi[index].transform);
            var sequence = cardObject.transform.DOLocalMove(GetVectorCardTarget(players[index].CardsAmount,true), timeDiscard);
            if (index == 0)
            {
                cardObject.transform.DOLocalRotate(new Vector3(0f,0f,-180f),timeDiscard);
            }
            else if (index == 1 || index == 3)
            {
                cardObject.transform.DOLocalRotate(new Vector3(0f,-180f,-0f),timeDiscard);
            }
            else if (index == 2)
            {
                cardObject.transform.DOLocalRotate(new Vector3(0f,180f,0f),timeDiscard);
            }
            StartCoroutine(WaitForMoveComplete(sequence));
        }

        private Vector3 GetVectorCardTarget(float index,bool isDiscard = false)
        {
            float maxWidth = GetMaxWidth();
            float marginHorizontal = (maxWidth -  widthLocal * players[this.index].CardsAmount) / 2;
            if (marginHorizontal >= 0)
            {
                if (isDiscard) index--;
                cardTargetPosition.x = -maxWidth / 2 + marginHorizontal + widthLocal * index +
                                      widthLocal / 2;
            }
            else
            {
                if (isDiscard) index = index - 0.5f;
                cardTargetPosition.x = -maxWidth / 2 + (widthLocal + marginHorizontal * 2 / players[this.index].CardsAmount) * index +
                                       widthLocal / 2;
            }

            cardTargetPosition.y = 0f;
            cardTargetPosition.z = 30f;

            return cardTargetPosition;
        }
        private IEnumerator WaitForMoveComplete(Tweener tweener)
        {
            yield return tweener.WaitForCompletion();
            SpwanCardHandle();
        }
        private void SpwanCardHandle()
        {
            for (int i = 0; i < players[index].CardsAmount; i++)
            {
                GameObject cardObject = players[index].CardsObject[i];
                cardObject.transform.localPosition = GetVectorCardTarget(i);
                Vector3 scale = cardObject.transform.localScale;
                scale.z = 10f + i; 
                cardObject.transform.localScale = scale;
            }
        }
        
        private string SetNameObject(Card card)
        {
            return card.Color + "-" + card.Value;
        }
        
        public void DisCard(string nickname)
        {
            index = ServerManager.Instance.PlayerOrder[nickname];
            if (cards.Count > 0)
            {
                Card card = cards[cards.Count - 1];
                GameObject cardObject = gameObjectCard[gameObjectCard.Count - 1];
                
                AddCard(card,cardObject);
                cards.Remove(card);
                gameObjectCard.Remove(cardObject);
            }
            else
            {
                Debug.LogError("Het bai rui");
            }
        }

        private float GetMaxWidth()
        {
            if (index == 0 || index == 2) return maxWidthHorizontal;
            return maxWidthVertical;
        }

        public void SetCardCanPlay(Card card)
        {
            players[0].SetCardCanPlay(card);
        }

        public void SetChildTableObject(int index,string nickname)
        {
            float dentaTime = 0.5f;
            float dentaDepth = 0.1f;
            
            int localIndex = ServerManager.Instance.PlayerOrder[nickname];
            GameObject cardObject = players[localIndex].GetCardObjectByIndex(index);

            cardObject.transform.SetParent(tableObject.transform);
            cardObject.transform.DOLocalMove(new Vector3(0,0,-dentaDepth*countDiscard),dentaTime);
            cardObject.transform.DOLocalRotate(new Vector3(0,0,Random.Range(-90,90)), dentaTime);
            countDiscard++;
        }
    }
}

