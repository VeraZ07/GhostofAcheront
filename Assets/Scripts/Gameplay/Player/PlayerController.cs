using DG.Tweening;
using Fusion;
using GOA.Assets;
using GOA.Interfaces;
using GOA.Level;
using GOA.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

namespace GOA
{
    public enum PlayerState { Paused, Alive, Dying, Dead, RisingAgain, Escaped, Sacrificed }

    public class PlayerController : NetworkBehaviour
    {

        #region fields
        //public static PlayerController Local { get; private set; }

        public const float InteractionMinimumDistance = 2f;

        [SerializeField]
        new Light light;

        [SerializeField]
        GameObject dustParticle;

        [SerializeField]
        GameObject characterObject;

        [SerializeField]
        List<Renderer> meshRenderers;

        [SerializeField]
        GameObject headPivot;
        public GameObject HeadPivot
        {
            get { return headPivot; }
        }

        [SerializeField]
        GameObject headMesh;
        //public GameObject HeadMesh
        //{
        //    get { return headMesh; }
        //}

        //[SerializeField]
        //VisualEffect headBloodVfx;

        [SerializeField]
        ParticleSystem headBloodParticles;

        Animator animator;

        NetworkCharacterControllerPrototypeCustom cc;

        bool running = false;
        float runMultiplier = 2f;

        float rotationSpeed = 360f;

        float yaw = 0;
        float pitch = 0;
        GameObject cam;
        float defaultSpeed;
        Transform camParent;
        Vector3 cameraLocalPositionDefault;
        Quaternion cameraLocalRotationDefault;

        public bool LeftAction { get; private set; }
        public bool RightAction { get; private set; }

        [Networked]
        [UnitySerializeField]
        public int PlayerId { get; private set; }

        public bool InputDisabled { get; set; }

        IInteractable lockedInteractable;
        bool leftInputDown = false;

        string animParamSpeed = "Speed";
        string animParamAngle = "Angle";
        float animSpeed = 0f;
        float animAngle = 0f;

        [Networked(OnChanged = nameof(OnStateChanged))]
        public int State { get; private set; } = 1;  

        int deadType = 0;
        Transform characterRoot;
        float ghostTime = 0f;
        #endregion

        #region native methods
        private void Awake()
        {
            cc = GetComponent<NetworkCharacterControllerPrototypeCustom>();
            defaultSpeed = cc.maxSpeed;
            animator = GetComponentInChildren<Animator>();
            //headBloodVfx.Stop();
            headBloodParticles.Stop();
            characterRoot = characterObject.transform.parent;
        }

        // Start is called before the first frame update
        void Start()
        {
            // Disable collisions between controller and internal ragdoll
            //Collider coll = GetComponent<Collider>();
            //Collider[] rc = GetComponentsInChildren<Collider>();
            //foreach(Collider c in rc)
            //{
            //    Physics.IgnoreCollision(coll, c, true);
            //}
            EnableRagdollColliders(false);
        }

        // Update is called once per frame
        void Update()
        {
           

            switch (State)
            {
                case (int)PlayerState.Alive:
                    float speed = cc.Velocity.magnitude * (Vector3.Angle(cc.Velocity, transform.forward) < 93f ? 1f : -1f);
                    float angle = Vector3.SignedAngle(cc.Velocity.normalized, Mathf.Sign(speed) * transform.forward, Vector3.up);

                    animSpeed = Mathf.MoveTowards(animSpeed, speed, Time.deltaTime * 10f);
                    animAngle = Mathf.MoveTowardsAngle(animAngle, angle, Time.deltaTime * 360f);
                    //if ( Mathf.Abs(speed) < 0.1f)
                    //    speed = Mathf.Abs(speed);
                    animator.SetFloat(animParamSpeed, animSpeed / (defaultSpeed * runMultiplier));
                    animator.SetFloat(animParamAngle, -animAngle / 180f);
                    break;
            }


#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.P))
            //    RiseAgain();
            if (Input.GetKeyDown(KeyCode.P))
            {
                //Inventory inv = new List<Inventory>(FindObjectsOfType<Inventory>()).Find(i => i.Object.HasInputAuthority);
                //Debug.Log("Found local inventory:" + inv);
                //if(inv.Items.Count > 0)
                //{
                //    Level.LevelBuilder builder = FindObjectOfType<Level.LevelBuilder>();

                //    Picker picker = new List<Picker>(FindObjectsOfType<Picker>()).Find(p => p.ItemAssetName == inv.Items[0].ToString() && p.Empty && builder.GetCurrentPlayingSector() == builder.GetSector(builder.GetTile(builder.CustomObjects[p.CustomObjectId].TileId).sectorIndex));
                //    Debug.Log("Found empty picker:" + picker.name);
                //    inv.RemoveItem(inv.Items[0].ToString());
                //    picker.Reactivate();
                //}
                //if (Runner.IsServer)
                //    State = (int)PlayerState.Dead;

                
            }
#endif
        }

       
        #endregion


