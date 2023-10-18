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
        public GameObject CharacterObject
        {
            get { return characterObject; }
        }

        [SerializeField]
        GameObject spiritPrefab;


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
        NetworkObject spirit; // Only local player sets this field
        Vector3 deadPosition;
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
           
            EnableRagdollColliders(false);

        }

        // Update is called once per frame
        void Update()
        {
           

            switch (State)
            {
                case (int)PlayerState.Alive:
                    UpdateAnimations();
                    break;
            }


#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.P))
            //    RiseAgain();
          
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


            if (Runner.GameMode == GameMode.Shared)
            {
                if (Runner.IsSharedModeMasterClient)
                {
                    if (!HasInputAuthority)
                        Object.ReleaseStateAuthority();
                }
                else
                {
                    if (HasInputAuthority)
                        Object.RequestStateAuthority();
                }
                
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
        void UpdateAnimations()
        {
            float speed = cc.Velocity.magnitude * (Vector3.Angle(cc.Velocity, transform.forward) < 93f ? 1f : -1f);
            float angle = Vector3.SignedAngle(cc.Velocity.normalized, Mathf.Sign(speed) * transform.forward, Vector3.up);

            animSpeed = Mathf.MoveTowards(animSpeed, speed, Time.deltaTime * 10f);
            animAngle = Mathf.MoveTowardsAngle(animAngle, angle, Time.deltaTime * 360f);
            //if ( Mathf.Abs(speed) < 0.1f)
            //    speed = Mathf.Abs(speed);
            animator.SetFloat(animParamSpeed, animSpeed / (defaultSpeed * runMultiplier));
            animator.SetFloat(animParamAngle, -animAngle / 180f);
        }

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
             
                // Check if we are looking at any interactor
                RaycastHit info;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out info, InteractionMinimumDistance))
                {
                  
                    interactable = info.collider.GetComponent<IInteractable>();
                }
            }

            if (interactable != null && interactable.IsInteractionEnabled()/* && !interactable.IsBusy()*/)
            {
                
                // Everyone can start check for interaction
                if (HasInputAuthority)
                {
                    // Set the cursor 
                    CursorManager.Instance.StartGameCursorEffect();

                    
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
                                    //lockedInteractable = interactable;
                                    //interactable.StartInteraction(this);

                                    RpcInteract(InteractableManager.Instance.GetInteractableId(interactable));
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

        [Rpc(sources:RpcSources.StateAuthority, targets:RpcTargets.All, Channel = RpcChannel.Reliable)]
        public void RpcInteract(int interactableId)
        {
            if (!Runner.IsServer && !Runner.IsSharedModeMasterClient)
                return;
            IInteractable interactable = InteractableManager.Instance.GetInteractable(interactableId);
            lockedInteractable = interactable;
            interactable.StartInteraction(this);
        }

        void UpdateCharacter(NetworkInputData data)
        {

            if (Input.GetKeyDown(KeyCode.O))
            {
                cc.Transform.position = cc.Transform.position + cc.Transform.forward * 3f;
            }

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

      
        void ResetAnimator()
        {
            animator.SetFloat(animParamSpeed, 0);
            animator.SetFloat(animParamAngle, 0);
        }

        IEnumerator DoLookAtYouDying()
        {
            MonsterController monster = FindObjectOfType<MonsterController>();
            yield return EyesEffect.Instance.CloseEyes();

            // Move the camera vertically
            Camera.main.transform.position = transform.position + Vector3.up * Level.LevelBuilder.TileHeight * .8f;// + dir.normalized * dist;
            // Switch the post processing 
            //UnityEngine.Rendering.Volume volume = FindObjectOfType<UnityEngine.Rendering.Volume>();
            //volume.profile = FindObjectOfType<LevelBuilder>().GlobalVolumeGhostProfile;
            VolumeUtility.SetProfile(FindObjectOfType<LevelBuilder>().GlobalVolumeGhostProfile);
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
            if (!InputDisabled && GetInput(out NetworkInputData data))
            {
                UpdateCharacter(data);

            }
        }



        void LoopDeadState()
        {
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

        void SetDyingState()
        {
            if (HasStateAuthority)
                State = (int)PlayerState.Dying;
        }

        void SetSacrificedState()
        {
            if(HasStateAuthority)
                State = (int)PlayerState.Sacrificed;
        }

        public void SetEscapedState()
        {
            if(HasStateAuthority)
                State = (int)PlayerState.Escaped;
        }

        void EnterDyingState()
        {
            //cc.enabled = false;
            //cc.Velocity = Vector3.zero;
            ResetAnimator();
            EnableRagdollColliders(true);

            // Store the dead position
            deadPosition = transform.position;
            // Move the character out of the controller
            characterObject.transform.parent = null;
        }

       

        void EnterDeadState()
        {
          
            if (Runner.IsServer || Runner.IsSharedModeMasterClient)
            {
                FindObjectOfType<GameManager>().PlayerHasDead(this);
            }

            // UNCOMMENT WHEN READY
            if (HasStateAuthority)
            {
                Debug.Log("Create spirit:" + gameObject.name);
                // Spawn the player spirit
                spirit = Runner.Spawn(spiritPrefab, transform.position, Quaternion.identity);
            }




        }

        void EnterRisingAgainState()
        {
            StartCoroutine(DoRiseAgain());

            
        }

        IEnumerator DoRiseAgain()
        {
            PlayerSpirit pSpirit = GetSpirit().GetComponent<PlayerSpirit>();
            yield return pSpirit.ExplodeLight();
            if (HasStateAuthority)
            {
                yield return EyesEffect.Instance.CloseEyes();
                cam.transform.localPosition = cameraLocalPositionDefault;
                cam.transform.localRotation = cameraLocalRotationDefault;

                //cc.TeleportToPosition(deadPosition);
                cc.Transform.position = deadPosition;
                //cc.WritePosition(deadPosition);
                VolumeUtility.SetProfile(FindObjectOfType<LevelBuilder>().GlobalVolumeDefaultProfile);
                SetRenderingLayer(LayerMask.NameToLayer(Layers.LocalCharacter));
                yield return new WaitForSeconds(.5f);
            
            }

            yield return pSpirit.DimLight();

            

            // On both client and server
            EnableRagdollColliders(false);
            characterObject.transform.parent = characterRoot;
            characterObject.transform.localPosition = Vector3.zero;
            characterObject.transform.localRotation = Quaternion.identity;
            animator.enabled = true;
            pitch = 0f;
            ResetAnimator();
            headMesh.SetActive(true);

            if (HasInputAuthority)
            {
                cc.Transform.position = deadPosition;
                yield return EyesEffect.Instance.OpenEyes();
            }
                

            yield return new WaitForSeconds(1f);
            if (HasStateAuthority)
            {
                State = (int)PlayerState.Alive;
                //cc.Transform.position = deadPosition;
                yield return new WaitForSeconds(1f);
                Runner.Despawn(spirit);
                
            }
                
            
        }

        public static void OnStateChanged(Changed<PlayerController> changed)
        {
           
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

        
        public NetworkObject GetSpirit()
        {
            if (!spirit)
                spirit = new List<PlayerSpirit>(FindObjectsOfType<PlayerSpirit>()).Find(p => p.Object.StateAuthority == Object.StateAuthority).Object;

            return spirit;
        }
        

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


        //[Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority, Channel = RpcChannel.Reliable, InvokeLocal = false)]
        //public void RpcSetDeadState(RpcInfo info = default)
        //{
        //    SetDeadState();
        //}
        public void SetDeadState()
        {
            if (HasStateAuthority)
            {
                State = (int)PlayerState.Dead;

            }
        }

        [Rpc(sources: RpcSources.All,targets:RpcTargets.StateAuthority, Channel = RpcChannel.Reliable, InvokeLocal = true )]
        public void RpcSetDyingState(RpcInfo info = default)
        {
            SetDyingState();
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
            if (this.HasInputAuthority)
            {
                StartCoroutine(DoLookAtYouDying());
            }
        }

        public void SetRisingAgainState()
        {
            if (HasStateAuthority)
            {
                State = (int)PlayerState.RisingAgain;
            }
        }


        [Rpc(sources: RpcSources.All, targets: RpcTargets.StateAuthority, Channel = RpcChannel.Reliable, InvokeLocal = true)]
        public void RpcSetEscapedState()
        {
            SetEscapedState();
        }

        [Rpc(sources:RpcSources.All, targets:RpcTargets.StateAuthority, Channel = RpcChannel.Reliable, InvokeLocal = true)]
        public void RpcSetSacrificedState()
        {
            SetSacrificedState();
        }

        

#endregion
    }

}
