using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class PlayerController : NetworkBehaviour
    {
        public static PlayerController Local { get; private set; }        

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

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (GetInput(out NetworkInputData data))
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
                if(!HasInputAuthority)
                {
                    SetCameraPitch(data.pitch);
                }
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
