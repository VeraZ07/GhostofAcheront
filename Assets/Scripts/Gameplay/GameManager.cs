using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        [Networked(OnChanged =nameof(OnLevelSizeChanged))] public int LevelSize { get; set; } = 0;

        

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
            SessionManager.OnShutdownCallback += (reason) =>
            {
                if (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(0) != UnityEngine.SceneManagement.SceneManager.GetActiveScene())
                {
                    Debug.Log("GameManager - Shutdown reason:" + reason);
                    if(reason != ShutdownReason.Ok)
                    {
                        Task.Delay(5000).Wait();

                    }
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                }
                    
            };
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        IEnumerator QuitGameDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Get the player camera in order to destroy it on session quit
            //Camera.main.transform.parent = null;

            SessionManager.Instance.QuitSession();
            //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        
        IEnumerator SetSacrificedStateDelayed(PlayerController player, float delay)
        {
            yield return new WaitForSeconds(delay);
            player.SetSacrificedState();
        }

        IEnumerator SetEscapedStateDelayed(PlayerController player, float delay)
        {
            yield return new WaitForSeconds(delay);
            player.SetEscapedState();
        }

        IEnumerator CheckForDeadAndAlive()
        {
            yield return new WaitForSeconds(1f);

            Debug.Log("GameManager - PlayerDead() - multiplayer");
            // We first check if all the players are dead
            List<PlayerController> all = new List<PlayerController>(FindObjectsOfType<PlayerController>());
            Debug.Log("GameManager - PlayerDead() - number of players:" + all.Count);
            bool allDead = true;
            foreach (PlayerController p in all)
            {
                if (p.State == (int)PlayerState.Alive)
                {
                    allDead = false;
                    break;
                }
            }

            Debug.Log("GameManager - PlayerDead() - allDead:" + allDead);

            if (allDead) // No one is alive
            {
                foreach (PlayerController p in all)
                {
                    Debug.Log("GameManager - PlayerDead() - player:" + p.name);
                    if (p.HasInputAuthority)
                        StartCoroutine(SetSacrificedStateDelayed(p, 1.5f));
                    else
                        p.SetSacrificedState();
                }
            }
            else // Someone is still alive, check where they are: are they in the escape trigger?
            {
                CheckForEscaped();
            }
        }

        public override void Spawned()
        {
            base.Spawned();
            LevelSize = 0;
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
         
           
            // Just wait a little more on the server to give every client the time to quit
            StartCoroutine(QuitGameDelayed((Runner.IsServer || Runner.IsSharedModeMasterClient) ? 5f : 4f));
        }

        public void YouLose()
        {
            OnGameLose?.Invoke();
            StartCoroutine(QuitGameDelayed(5f));
        }

        public void PlayerEnteredTheEscapeTrigger(PlayerController player)
        {
            if(SessionManager.Instance.Runner.IsSinglePlayer)
            {
                FindObjectOfType<MonsterController>()?.SetPlayerEscapedState();
                player.SetEscapedState();
            }
            else
            {
                CheckForEscaped();
                
            }
            


        }
  
        public void PlayerHasDead(PlayerController player)
        {
            Debug.Log("GameManager - PlayerDead()");

            if (SessionManager.Instance.Runner.IsSinglePlayer)
            {
#if UNITY_EDITOR
                return;
#endif 
                YouLose();

            }
            else
            {
                StartCoroutine(CheckForDeadAndAlive());

            }
        }

       

        public void CheckForEscaped()
        {
            Debug.Log("DeadOrAlive:1");

            // Check if there is at least a player alive outside this trigger
            PlayerController outside = new List<PlayerController>(FindObjectsOfType<PlayerController>()).Find(p => !EscapeTrigger.Instance.IsPlayerInside(p) && p.State == (int)PlayerState.Alive);
            if (outside) // Yes there is, do nothing
                return;

            Debug.Log("DeadOrAlive:2");

            // There is no one still alive outside the trigger ( maybe they are all dead ). All the players inside the trigger
            // are finally free to escape.
            // Report the monster
            FindObjectOfType<MonsterController>()?.SetPlayerEscapedState();
            // Set players free
            List<PlayerController> aliveAll = new List<PlayerController>(FindObjectsOfType<PlayerController>()).FindAll(p => p.State == (int)PlayerState.Alive);
            foreach (PlayerController p in aliveAll)
            {
                if (p.HasInputAuthority)
                    StartCoroutine(SetEscapedStateDelayed(p, 1.5f));
                else
                    p.SetEscapedState();
            }

            Debug.Log("DeadOrAlive:3");
            // Sacrifice all the players already in the dead state: dying players will be sacrificed when moving to the dead state too. 
            List<PlayerController> deadAll = new List<PlayerController>(FindObjectsOfType<PlayerController>()).FindAll(p => p.State == (int)PlayerState.Dead);
            Debug.Log("DeadAll.Count:" + deadAll.Count);
            foreach (PlayerController p in deadAll)
            {
                if (p.HasInputAuthority)
                    StartCoroutine(SetSacrificedStateDelayed(p, 1.5f));
                else
                    p.SetSacrificedState();
            }

        }


        public static void OnLevelSizeChanged(Changed<GameManager> changed)
        {
            changed.Behaviour.OnLeveSizeChanged?.Invoke(changed.Behaviour.LevelSize);
        }
    }

}
