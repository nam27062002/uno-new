using System;
using Photon.Realtime;

namespace _Scripts.BEAN
{
    [Serializable]
    public class MessageData
    {
        private Player player;
        private string message;
        private DateTime timestamp;
        public MessageData(Player player, string message, DateTime timestamp)
        {
            this.player = player;
            this.message = message;
            this.timestamp = timestamp;
        }
    
        public Player Player { get { return player; } }
        public string Message { get { return message; } }
        public DateTime Timestamp { get { return timestamp; } }

    }
}
