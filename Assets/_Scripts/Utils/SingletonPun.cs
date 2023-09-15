using Photon.Pun;
using UnityEngine;

namespace _Scripts.Utils
{
    public class SingletonPun<T> : MonoBehaviourPunCallbacks where T : class
    {
        private static T instance;

        public static T Instance => instance;
        
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T; 
            }
        }
    }
}

