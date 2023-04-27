using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class FloorRotationFixer : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            transform.forward = Vector3.forward;

            //if(transform.forward != Vector3.forward)
            //{
            //    if(transform.forward == Vector3.back)
            //    {
            //        transform.rotation = Quaternion.LookRotation(Vector3.back, Vector3.up);
            //    }
            //    else if (transform.forward == Vector3.right)
            //    {
            //        transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
            //    }
            //    else if (transform.forward == Vector3.left)
            //    {
            //        transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
            //    }
            //}
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