        #region fusion 
        public override void Spawned()
        {
            base.Spawned();


            if (HasInputAuthority)
            {
                gameObject.AddComponent<PlayerInput>();
            }

            cam = GetComponentInChildren<Camera>().gameObject;
            camParent = cam.transform.parent;
            cameraLocalPositionDefault = cam.transform.localPosition;
            cameraLocalRotationDefault = cam.transform.localRotation;

            // Disable camera for non local player
            if (!HasInputAuthority)
            {
                // Create a fake cam to replay the original
                GameObject fakeCam = new GameObject("FakeCam");
                fakeCam.transform.parent = cam.transform.parent;
                fakeCam.transform.localPosition = cam.transform.localPosition;
                fakeCam.transform.localRotation = cam.transform.localRotation;
                DestroyImmediate(cam.gameObject);
                cam = fakeCam;

                // Destory light
                DestroyImmediate(light.gameObject);

                // Destroy dust
                DestroyImmediate(dustParticle);
            }
            else
            {
                //Local = this;
                SetRenderingLayer(LayerMask.NameToLayer(Layers.LocalCharacter));
             
            }

            //if (Runner.IsServer)
            //{
            //    State = (int)PlayerState.Alive;
            //}

            // Destroy level camera if any
            Camera levelCam = new List<Camera>(GameObject.FindObjectsOfType<Camera>()).Find(c => c.transform.parent == null);
            if (levelCam)
                DestroyImmediate(levelCam.gameObject);

        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);

            Debug.LogFormat("Despawned player " + this.Object.InputAuthority);

            if (HasInputAuthority)
            {
                // Move the camera outside
                cam.transform.parent = null;
            }

        }



        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            

