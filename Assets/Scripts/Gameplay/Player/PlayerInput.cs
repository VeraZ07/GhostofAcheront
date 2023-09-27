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
        float mouseSensMul = .2f;
        float mouseSens;
        
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                playerController = GetComponent<PlayerController>();
                //CursorManager.Instance.HideCursor();
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
            moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            moveInput.Normalize();
                
            lookInput = new Vector2(Input.GetAxis("Mouse X") * mouseSens, Input.GetAxis("Mouse Y") * mouseSens);

            if (!playerController.InputDisabled)
                if(playerController.State == (int)PlayerState.Alive || playerController.State == (int)PlayerState.Dead)
                    playerController.SetCameraPitch(lookInput.y);
                

            run = Input.GetAxisRaw("Fire3") == 1f;

            leftAction = Input.GetMouseButton(0);
            rightAction = Input.GetMouseButton(1);
        }

        private void OnEnable()
        {
            OptionManager.Instance.OnApply += HandleOnOptionApply;
            HandleOnOptionApply();
        }

        private void OnDisable()
        {
            OptionManager.Instance.OnApply -= HandleOnOptionApply;
        }

        private void OnDestroy()
        {
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
        }

        void HandleOnOptionApply()
        {
            mouseSens = OptionManager.Instance.CurrentMouseSensitivity * mouseSensMul;
        }

        public NetworkInputData GetInput()
        {
            var data = new NetworkInputData();
            data.move = moveInput;

            data.yaw = lookInput.x;
            data.pitch = lookInput.y;

            data.run = run;
            data.leftAction = leftAction;
            data.rightAction = rightAction;

            return data;
        }
    }

}
