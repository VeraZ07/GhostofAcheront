using Fusion;
using GOA.Settings;
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
        //public bool LeftAction
        //{
        //    get { return leftAction; }
        //}
        bool rightAction;

        PlayerController playerController;
        float mouseSens;
        float mouseVertical;
        
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
                
            lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * mouseVertical) * mouseSens;
            

            if (!playerController.InputDisabled)
                if(playerController.State == (int)PlayerState.Alive || playerController.State == (int)PlayerState.Dead || playerController.State == (int)PlayerState.Dying)
                    playerController.SetCameraPitch(lookInput.y);
                

            run = Input.GetButton("Fire3");

            leftAction = Input.GetButton("Fire1");
            rightAction = Input.GetButton("Fire2");
        }

        private void OnEnable()
        {
            OptionManager.OnApply += HandleOnOptionApply;
            HandleOnOptionApply();
        }

        private void OnDisable()
        {
            OptionManager.OnApply -= HandleOnOptionApply;
        }

        private void OnDestroy()
        {
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;
        }

        void HandleOnOptionApply()
        {
            mouseSens = OptionCollection.MouseSensitivityOptionList[OptionManager.Instance.MouseSensitivityCurrentId].Value;
            mouseVertical = OptionCollection.InvertedMouseOptionList[OptionManager.Instance.VerticalMouseCurrentId].Value;
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