            switch (State)
            {
                case (int)PlayerState.Alive:
                    LoopAliveState();
                    break;
                case (int)PlayerState.Dying:
                    LoopDyingState();
                    break;
                case (int)PlayerState.Dead:
                    LoopDeadState();
                    break;
                case (int)PlayerState.RisingAgain:
                    LoopRisingAgainState();
                    break;
            }


        }


        #endregion

        #region private methods
       

        void EnableRagdollColliders(bool value)
        {
            Collider[] rc = characterObject.GetComponentsInChildren<Collider>();
            foreach (Collider c in rc)
            {
                c.enabled = value;
            }
        }

        /// <summary>
        /// Simply checks if there is any object we can interact with ( we might look at it ).
        /// Both client and server.
        /// </summary>
        /// <returns></returns>
        void CheckForInteraction(NetworkInputData data)
        {
            

            // Check if we are closed to any interactable
            IInteractable interactable = null;
            if (Physics.OverlapSphere(cam.transform.position, InteractionMinimumDistance, LayerMask.GetMask(Layers.Interactable)) != null)
            {
                //Debug.LogFormat("Closed to some interactable");

                // Check if we are looking at any interactor
                RaycastHit info;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out info, InteractionMinimumDistance))
                {
                    //Debug.LogFormat("Looking at {0}", info.collider.gameObject);
                    interactable = info.collider.GetComponent<IInteractable>();
                }
            }

            if (interactable != null && interactable.IsInteractionEnabled()/* && !interactable.IsBusy()*/)
            {
                // Set the cursor 
                if(HasInputAuthority)
                    CursorManager.Instance.StartGameCursorEffect();

                // Only the server can start an interaction
                if (Runner.IsServer)
                {
                    
                    if (interactable != null)
                    {
                        if (interactable.IsInteractionEnabled()/* && !interactable.IsBusy()*/)
                        {
                            if (data.leftAction)
                            {
                                if (!leftInputDown)
                                {
                                    leftInputDown = true;
                                }
                            }
                            else
                            {
                                if (leftInputDown)
                                {
                                    leftInputDown = false;
                                    lockedInteractable = interactable;
                                    interactable.StartInteraction(this);
                                }
                            }
                            
                            
                        }
                    }   
                }
            }
            else
            {
                // Reset the cursor
                if (HasInputAuthority)
                    CursorManager.Instance.StopGameCursorEffect();
            }
   
        }

        void UpdateCharacter(NetworkInputData data)
        {
            
            // 
            // Apply rotation
            //
            cc.Rotate(data.yaw);

            //
            // Apply movement
            //
            Vector3 move;
            
            if (data.run)
            {
                move = transform.forward;
                cc.maxSpeed = defaultSpeed * runMultiplier;
            }
            else
            {
                move = transform.forward * data.move.y + transform.right * data.move.x;
                cc.maxSpeed = defaultSpeed;
            }

            move.Normalize();

            //cc.Move(move * Runner.DeltaTime);
            cc.Move(move);

            //
            // Actions
            //
            LeftAction = data.leftAction;
            RightAction = data.rightAction;

            // Set the camera pitch for the other players
            if (!HasInputAuthority)
            {
                SetCameraPitch(data.pitch);
            }
        }

        void UpdateGhost(NetworkInputData data)
        {
            // 
            // Apply rotation
            //
            cc.Rotate(data.yaw);

            //
            // Apply movement
            //
            Vector3 move;

            move = transform.forward * data.move.y + transform.right * data.move.x;
            cc.maxSpeed = defaultSpeed;

            move.Normalize();

            
            cc.Move(move);

            //
            // Actions
            //
            //LeftAction = data.leftAction;
            //RightAction = data.rightAction;

            // Set the camera pitch for the other players
            if (!HasInputAuthority)
            {
                SetCameraPitch(data.pitch);
            }
        }

        void ResetAnimator()
        {
            animator.SetFloat(animParamSpeed, 0);
            animator.SetFloat(animParamAngle, 0);
        }

        IEnumerator DoSwitchToGhostMode()
        {
            yield return EyesEffect.Instance.CloseEyes();
            characterObject.transform.parent = null;

            // Switch the post processing 
            UnityEngine.Rendering.Volume volume = FindObjectOfType<UnityEngine.Rendering.Volume>();
            volume.profile = FindObjectOfType<LevelBuilder>().GhostProfile;

            // Check a safe position for the player to move
            transform.position += Vector3.up * 3f + Vector3.right * 3f - Vector3.forward * 3f;
            Camera.main.transform.LookAt(transform);
            SetRenderingLayer(LayerMask.NameToLayer(Layers.Default)); 
            yield return EyesEffect.Instance.OpenEyes();
        }

        IEnumerator DoLookAtYouDying()
        {
           
            MonsterController monster = FindObjectOfType<MonsterController>();
            yield return EyesEffect.Instance.CloseEyes();

            

            //characterObject.transform.parent = null;
            Camera.main.transform.parent = null;
            // Check a safe position for the camera to move
            List<int> coords = new List<int>(new int[] { 0, 1, 2, 3});
            bool found = false;
            int coord = coords[0]; // We always try moving the camera forward
            coords.Remove(coord);
            Vector3 dir = Vector3.zero;
            float dist = Level.LevelBuilder.TileSize * .4f;
            while (!found && coords.Count>0)
            {
                Vector3 tmpDir = Quaternion.AngleAxis(90f * coord, Vector3.up) * monster.transform.forward;
                
                Vector3 origin = transform.position + Vector3.up * 3f;
                LayerMask mask = LayerMask.GetMask(new string[] { Layers.Wall });
                if (!Physics.Raycast(origin, tmpDir.normalized, dist, mask))
                {
                    dir = tmpDir;
                    found = true;
                }
                else
                {
                    coord = coords[Random.Range(0, coords.Count)];
                }
                    
            }
            Camera.main.transform.position = transform.position + Vector3.up * Level.LevelBuilder.TileHeight * .8f + dir.normalized * dist;
            Camera.main.transform.LookAt(HeadPivot.transform);
            // Switch the post processing 
            UnityEngine.Rendering.Volume volume = FindObjectOfType<UnityEngine.Rendering.Volume>();
            volume.profile = FindObjectOfType<LevelBuilder>().GhostProfile;
            SetRenderingLayer(LayerMask.NameToLayer(Layers.Default));
            yield return EyesEffect.Instance.OpenEyes();
        }

        IEnumerator StopHeadBloodVfx(float delay)
        {
            yield return new WaitForSeconds(delay);
            //headBloodVfx.Stop();
            headBloodParticles.Stop();
        }

        void SetRenderingLayer(int layer)
        {
            foreach (Renderer rend in meshRenderers)
                rend.gameObject.layer = layer;
        }
        #endregion

        #region state management

        void LoopAliveState()
        {
            if (!InputDisabled && GetInput(out NetworkInputData data))
            {
                UpdateCharacter(data);

                CheckForInteraction(data);

            }
        }

        void LoopDyingState()
        {
            
        }

        void LoopDeadState()
        {
            if (ghostTime > 0)
            {
                ghostTime -= Runner.DeltaTime;
                return;
            }

            if (!InputDisabled && GetInput(out NetworkInputData data))
            {
                UpdateCharacter(data);

            }
        }

        void LoopEscapedState()
        {

        }

        void LoopRisingAgainState()
        {

        }

        void EnterEscapedState()
        {
            if (HasInputAuthority)
                FindObjectOfType<GameManager>().YouWin();
        }

        void EnterSacrificedState()
        {
            if (HasInputAuthority)
                FindObjectOfType<GameManager>().YouLose();
        }

        void EnterAliveState()
        {

        }

        void EnterDyingState()
        {
            cc.enabled = false;
            cc.Velocity = Vector3.zero;
            ResetAnimator();
            EnableRagdollColliders(true);
        }

        void EnterDeadState()
        {
            
            // Both on client and server
            // Move the character out of the controller
            characterObject.transform.parent = null;
            // Move the controller back to the camera
            Vector3 pos = cam.transform.position;
            
            pos.y = 0.1f;
            transform.position = pos;
            cam.transform.parent = camParent;
            cc.enabled = true;
            cc.Velocity = Vector3.zero;

            ghostTime = 2f;

            if(Runner.IsServer)
                FindObjectOfType<GameManager>().PlayerHasDead(this);

            if (Runner.IsServer && !Runner.IsSinglePlayer)
            {
                Inventory inv = new List<Inventory>(FindObjectsOfType<Inventory>()).Find(i => i.Object.InputAuthority == this.Object.InputAuthority);
                inv.RespawnAllItems();
#if USE_HOST_MIGRATION
                SessionManager.Instance.PushSnapshot();
#endif
            }

            

        }

        void EnterRisingAgainState()
        {
            EnableRagdollColliders(false);
            // Both on client and server
            characterObject.transform.parent = characterRoot;
            characterObject.transform.localPosition = Vector3.zero;
            characterObject.transform.localRotation = Quaternion.identity;
            animator.enabled = true;
            pitch = 0f;
            cam.transform.localPosition = cameraLocalPositionDefault;
            cam.transform.localRotation = cameraLocalRotationDefault;
            ResetAnimator();
            headMesh.SetActive(true);
            SetRenderingLayer(LayerMask.NameToLayer(Layers.LocalCharacter));
            State = (int)PlayerState.Alive;
        }

        

        public static void OnStateChanged(Changed<PlayerController> changed)
        {
            Debug.LogFormat("Setting new state: {0}", (PlayerState)changed.Behaviour.State);
           
            switch (changed.Behaviour.State)
            {
                case (int)PlayerState.Alive:
                    changed.Behaviour.EnterAliveState();
                    break;
                case (int)PlayerState.Dying:
                    changed.Behaviour.EnterDyingState();
                    break;
                case (int)PlayerState.Dead:
                    changed.Behaviour.EnterDeadState();
                    break;
                case (int)PlayerState.RisingAgain:
                    changed.Behaviour.EnterRisingAgainState();
                    break;
                case (int)PlayerState.Escaped:
                    changed.Behaviour.EnterEscapedState();
                    break;
                case (int)PlayerState.Sacrificed:
                    changed.Behaviour.EnterSacrificedState();
                    break;
            }
        }
