using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class PlayerInput : MonoBehaviour
    {
        public static PlayerInput Instance { get; private set; }
        
        Vector2 moveInput;
        Vector2 lookInput;
        bool run;
        bool leftAction;
        bool rightAction;

        PlayerController playerController;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                playerController = GetComponent<PlayerController>();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {
            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            moveInput.Normalize();
                
            lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            playerController.SetCameraPitch(lookInput.y);

            run = Input.GetAxisRaw("Fire3") == 1f;

            leftAction = Input.GetMouseButton(0);
            rightAction = Input.GetMouseButton(1);
        }

        private void OnDestroy()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public NetworkInputData GetInput()
        {
            var data = new NetworkInputData();
            data.move = moveInput;

            data.yaw = lookInput.x;

            data.run = run;
            data.leftAction = leftAction;
            data.rightAction = rightAction;

            return data;
        }
    }

}
