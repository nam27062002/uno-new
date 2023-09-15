using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class UiPlayerSingleManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nicknameText;
        [SerializeField] private Color myNicknameColor;
        [SerializeField] private Color defaultNicknameColor;
        public void SetNickName(string nicknameText, bool isMyNickname)
        {
            this.nicknameText.text = nicknameText;
            this.nicknameText.color = isMyNickname ? myNicknameColor : defaultNicknameColor;
        }
    }
}