#endregion

#region public methods     

        public void SetCameraPitch(float value)
        {
            pitch += value * cc.lookSpeed * Runner.DeltaTime;
            pitch = Mathf.Clamp(pitch, -80, 80);
            cam.transform.localEulerAngles = Vector3.left * pitch;
        }

        
        public void Init(int playerId)
        {
            PlayerId = playerId;
        }

        /// <summary>
        /// This method is called by the monster server side
        /// </summary>
        public void SetDyingState()
        {
            if (Runner.IsServer)
            {
                State = (int)PlayerState.Dying;
            }
        }

        /// <summary>
        /// This method is called by the monster server side
        /// </summary>
        public void SetDeadState()
        {
            if (Runner.IsServer)
            {
                State = (int)PlayerState.Dead;
                //this.deadType = deadType;


            }
        }

        public void ExplodeHead()
        {
            headMesh.SetActive(false);
            //headBloodVfx.SendEvent("OnPlay");
            //headBloodVfx.Play();
            headBloodParticles.Play();
            StartCoroutine(StopHeadBloodVfx(8f));
        }

        public void LookAtYouDying()
        {
            Debug.Log("KILL - LookAtYouDying()");
            if (this.HasInputAuthority)
            {
                StartCoroutine(DoLookAtYouDying());
            }
        }

        public void RiseAgain()
        {
            if (Runner.IsServer)
            {
                State = (int)PlayerState.RisingAgain;
            }
        }

        public void SetEscapedState()
        {
            State = (int)PlayerState.Escaped;
        }

        public void SetSacrificedState()
        {
            Debug.Log("Sacrificed:" + gameObject.name);
            State = (int)PlayerState.Sacrificed;
        }

#endregion
    }

}
