using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class UIMessageSingleManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI messageText;
        public void SetMessage(string nickname, string message,bool isMyMessage)
        {
            string messageColor = isMyMessage ? "green" : "#17CEFF";
            messageText.text = $"<color={messageColor}>{nickname}</color>: {message}";
        }
    }
}
