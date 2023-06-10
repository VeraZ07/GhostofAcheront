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
            StartCoroutine(LoadMenuDelayed(Runner.IsServer ? 5f : 4f));
        }

        public void YouLose()
        {
            OnGameLose?.Invoke();
            StartCoroutine(LoadMenuDelayed(5f));
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
                CheckForAliveAndDead();
                
            }
            


        }
  
        public void PlayerDied(PlayerController player)
        {
            if (SessionManager.Instance.Runner.IsSinglePlayer)
            {
                YouLose();
            }
            else
            {
                // We first check if all the players are dead
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
                
                if (allDead) // No one is alive
                {
                    foreach(PlayerController p in all)
                    {
                        if (p.HasInputAuthority)
                            StartCoroutine(SetSacrificedStateDelayed(p, 1.5f));
                        else
                            p.SetSacrificedState();
                    }
                }
                else // Someone is still alive, check where they are: are they in the escape trigger?
                {
                    CheckForAliveAndDead();
                }

            }
        }

        public void CheckForAliveAndDead()
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
