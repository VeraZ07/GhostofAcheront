using Fusion;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GOA
{
    public enum MonsterState { Idle, Moving, PlayerSpotted, Hunting, PlayerLost, Killing }

    public class MonsterController : NetworkBehaviour
    {
        [SerializeField]
        GameObject meshRoot;

        [SerializeField]
        float sightRange = 8f;

        
        [SerializeField]
        Animator animator;
        
        NavMeshAgent agent;

  
        string paramSpeed = "Speed";
        string paramAttack = "Attack";
        string paramAttackType = "AttackType";

        List<PlayerController> players = new List<PlayerController>();
        LevelBuilder builder;

        [Networked] public int State { get; private set; } = -1;

        float idleTimeMin = 10f;
        float idleTimeMax = 20f;
        float timer = 0;

        [SerializeField]
        PlayerController prey = null;
        Vector3 preyLastPosition;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            //animator = GetComponent<Animator>();
        }

        // Start is called before the first frame update
        void Start()
        {
            //Hide();
            NavMesh.pathfindingIterationsPerFrame = 250;
        }

       

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

                        break;
                }

               


                // Animation
                //Debug.Log("Monster Speed:" + agent.velocity.magnitude);
            }

          
           
            
        }

        private void Update()
        {
            if (animator)
            {
                animator.SetFloat(paramSpeed, agent.velocity.magnitude / agent.speed);
            }
                
        }

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
                    break;
                case (int)MonsterState.PlayerLost:
                    break;
            }
        }

        void EnterIdleState()
        {
            timer = Random.Range(idleTimeMin, idleTimeMax);
        }

        void EnterMovingState()
        {
            if (!agent.hasPath && !agent.pathPending)
                agent.SetDestination(GetDestination());
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
            else if (!agent.hasPath && !agent.pathPending)
                SetState((int)MonsterState.Idle);

        }

        void LoopHuntingState()
        {

            if (CheckForPlayer())
            {
                //Vector3 dir = Vector3.ProjectOnPlane(prey.transform.position - transform.position, Vector3.up);
                //agent.Move(-dir.normalized * Runner.DeltaTime * 20f);
                agent.destination = prey.transform.position + Vector3.up;
                //if (!agent.SetDestination(prey.transform.position)) // Follow the player
                //{
                //    Debug.Log("Destination error");
                //}
            }
            else
            {
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

        bool CheckForPlayer()
        {
            //Debug.Log("MONSTER - Checking for player...");

            if (players.Count == 0)
                players = new List<PlayerController>(FindObjectsOfType<PlayerController>());

            List<PlayerController> candidates = new List<PlayerController>();
            foreach(PlayerController player in players)
            {
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
                prey = candidates[Random.Range(0, candidates.Count)];
                return true;
            }
            else
            {
                if (prey)
                {
                    preyLastPosition = prey.transform.position;
                    prey = null;
                }
                return false;
            }
        }

        /// <summary>
        /// Returns a reacheable point in the navmesh
        /// </summary>
        /// <returns></returns>
        public Vector3 GetDestination()
        {
            // Load all the player controllers into a list
            if(players.Count == 0)
                players = new List<PlayerController>(FindObjectsOfType<PlayerController>());

            // 

            // Get a new target 
            Transform target = players[Random.Range(0, players.Count)].transform;

            
            return target.position;
            
        }


 

    }

}
