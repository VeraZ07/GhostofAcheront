using Fusion;
using GOA.Interfaces;
using GOA.Level;
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
        bool isAttacking = false;
        float idleTime = 0;
        float minIdleTime = 30;
        float maxIdleTime = 60;
        int sectorId = -1;
        LevelBuilder.BullObjectGroup group;

        private void Awake()
        {
            
        }

        private void Start()
        {
            NavMesh.pathfindingIterationsPerFrame = GameConfig.PathfindingIterationsPerFrame;
            agent.enabled = true;
            idleTime = Random.Range(minIdleTime, maxIdleTime);
            // Get the id of the sctor the bull belongs to
            
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

            if (!isAttacking)
            {
                // Choose to move or stay
                if(idleTime > 0) // Stay
                {
                    // Decrease timer
                    idleTime -= Runner.DeltaTime;
                    
                }
                else
                {
                    // Get a new destination
                    SetWanderingDestination();
                }
                
            }
            else // Is attacking the player
            {

            }
        }

        void SetWanderingDestination()
        {
            List<int> freeTiles = group.Builder.GetSector(group.SectorId).TileIds.FindAll(tid=>group.Builder.TileIsFree(tid));
            int destId = freeTiles[Random.Range(0, freeTiles.Count)];
            Vector3 dest = group.Builder.GetTile(destId).GetCenterPosition();
            agent.SetDestination(dest);
        }

        public void Init(LevelBuilder.BullObjectGroup group)
        {
            if(!agent)
                agent = GetComponent<NavMeshAgent>();
            agent.enabled = false;
            this.group = group;
            
        }

    }

}
