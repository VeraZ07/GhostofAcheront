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
        public UnityAction<int> OnLeveSizeChanged;

        
        //public static GameManager Instance { get; private set; }

        [Networked] public int GameSeed { get; private set; } = 0;

        [Networked(OnChanged =nameof(OnLevelSizeChanged))] public int LevelSize { get; set; } = 1;

        

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

        public override void Spawned()
        {
            base.Spawned();
            LevelSize = 1;
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
            //FindObjectOfType<MonsterController>()?.SetPlayerEscapedState();
            //PlayerController[] players = FindObjectsOfType<PlayerController>();
            //foreach(PlayerController player in players)
            //{
            //    player.SetEscapedState();
            //}
           
            // Just wait a little more on the server to give every client the time to quit
            StartCoroutine(LoadMenuDelayed(Runner.IsServer ? 5f : 4f));
        }

        public void YouLose()
        {
            OnGameLose?.Invoke();
            StartCoroutine(LoadMenuDelayed(5f));
        }

  
        public void PlayerDead(PlayerController player)
        {
            if (SessionManager.Instance.Runner.IsSinglePlayer)
            {
                YouLose();
            }
            else
            {
                List<PlayerController> all = new List<PlayerController>(FindObjectsOfType<PlayerController>());
                bool allDead = true;
                foreach(PlayerController p in all)
                {
                    if(p.State == (int)PlayerState.Alive)
                    {
                        allDead = false;
                        break;
                    }
                }
                if (allDead)
                {
                    foreach(PlayerController p in all)
                    {
                        p.SetSacrificedState();
                    }
                }
            }
        }

    
        public static void OnLevelSizeChanged(Changed<GameManager> changed)
        {
            changed.Behaviour.OnLeveSizeChanged?.Invoke(changed.Behaviour.LevelSize);
        }
    }

}
