using Fusion;
using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GOA
{
    public class BullController : NetworkBehaviour
    {
        //[UnitySerializeField] [Networked(OnChanged = nameof(OnIsDeadChanged))] NetworkBool IsDead { get; set; }

        NavMeshAgent agent;
        bool isDead = false;
        float idleTime = 0;
        float minIdleTime = 30;
        float maxIdleTime = 60;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            agent.enabled = true;
        }

        public override void Spawned()
        {
            base.Spawned();


        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if ((!Runner.IsSinglePlayer && !Runner.IsSharedModeMasterClient) || isDead)
                return;


        }

       

        //public static void OnIsDeadChanged(Changed<BullController> changed)
        //{

        //}
    }

}
