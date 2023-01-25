using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GOA
{
    public class Monster : MonoBehaviour
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

        // Update is called once per frame
        void Update()
        {
            if (hidden)
                return;

            destination = target.position;

            if((System.DateTime.Now - lastPathTime).TotalSeconds > pathTime)
            {
                lastPathTime = System.DateTime.Now;
                agent.SetDestination(target.position);
            }
            
        }

        public void Show()
        {
            hidden = false;
            meshRoot.SetActive(true);
        }

        public void Hide()
        {
            hidden = true;
            meshRoot.SetActive(false);
        }
    }

}
