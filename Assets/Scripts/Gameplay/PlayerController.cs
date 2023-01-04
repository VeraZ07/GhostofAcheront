using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class PlayerController : NetworkBehaviour
    {
      
        NetworkCharacterControllerPrototype cc;

        private void Awake()
        {
            cc = GetComponent<NetworkCharacterControllerPrototype>();
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
            
        }

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (GetInput(out NetworkInputData data))
            {
                cc.Move(new Vector3(data.leftAxis.x, 0f, data.leftAxis.y));
            }

            
        }

    }

}
