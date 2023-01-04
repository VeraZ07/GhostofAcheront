using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class PlayerInput : MonoBehaviour
    {
        static PlayerInput instance;

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
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

        }

        public static NetworkInputData GetInput()
        {
            var data = new NetworkInputData();
            data.leftAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (data.leftAxis.magnitude > 1f)
                data.leftAxis = data.leftAxis.normalized;

            return data;
        }
    }

}
