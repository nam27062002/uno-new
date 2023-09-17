using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UiPlayerSingleManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private Color myNicknameColor;
        [SerializeField] private Color defaultNicknameColor;
        [SerializeField] private Outline avatarOutline;
        [SerializeField] private Outline nicknameOutline;
        [SerializeField] private GameObject readyPrefab;
        
        private bool isReady = false;
        public bool IsReady => isReady;
        private void Awake()
        {
            readyPrefab.SetActive(false);
        }

        public void SetNickName(string nickname, bool isMyNickname)
        {
            nicknameText.text = nickname;
            Color targetColor = isMyNickname ? myNicknameColor : defaultNicknameColor;
            avatarOutline.effectColor = targetColor;
            nicknameOutline.effectColor = targetColor;
        }

        public bool IsNickname(string nickname) => nicknameText.text == nickname;
        
        public void SetActiveReadyPrefab()
        {
            isReady = !isReady;
            readyPrefab.SetActive(isReady);
        }
        
        public void SetActiveReadyPrefab(bool isActive)
        {
            isReady = isActive;
            readyPrefab.SetActive(isReady);
        }
    }
}
