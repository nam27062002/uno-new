using System;
using _Scripts.Utils;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.LoadScene
{
    public enum SceneName
    {
        Init,
        Game,
        Loading,
        Room,
        Lobby,
    }
    public class LoaderManager : Singleton<LoaderManager>
    {

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Load(SceneName sceneName,bool isUsingPhoton = false)
        {
            if (isUsingPhoton)
            {
                PhotonNetwork.LoadLevel(sceneName.ToString());
            }
            else
            {
                SceneManager.LoadScene(sceneName.ToString());
            }
        }
    }
}