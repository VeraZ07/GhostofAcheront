using DG.Tweening;
using Fusion;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition;

namespace GOA
{
    /// <summary>
    /// The player spirit of any player is owned by the server
    /// </summary>
    public class PlayerSpirit : NetworkBehaviour
    {
        [SerializeField]
        float avoidingDistance = 5f;

        [SerializeField]
        float reviveDistance = 1f;

        [SerializeField]
        GameObject orb;

        [SerializeField]
        Light orbLight;

        //[UnitySerializeField] [Networked] public int PlayerId { get; private set; }

        Vector3 spawnPosition;

        [SerializeField]
        List<Transform> avoidingTargets = new List<Transform>(); // The players we are running from

        List<PlayerController> players;
        NavMeshAgent agent;
        Vector3 currentDirection;
        Vector3 velocity = Vector3.zero;
        bool reviving = false;
        PlayerController owner;
        string spineName = "mixamorig:Spine1";
        Transform spine;
        bool ready = false;
        bool reunion = false;

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
            if (Input.GetKeyDown(KeyCode.R))
                reunion = true;

            if (!HasStateAuthority || reviving || !ready)
                return;

            if (!reunion)
            {
                CheckTargetsToAvoid();

                if (avoidingTargets.Count > 0)
                    TryToAvoidTargets();
                else
                    MoveBackToTheDeadBody();

                CheckForResurrection();
            }
            else
            {
                // If the orb is closed to the player it can move inside the chest
                if(Vector3.Distance(transform.position, spawnPosition) < 1f)
                {
                    reunion = false;
                    EnterIntoThePlayer();
                }
            }
        }

        public override void Spawned()
        {
            base.Spawned();

            if(HasStateAuthority)
            {
                spawnPosition = transform.position;
                agent.enabled = true;
                players = new List<PlayerController>(FindObjectsOfType<PlayerController>()).FindAll(p => !p.HasStateAuthority);
            }

            // For all clients, we make the horb exit from the chest
            // The spirit and the dead player share the same playerId
            ExitFromThePlayer();
        }

        /// <summary>
        /// Server only
        /// </summary>
        /// <param name="playerId"></param>
        //public void Init(int playerId)
        //{
        //    PlayerId = playerId;
        //}

        void ExitFromThePlayer()
        {
            owner = new List<PlayerController>(FindObjectsOfType<PlayerController>()).Find(p => p.Object.StateAuthority == Object.StateAuthority);
            spine = new List<Transform>(owner.CharacterObject.GetComponentsInChildren<Transform>()).Find(t => spineName.ToLower().Equals(t.gameObject.name.ToLower()));
            Vector3 positionDefault = orb.transform.position;
            Transform parentDefault = orb.transform.parent;
            orb.transform.parent = spine;
            orb.transform.localPosition = Vector3.zero;
            orb.transform.DOMove(positionDefault, 1f).SetDelay(1f).OnComplete(() => { orb.transform.parent = parentDefault; ready = true; });
            orbLight.GetComponent<HDAdditionalLightData>().intensity = 1f;
        }

        void EnterIntoThePlayer()
        {
            reviving = true;
            owner.SetRisingAgainState();
            
            //reviving = true;
            //var lightData = orbLight.GetComponent<HDAdditionalLightData>();
            //DOTween.To(() => lightData.intensity, x => lightData.intensity = x, 72000f, 1f).OnComplete(() =>
            // {
            //     DOTween.To(() => lightData.intensity, x => lightData.intensity = x, 0f, .1f).SetDelay(0.4f);
            //    // Put the light outside
            //    orbLight.transform.parent = orbLight.transform.parent.parent;
            //     orb.SetActive(false);
            //     owner.SetRisingAgainState();
            // });

        }

        public IEnumerator ExplodeLight()
        {
            var lightData = orbLight.GetComponent<HDAdditionalLightData>();
            yield return DOTween.To(() => lightData.intensity, x => lightData.intensity = x, 72000f, 1f).WaitForCompletion();
            
            // Put the light outside
            orbLight.transform.parent = orbLight.transform.parent.parent;
            orb.SetActive(false);
            
        }

        public IEnumerator DimLight()
        {
            var lightData = orbLight.GetComponent<HDAdditionalLightData>();
            yield return DOTween.To(() => lightData.intensity, x => lightData.intensity = x, 0f, .1f).WaitForCompletion();
        }

        void CheckForResurrection()
        {
            foreach(PlayerController player in players)
            {
                if (player.State != (int)PlayerState.Alive || Vector3.Distance(player.transform.position, transform.position) > reviveDistance)
                    continue;

                //reviving = true;
                //PlayerController local = new List<PlayerController>(FindObjectsOfType<PlayerController>()).Find(p => p.HasStateAuthority);
                //local.SetRisingAgainState();

                reunion = true;
                agent.SetDestination(spawnPosition);
            }
        }

        void CheckTargetsToAvoid()
        {
            avoidingTargets.Clear();
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
                //velocity = Vector3.zero;
                currentDirection = Vector3.zero;
            }
        }

        void MoveBackToTheDeadBody()
        {
            agent.SetDestination(spawnPosition);
        }

        void TryToAvoidTargets()
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
            
        }
    }

}
