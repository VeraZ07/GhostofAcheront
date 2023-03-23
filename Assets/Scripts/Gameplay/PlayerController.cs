using Fusion;
using GOA.Assets;
using GOA.Interfaces;
using GOA.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class PlayerController : NetworkBehaviour
    {
        public static PlayerController Local { get; private set; }

        public const float InteractionMinimumDistance = 1.5f;

        [SerializeField]
        new Light light;

        [SerializeField]
        GameObject dustParticle;

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

        private void Awake()
        {
            cc = GetComponent<NetworkCharacterControllerPrototypeCustom>();
            defaultSpeed = cc.maxSpeed;
                
        }

        // Start is called before the first frame update
        void Start()
        {
            
            
        }

        // Update is called once per frame
        void Update()
        {
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
                    Debug.LogFormat("Looking at {0}", info.collider.gameObject);
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
                        if (data.leftAction && interactable.IsInteractionEnabled() && !interactable.IsBusy())
                        {
                            lockedInteractable = interactable;
                            interactable.StartInteraction(this);
                            
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
            Vector3 move = transform.forward * data.move.y + transform.right * data.move.x;
            move.Normalize();

            if (data.run)
                cc.maxSpeed = defaultSpeed * runMultiplier;
            else
                cc.maxSpeed = defaultSpeed;


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

        #endregion

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (!InputDisabled && GetInput(out NetworkInputData data))
            {
                UpdateCharacter(data);

                CheckForInteraction(data);

            }

            


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
    }

}
