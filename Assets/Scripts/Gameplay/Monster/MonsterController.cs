using DG.Tweening;
using Fusion;
using GOA.Audio;
using GOA.Interfaces;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static GOA.Level.LevelBuilder;

namespace GOA
{
    public enum MonsterState { Idle, Moving, PlayerSpotted, Hunting, PlayerLost, Killing, PlayerEscaped }

    public class MonsterController : NetworkBehaviour
    {
        #region fields
        [System.Serializable]
        private class AttackerData
        {
            [SerializeField]
            public int attackId;

            [SerializeField]
            public int weight = 1;
        }

        [SerializeField]
        GameObject meshRoot;

        [SerializeField]
        float sightRange = 8f * 1.5f;

        [SerializeField]
        float sightAngle = 60f;

        [SerializeField]
        Transform head;
        
        [SerializeField]
        Animator animator;

        IKiller deathMaker;        
        
        NavMeshAgent agent;

        string paramSpeed = "Speed";
   
        List<PlayerController> players = new List<PlayerController>();
        LevelBuilder builder;

        [Networked] public int State { get; private set; } = -1;

        float idleTimeMin = 6.0f;
        float idleTimeMax = 9.0f;
        float timer = 0;

        [SerializeField]
        PlayerController prey = null;
        
        //bool preyLost = false;
        //float preyLostTime = 0f;
        float extraTimeCurrent = 0;
        bool usingExtraTime = false;
        float extraTime = 1f;
        PlayerController lastPrey;
                
        float huntingTime = .5f;
        System.DateTime lastHuntingDT;
        NavMeshPath huntingPath;
        float attackRange = .8f * 1.1f;
        int monsterNoTrackMax = 4;

        [SerializeField]
        List<AttackerData> attackers;

        float walkSpeed;
        float runSpeed;

        bool patching = false;

        MonsterAudioController audioController;
        #endregion

        #region native

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            deathMaker = GetComponent<IKiller>();
            runSpeed = agent.speed;
            walkSpeed = runSpeed * .5f;
            agent.enabled = false;
        }

