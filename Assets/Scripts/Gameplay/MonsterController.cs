using DG.Tweening;
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
                
        float huntingTime = .5f;
        System.DateTime lastHuntingDT;
        NavMeshPath huntingPath;
        float attackDistance = 1.5f;

        [SerializeField]
        Transform[] biteTargets;


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
                        LoopKillingState();
                        break;
                }


                
                if (animator)
                {
                    animator.SetFloat(paramSpeed, agent.velocity.magnitude / agent.speed);
                }
                
            }
            
        }

        private void Update()
        {
            
            
        }

        public void Init()
        {
            GetComponent<NavMeshAgent>().enabled = false;
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
            // Stop moving
            agent.isStopped = true;
            // Run a random animation
            int deadType = 0;
            animator.SetFloat(paramAttackType, deadType);
            animator.SetTrigger(paramAttack);

          
            StartCoroutine(Kill(prey, deadType));

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
            else if (!agent.hasPath && !agent.pathPending)
                SetState((int)MonsterState.Idle);

        }

       
        void LoopHuntingState()
        {

            float preyDistance = Vector3.Distance(prey.transform.position, transform.position);
            if(preyDistance < attackDistance)
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


        System.DateTime _startAnimTime;
        void StartAnim()
        {
            _startAnimTime = System.DateTime.Now;
        }
        void StopAnim()
        {
            Debug.Log("Time:" +(System.DateTime.Now - _startAnimTime).TotalSeconds);
        }

        IEnumerator Kill(PlayerController player, int deadType)
        {
            switch (deadType)
            {
                case 0:
                    float animationLength = 2.48f;
                    float total = 0f;
                    System.DateTime start;
                    Debug.Log("Start killing...");
                    agent.velocity = Vector3.zero;
                    player.SetDyingState();

                    // Just wait a little bit
                    start = System.DateTime.Now;
                    yield return new WaitForSeconds(.1f);
                    
                    // Adjust the player position and rotation
                    player.transform.DORotateQuaternion(biteTargets[0].rotation, 0.5f);
                    yield return player.transform.DOMove(biteTargets[0].position, 0.5f).WaitForCompletion();

                    total = (float)(System.DateTime.Now - start).TotalSeconds;

                    // Wait for the monster to open its mouth
                    start = System.DateTime.Now;
                    yield return new WaitForSeconds(1.36f - total);
                    // Bite the player



                    yield return new WaitForSeconds(animationLength/* - 0.6f*/);
                    

                    player.SetDeadState();
                    agent.isStopped = false;
                    SetState((int)MonsterState.Idle);
                    break;

            }
        }

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
