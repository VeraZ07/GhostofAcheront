using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GOA
{
    public class Monster : NetworkBehaviour
    {
        [SerializeField]
        GameObject meshRoot;

        [SerializeField]
        float sightRange = 8f;

        [SerializeField]
        Transform target;

        [SerializeField]
        Animator animator;

        bool hidden = false;

        NavMeshAgent agent;

        Vector3 destination;

        System.DateTime lastPathTime;
        float pathTime = 5f;

        System.DateTime lastPlayerTime; // The last time the monster went for one of the players
        float playerTime = 3;//120f;

        List<PlayerController> playerControllers;
        float wanderingRange = 10f;

        
        string paramSpeed = "Speed";
        string paramAttack = "Attack";
        string paramAttackType = "AttackType";


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

            playerControllers = new List<PlayerController>(FindObjectsOfType<PlayerController>());

            if(Runner.IsServer)
                GetComponent<NavMeshAgent>().enabled = true;
        }

        // Update is called once per frame
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            return;

            if (Runner.IsServer)
            {

                // Logic
                destination = target.position;

                if ((System.DateTime.Now - lastPathTime).TotalSeconds > pathTime)
                {
                    // Only try to get a new destination if you don't have any 
                    if (!agent.hasPath)
                    {
                        if (GetDestination(out destination))
                        {
                            lastPathTime = System.DateTime.Now;
                            agent.SetDestination(destination);
                        }
                    }

                }


                // Animation
                Debug.Log("Monster Speed:" + agent.velocity.magnitude);
            }

          
           
            
        }

        private void Update()
        {
            //if (!Runner)
            //{
            //    agent.SetDestination(target.position);
            //    // Animation
            //    Debug.Log("Monster CurrentSpeed:" + agent.velocity.magnitude);
            //    Debug.Log("Monster MaxSpeed:" + agent.speed);
            //    if(animator)
            //        animator.SetFloat(paramSpeed, agent.velocity.magnitude / agent.speed);
            //}
            
        }

        /// <summary>
        /// Returns a reacheable point in the navmesh
        /// </summary>
        /// <returns></returns>
        public bool GetDestination(out Vector3 destination)
        {
            destination = Vector3.zero;

            Vector3 origin = transform.position;

            if((System.DateTime.Now - lastPlayerTime).TotalSeconds > playerTime)
            {
                lastPlayerTime = System.DateTime.Now;
                // Get a random player
                PlayerController target = playerControllers[Random.Range(0, playerControllers.Count)];
                origin = target.transform.position;
                 
            }
               

            Vector3 point = origin + Random.insideUnitSphere * wanderingRange;
            NavMeshHit hit;
            if(NavMesh.SamplePosition(point, out hit, 1.0f, NavMesh.AllAreas))
            {
                destination = hit.position;
                return true;
            }

            return false;
        }
    }

}
