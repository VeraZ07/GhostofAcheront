using DG.Tweening;
using Fusion;
using GOA.Interfaces;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static GOA.Level.LevelBuilder;

namespace GOA
{
    public enum MonsterState { Idle, Moving, PlayerSpotted, Hunting, PlayerLost, Killing }

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
        float sightRange = 8f;

        
        [SerializeField]
        Animator animator;

        IKiller deathMaker;        

        
        NavMeshAgent agent;

  
        string paramSpeed = "Speed";
        


        List<PlayerController> players = new List<PlayerController>();
        LevelBuilder builder;

        [Networked] public int State { get; private set; } = -1;

        float idleTimeMin = 10f;
        float idleTimeMax = 20f;
        float timer = 0;

        [SerializeField]
        PlayerController prey = null;
        Vector3 preyLastPosition;
                
        float huntingTime = .5f;
        System.DateTime lastHuntingDT;
        NavMeshPath huntingPath;
        float attackRange = .8f;

        [SerializeField]
        List<AttackerData> attackers;
        #endregion

        #region native

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            //animator = GetComponent<Animator>();
            deathMaker = GetComponent<IKiller>();
        }

        // Start is called before the first frame update
        void Start()
        {
            //Hide();
            NavMesh.pathfindingIterationsPerFrame = 250;
        }

        #endregion

        #region fusion overrides

        public override void Spawned()
        {
            base.Spawned();

            
            if (Runner.IsServer)
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

            if (Runner.IsServer)
            {

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
                }


                
                if (animator)
                {
                    animator.SetFloat(paramSpeed, agent.velocity.magnitude / agent.speed);
                }
                
            }
            
        }
        #endregion


        #region state management
        void SetState(int state)
        {
            if (State == state)
                return;
            Debug.Log("Monster - Setting new state: " + (MonsterState)state);
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
            }
        }

        void EnterIdleState()
        {
            if(agent.hasPath)
                agent.ResetPath();
            timer = Random.Range(idleTimeMin, idleTimeMax);
        }

        void EnterMovingState()
        {
            if (!agent.hasPath && !agent.pathPending)
                agent.SetDestination(GetDestination());
        }

        void EnterHuntingState()
        {
            
        }


        void EnterKillingState()
        {
            // Get an attacker data from the list
            AttackerData data = attackers[Random.Range(0, attackers.Count)];
            // Get the corresponding attacker component
            IKiller attacker = GetComponent<IKiller>();
            // Call the attacker
            attacker.Kill(prey, data.attackId);
        }



        void EnterPlayerLostState()
        {
            huntingPath = null;
        }

        void LoopKillingState()
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

            float preyDistance = Vector3.Distance(prey.transform.position, transform.position);
            if(preyDistance < attackRange)
            {
                SetState((int)MonsterState.Killing);
                return;
            }

            if (CheckForPlayer())
            {

                if ((System.DateTime.Now - lastHuntingDT).TotalSeconds > huntingTime)
                {
                    if (huntingPath == null)
                        huntingPath = new NavMeshPath();
                    
                    lastHuntingDT = System.DateTime.Now;
                   
                    agent.CalculatePath(prey.transform.position, huntingPath);
                    
                }

                if (huntingPath != null && (huntingPath.status == NavMeshPathStatus.PathComplete || huntingPath.status == NavMeshPathStatus.PathPartial))
                {
                    agent.SetPath(huntingPath);
                    huntingPath = null;
                }
                    
                    
               
            }
            else
            {
                huntingPath = null;
                SetState((int)MonsterState.PlayerLost);
            }
        }

        void LoopPlayerSpottedState()
        {
            SetState((int)MonsterState.Hunting);
        }

        void LoopPlayerLostState()
        {
            SetState((int)MonsterState.Moving);
        }


        #endregion

        #region private methods
        bool CheckForPlayer()
        {
            //Debug.Log("MONSTER - Checking for player...");
#if UNITY_EDITOR
            //return false;
#endif

            if (players.Count == 0)
                players = new List<PlayerController>(FindObjectsOfType<PlayerController>());

            List<PlayerController> candidates = new List<PlayerController>();
            foreach(PlayerController player in players)
            {
                if (player.State != (int)PlayerState.Alive)
                    continue;

                if (Vector3.Distance(transform.position, player.transform.position) > sightRange)
                    continue; // Too far

                Vector3 dir = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up);
                LayerMask mask = LayerMask.GetMask(new string[] { Layers.Wall });
                if (Physics.Raycast(transform.position + Vector3.up, dir.normalized, dir.magnitude, mask))
                    continue; // A wall is stopping the sight

                //Debug.Log("MONSTER - Adding new prey candidate:" + player);

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
                    preyLastPosition = prey.transform.position;
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
            if(players.Count == 0)
                players = new List<PlayerController>(FindObjectsOfType<PlayerController>());

            bool trackPlayer = Random.Range(0, 4) == 0;

            if (trackPlayer)
            {
                Transform target = players[Random.Range(0, players.Count)].transform;
                return target.position;
            }
            else
            {
                Puzzle nextPuzzle = builder.GetNextPuzzleToSolve();
                Debug.Log("NextPuzzleToSolve:" + nextPuzzle);
                int puzzleId = builder.GetPuzzleId(nextPuzzle);
                Debug.Log("NextPuzzleToSolve.Id:" + puzzleId);
                int gateIndex = new List<CustomObject>(builder.CustomObjects).FindIndex(g => g.GetType() == typeof(Gate) && (g as Gate).PuzzleIndex == puzzleId);
                Debug.Log("Gate.Id:" + gateIndex);
                Connection conn = new List<Connection>(builder.Connections).Find(c => c.gateIndex == gateIndex);
                Debug.Log("Conn:" + conn);
                Tile tile = builder.GetTile(conn.SourceTileId);
                Debug.Log("Tile:" + tile);
                Sector sector = builder.GetSector(tile.sectorIndex);
                Debug.Log("Sector:" + sector);

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

        public void Init()
        {
            GetComponent<NavMeshAgent>().enabled = false;
        }

        #endregion
    }

}
