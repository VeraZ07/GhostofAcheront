using Fusion;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GOA
{
    /// <summary>
    /// The player spirit of any player is owned by the server
    /// </summary>
    public class PlayerSpirit : NetworkBehaviour
    {
        [SerializeField]
        float avoidingDistance = 5f;

        [UnitySerializeField] [Networked] public int PlayerId { get; private set; }

        Vector3 spawnPosition;

        [SerializeField]
        List<Transform> avoidingTargets = new List<Transform>(); // The players we are running from

        List<PlayerController> players;
        NavMeshAgent agent;
        Vector3 currentDirection;
        Vector3 velocity = Vector3.zero;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.enabled = false;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            //GetComponentInChildren<SpriteRenderer>().transform.LookAt(Camera.main.transform);
            CheckTargetsToAvoid();

            if (avoidingTargets.Count > 0)
                TryToAvoidTargets();
            else
                MoveBackToTheDeadBody();
        }

        public override void Spawned()
        {
            base.Spawned();

            spawnPosition = transform.position;

            if (Runner.IsServer)
            {
                agent.enabled = true;
                players = new List<PlayerController>(FindObjectsOfType<PlayerController>()).FindAll(p => p.PlayerId != PlayerId);
            }
        }

        /// <summary>
        /// Server only
        /// </summary>
        /// <param name="playerId"></param>
        public void Init(int playerId)
        {
            
            PlayerId = playerId;
        }

        void CheckTargetsToAvoid()
        {
            //avoidingTargets.Clear();
            foreach(PlayerController player in players)
            {
                if (player.State != (int)PlayerState.Alive)
                    continue;
                Vector3 dir = transform.position - player.transform.position;
                float dist = dir.magnitude;
                if (dist < avoidingDistance)
                {
                    // We dont want to avoid a player behind a wall
                    Ray ray = new Ray(player.transform.position + Vector3.up, dir.normalized);
                    if (!Physics.Raycast(ray, dir.magnitude, LayerMask.GetMask(new string[] { Layers.Wall })))
                        avoidingTargets.Add(player.transform);
                    
                }
            }

            if(avoidingTargets.Count > 0)
            {
                //if (agent.enabled)
                //    agent.enabled = false;
            }
            else
            {
                //if (!agent.enabled)
                //    agent.enabled = true;
                velocity = Vector3.zero;
                currentDirection = Vector3.zero;
            }
        }

        void MoveBackToTheDeadBody()
        {

        }

        void TryToAvoidTargets()
        {
            if (avoidingTargets.Count > 0)
            {

                // Check all the directions that should be avoided
                List<Vector3> avoidingDirections = new List<Vector3>();
                foreach (Transform target in avoidingTargets)
                    avoidingDirections.Add(Vector3.ProjectOnPlane(target.position - transform.position, Vector3.up));

                // Check if the last direction is still valid
                float minAngle = 45;
                float minDist = LevelBuilder.TileSize / 2f;
                Vector3 oldDir = currentDirection;
                if (currentDirection != Vector3.zero)
                {
                    // Check avoiding directions
                    for (int i = 0; i < avoidingDirections.Count && currentDirection != Vector3.zero; i++)
                    {
                        if (Vector3.Angle(currentDirection, avoidingDirections[i]) < minAngle)
                            currentDirection = Vector3.zero; // We reset the last direction
                    }
                    // Check raycast
                    Ray ray = new Ray(transform.position + Vector3.up, currentDirection);
                    if (Physics.Raycast(ray, minDist, LayerMask.GetMask(new string[] { Layers.Wall })))
                        currentDirection = Vector3.zero;
                }

                if (currentDirection == Vector3.zero)
                {
                    // If the last direction is zero we don't have valid direction, so we try to get one
                    List<Vector3> candidates = new List<Vector3>();
                    candidates.Add(Vector3.forward);
                    candidates.Add(Vector3.right);
                    candidates.Add(Vector3.back);
                    candidates.Add(Vector3.left);
                    candidates.Remove(oldDir); // Eventually remove the last direction from candidates
                    while (currentDirection == Vector3.zero && candidates.Count > 0)
                    {
                        currentDirection = candidates[Random.Range(0, candidates.Count)];
                        candidates.Remove(currentDirection);

                        // Check avoiding directions
                        for (int i = 0; i < avoidingDirections.Count && currentDirection != Vector3.zero; i++)
                        {
                            if (Vector3.Angle(currentDirection, avoidingDirections[i]) < minAngle)
                                currentDirection = Vector3.zero; // We reset the last direction
                        }
                        // Check raycast
                        Ray ray = new Ray(transform.position + Vector3.up, currentDirection);
                        if (Physics.Raycast(ray, minDist, LayerMask.GetMask(new string[] { Layers.Wall })))
                            currentDirection = Vector3.zero;
                    }
                }


                // If we have a valid direction we can run away
                if (currentDirection != Vector3.zero)
                {
                    Vector3 destination = transform.position + currentDirection * LevelBuilder.TileSize * .25f;
                    agent.SetDestination(destination);
                }
                else
                {
                    agent.SetDestination(spawnPosition);
                }


            }
            else
            {
                // Check players distance
            }
        }
    }

}
