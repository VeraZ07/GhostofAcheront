using Fusion;
using GOA.Assets;
using GOA.Interfaces;
using GOA.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public enum PlayerState { Paused, Alive, Dying, Dead }

    public class PlayerController : NetworkBehaviour
    {
        

        public static PlayerController Local { get; private set; }

        public const float InteractionMinimumDistance = 1.5f;

        [SerializeField]
        new Light light;

        [SerializeField]
        GameObject dustParticle;

        
        Animator animator;

        NetworkCharacterControllerPrototypeCustom cc;

        bool running = false;
        float runMultiplier = 2f;

        float rotationSpeed = 360f;

        float yaw = 0;
        float pitch = 0;
        GameObject cam;
        float defaultSpeed;

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
        public int State { get; private set; } = -1;

        int deadType = 0;

        private void Awake()
        {
            cc = GetComponent<NetworkCharacterControllerPrototypeCustom>();
            defaultSpeed = cc.maxSpeed;
            animator = GetComponentInChildren<Animator>();

        }

        // Start is called before the first frame update
        void Start()
        {
            
            
        }

        // Update is called once per frame
        void Update()
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

            if (interactable != null && interactable.IsInteractionEnabled())
            {
                // Set the cursor 
                if(HasInputAuthority)
                    CursorManager.Instance.StartGameCursorEffect();

                // Only the server can start an interaction
                if (Runner.IsServer)
                {
                    
                    if (interactable != null)
                    {
                        if (/*data.leftAction && */interactable.IsInteractionEnabled() && !interactable.IsBusy())
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

        //void Set

        public override void Spawned()
        {
            base.Spawned();

            
            if (HasInputAuthority)
            {
                gameObject.AddComponent<PlayerInput>();
            }

            cam = GetComponentInChildren<Camera>().gameObject;

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
                Local = this;
                Renderer[] rends = GetComponentsInChildren<Renderer>();
                foreach(Renderer rend in rends)
                    rend.gameObject.layer = 9;

                
            }

            if (Runner.IsServer)
            {
                State = (int)PlayerState.Alive;
            }

            // Destroy level camera if any
            Camera levelCam = new List<Camera>(GameObject.FindObjectsOfType<Camera>()).Find(c => c.transform.parent == null);
            if (levelCam)
                DestroyImmediate(levelCam.gameObject);
        }

        #region rpc
        /// <summary>
        /// Sent by the server to the client when interaction starts.
        /// </summary>
        [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
        public void RpcOpenItemSelector()
        {
            ItemSelectorUI.Instance.Open();
        }

        /// <summary>
        /// Sent by the client to the server when interaction stops.
        /// </summary>
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RpcCloseItemSelector()
        {
            lockedInteractable.StopInteraction(this);
            lockedInteractable = null;
        }

        /// <summary>
        /// Sent by the client to the server to use an item.
        /// </summary>
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RpcUseItemByName(string itemName)
        {
            Debug.LogFormat("Using item {0}", itemName);

            if (lockedInteractable.TryUseItem(itemName))
            {
                Inventory inventory = new List<Inventory>(FindObjectsOfType<Inventory>()).Find(i => i.PlayerId == PlayerId);
                inventory.RemoveItem(itemName);
            }
            
            //Inventory inventory = new List<Inventory>(GameObject.FindObjectsOfType<Inventory>()).Find(i => i.PlayerId == Runner.LocalPlayer.PlayerId);
            //ItemAsset asset = 
        }

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

        }

        void EnterAliveState()
        {

        }

        void EnterDyingState()
        {
            if (Runner.IsServer)
            {
                // Send rpc Die(type)
                cc.enabled = false;
                cc.Velocity = Vector3.zero;
                ResetAnimator();
            }
        }

        void EnterDeadState()
        {
            if (Runner.IsServer)
            {
                // Send rpc Die(type)
            }
        }

        #endregion

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

            }


        }

        void ResetAnimator()
        {
            animator.SetFloat(animParamSpeed, 0);
            animator.SetFloat(animParamAngle, 0);
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

        public void SetDyingState()
        {
            if (Runner.IsServer)
            {
                State = (int)PlayerState.Dying;
                //this.deadType = deadType;


            }
        }

        public void SetDeadState()
        {
            if (Runner.IsServer)
            {
                State = (int)PlayerState.Dead;
                //this.deadType = deadType;


            }
        }

        public static void OnStateChanged(Changed<PlayerController> changed)
        {
            Debug.Log("OnStateChanged:" + changed.Behaviour.State);

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
            }
        }
    }

}