        // Start is called before the first frame update
        void Start()
        {
            //Hide();
            NavMesh.pathfindingIterationsPerFrame = 250;
            agent.enabled = true;

            
        }

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.P))
        //        gameObject.SetActive(false);
        //}

        #endregion

        #region fusion overrides

        public override void Spawned()
        {
            base.Spawned();

            
            if (Runner.IsServer || Runner.IsSharedModeMasterClient)
            {
                GetComponent<NavMeshAgent>().enabled = true;
                
                // Get the level builder
                builder = FindObjectOfType<LevelBuilder>();

                SetState((int)MonsterState.Idle);

            }
                
        }

        // Update is called once per frame
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if ((Runner.IsServer || Runner.IsSharedModeMasterClient) && !patching)
            {
                //agent.enabled = false; // TO REMOVE *******************************************

                switch (State)
                {
                    case (int)MonsterState.Idle:
                        LoopIdleState();
                        break;
                    case (int)MonsterState.Moving:
                        LoopMovingState();
                        break;
                    case (int)MonsterState.PlayerSpotted:
                        LoopPlayerSpottedState();
                        break;
                    case (int)MonsterState.Hunting:
                        LoopHuntingState();
                        break;
                    case (int)MonsterState.PlayerLost:
                        LoopPlayerLostState();
                        break;
                    case (int)MonsterState.Killing:
                        LoopKillingState();
                        break;
                    case (int)MonsterState.PlayerEscaped:
                        LoopPlayerEscapedState();
                        break;
                }


                
                if (animator)
                {
                    animator.SetFloat(paramSpeed, agent.velocity.magnitude / runSpeed);
                }


            }
            
        }
        #endregion


        #region state management
        void SetState(int state)
        {
            if (Runner.IsServer || Runner.IsSharedModeMasterClient)
            {
                if (State == state)
                    return;

                State = state;
                switch (State)
                {
                    case (int)MonsterState.Idle:
                        EnterIdleState();
                        break;
                    case (int)MonsterState.Moving:
                        EnterMovingState();
                        break;
                    case (int)MonsterState.PlayerSpotted:
                        EnterPlayerSpottedState();
                        break;
                    case (int)MonsterState.Hunting:
                        EnterHuntingState();
                        break;
                    case (int)MonsterState.PlayerLost:
                        EnterPlayerLostState();
                        break;
                    case (int)MonsterState.Killing:
                        EnterKillingState();
                        break;
                    case (int)MonsterState.PlayerEscaped:
                        EnterPlayerEscapedState();
                        break;
                }
            }
            
            
        }

        IEnumerator PatchAgent()
        {
            patching = true;
            agent.ResetPath();
            agent.enabled = false;
            agent.path = null;
            
            yield return new WaitForSeconds(0.5f);
            agent.enabled = true;
            patching = false;
        }

        void EnterIdleState()
        {
            //StartCoroutine(PatchAgent());
            agent.speed = walkSpeed;
            try
            {
                agent.ResetPath();
            }
            catch (System.Exception) 
            {
                //StartCoroutine(PatchAgent());
            }
            //agent.enabled = false;
            timer = Random.Range(idleTimeMin, idleTimeMax);

        }

        void EnterMovingState()
        {
            //agent.enabled = true;
            agent.speed = walkSpeed;

            Debug.Log("MONSTER - agent.hasPath:" + agent.hasPath);
            Debug.Log("MONSTER - agent.pathPending:" + agent.pathPending);

            if (!agent.hasPath && !agent.pathPending)
                agent.SetDestination(GetDestination());
        }

        void EnterHuntingState()
        {
            agent.speed = runSpeed;
        }

        void EnterPlayerSpottedState()
        {
            usingExtraTime = false;
            SetState((int)MonsterState.Hunting);
        }

        void EnterKillingState()
        {
            // Get an attacker data from the list
            AttackerData data = attackers[Random.Range(0, attackers.Count)];
            // Get the corresponding attacker component
            IKiller attacker = GetComponent<IKiller>();
            // Call the attacker
            attacker.Kill(prey, data.attackId);
            prey = null;
            lastPrey = null;
        }

        void EnterPlayerEscapedState()
        {
            agent.speed = 0;
            agent.isStopped = true;
        }

        void EnterPlayerLostState()
        {
            huntingPath = null;
            usingExtraTime = false;
            SetState((int)MonsterState.Moving);
        }

        void LoopKillingState()
        {
           
        }

        void LoopPlayerEscapedState()
        {

        }

        void LoopIdleState()
        {
            timer -= Time.fixedDeltaTime;
            
            if(timer < 0) // Prepare for state changing
            {
                SetState((int)MonsterState.Moving);
            }
            else
            {
                // You may do something while in idle
                if (CheckForPlayer())
                    SetState((int)MonsterState.PlayerSpotted);
            }
        }

        void LoopMovingState()
        {
            if (CheckForPlayer())
                SetState((int)MonsterState.PlayerSpotted);
            else if ((!agent.hasPath && !agent.pathPending) || Vector3.Distance(transform.position, agent.destination) < Tile.Size * .5f)
                SetState((int)MonsterState.Idle);
        }

       
        void LoopHuntingState()
        {
            Vector3 preyPos = !usingExtraTime ? prey.transform.position : lastPrey.transform.position;

            float preyDistance = Vector3.Distance(preyPos, transform.position);
            if(preyDistance < attackRange)
            {
                usingExtraTime = false;
                SetState((int)MonsterState.Killing);
                return;
            }

            if (CheckForPlayer() || usingExtraTime)
            {

                if ((System.DateTime.Now - lastHuntingDT).TotalSeconds > huntingTime)
                {
                    if (huntingPath == null)
                        huntingPath = new NavMeshPath();
                    
                    lastHuntingDT = System.DateTime.Now;
                   
                    agent.CalculatePath(preyPos, huntingPath);
                    

                }

                if (huntingPath != null && (huntingPath.status == NavMeshPathStatus.PathComplete || huntingPath.status == NavMeshPathStatus.PathPartial))
                {
                    agent.SetPath(huntingPath);
                    huntingPath = null;
                }

                if (usingExtraTime)
                {
                    extraTimeCurrent -= Time.deltaTime;
                    if (extraTimeCurrent < 0)
                    {
                        usingExtraTime = false;
                        huntingPath = null;
                        SetState((int)MonsterState.PlayerLost);
                    }
                        
                }    
               
            }
            else
            {
               usingExtraTime = true;
               extraTimeCurrent = extraTime;
            }
        }

        void LoopPlayerSpottedState()
        {
        }

        void LoopPlayerLostState()
        {
        }


        #endregion
                

        #region private methods
        bool CheckForPlayer()
        {
           

            //Debug.Log("MONSTER - Checking for player...");
#if UNITY_EDITOR
            //return true;
#endif

            //if (players.Count != new List<PlayerRef>(Runner.ActivePlayers).Count)
            //    players.Clear();

            //if (players.Count == 0)
            players = new List<PlayerController>(FindObjectsOfType<PlayerController>());

            List<PlayerController> candidates = new List<PlayerController>();
            foreach(PlayerController player in players)
            {

                if (player.State != (int)PlayerState.Alive)
                    continue;

                if (Vector3.Distance(transform.position, player.transform.position) > sightRange)
                    continue; // Too far

                Vector3 dir = Vector3.zero;
                if (State == (int)MonsterState.Idle)
                {
                    dir = Vector3.ProjectOnPlane(head.forward, Vector3.up);
                    Vector3 pmDir = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up);
                    if (Vector3.Angle(dir, pmDir) > sightAngle * .5f)
                        continue;
                }
                else
                {
                    dir = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up);
                }

                //Vector3 dir = Vector3.ProjectOnPlane(head.forward, Vector3.up);
                //Vector3 pmDir = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up);
                //if (Vector3.Angle(dir, pmDir) > sightAngle * .5f)
                //    continue;

                //dir += Vector3.up * 1.5f;
                
                
                


                LayerMask mask = LayerMask.GetMask(new string[] { Layers.Wall });
                if (Physics.Raycast(transform.position + Vector3.up, dir.normalized, dir.magnitude, mask))
                    continue; // A wall is stopping the sight

                

                candidates.Add(player);
            }

            if(candidates.Count > 0)
            {
                PlayerController tmp = candidates[Random.Range(0, candidates.Count)];
                if(prey != tmp)
                {
                   
                    prey = tmp;
                    huntingPath = null;
                }
                    
                return true;
            }
            else
            {
                if (prey)
                {
                    lastPrey = prey;
                    prey = null;
                    huntingPath = null;
                }
                return false;
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Returns a reacheable point in the navmesh
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDestination()
        {
            // Load all the player controllers into a list
            //if(players.Count == 0)
            //    players = new List<PlayerController>(FindObjectsOfType<PlayerController>());
            

            bool trackPlayer = Random.Range(0, monsterNoTrackMax) == 0;
#if UNITY_EDITOR
            //trackPlayer = true; // TO REMOVE
#endif
            
            if (trackPlayer)
            {
                players = new List<PlayerController>(FindObjectsOfType<PlayerController>());
                Transform target = players[Random.Range(0, players.Count)].transform;
                return target.position;
            }
            else
            {
                
                //Sector sector = builder.GetSector(tile.sectorIndex);
                Sector sector = builder.GetCurrentPlayingSector();
                
                int targetTileId = sector.TileIds[Random.Range(0, sector.TileIds.Count)];
                Vector3 pos = builder.GetTile(targetTileId).GetPosition();
                pos += Vector3.right * Tile.Size * .5f + Vector3.back * Tile.Size * .5f;
                return pos;
            }
            
            
        }

        public void SetIdleState()
        {
            SetState((int)MonsterState.Idle);
        }

        public void SetPlayerEscapedState()
        {
            SetState((int)MonsterState.PlayerEscaped);
        }

        public void Init()
        {
            GetComponent<NavMeshAgent>().enabled = false;
        }

        #endregion
    }

}
