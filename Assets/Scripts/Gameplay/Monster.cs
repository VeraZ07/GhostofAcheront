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

        bool hidden = false;

        NavMeshAgent agent;

        Vector3 destination;

        System.DateTime lastPathTime;
        float pathTime = 5f;

        System.DateTime lastPlayerTime; // The last time the monster went for one of the players
        float playerTime = 120f;

        List<PlayerController> playerControllers;
        float wanderingRange = 10f;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            
        }

        // Start is called before the first frame update
        void Start()
        {
            //Hide();
            NavMesh.pathfindingIterationsPerFrame = 500;
        }

        public override void Spawned()
        {
            base.Spawned();

            playerControllers = new List<PlayerController>(FindObjectsOfType<PlayerController>());
        }

        // Update is called once per frame
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (Runner.IsServer)
            {
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
            }

          
           
            
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
