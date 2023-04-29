using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GOA
{
    
    public class GameManager : NetworkBehaviour
    {
        public UnityAction OnGameWin;
        public UnityAction OnGameLose;

        
        //public static GameManager Instance { get; private set; }

        [Networked] public int GameSeed { get; private set; } = 0;

        private void Awake()
        {
            //if (!Instance)
            //{
            //    Instance = this;
            //    DontDestroyOnLoad(gameObject);
            //}
            //else
            //{
                
            //    Destroy(gameObject);
                
            //}

            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        IEnumerator LoadMenuDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Get the player camera in order to destroy it on session quit
            Camera.main.transform.parent = null;

            SessionManager.Instance.QuitSession();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
        }



        public void CreateNewSeed()
        {
            GameSeed = (int)System.DateTime.UtcNow.Ticks;
        }

        public void YouWin()
        {
            OnGameWin?.Invoke();
            FindObjectOfType<MonsterController>().SetPlayerEscapedState();
            PlayerController[] players = FindObjectsOfType<PlayerController>();
            foreach(PlayerController player in players)
            {
                player.Escape();
            }
            StartCoroutine(LoadMenuDelayed(5f));
        }

        public void YouLose()
        {
            OnGameLose?.Invoke();
            StartCoroutine(LoadMenuDelayed(5f));
        }

        public void PlayerExit(PlayerController player)
        {
            if (SessionManager.Instance.Runner.IsSinglePlayer)
                YouWin();
        }

        public void PlayerDead(PlayerController player)
        {
            if (SessionManager.Instance.Runner.IsSinglePlayer)
                YouLose();
        }
    }

}
