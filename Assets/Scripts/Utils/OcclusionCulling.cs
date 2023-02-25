using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class OcclusionCulling : MonoBehaviour
    {
        [SerializeField]
        Collider occlusionTrigger;

        Vector3[] outCorners = new Vector3[4];

        List<MeshRenderer> renderers = new List<MeshRenderer>();

        // Start is called before the first frame update
        void Start()
        {
            Camera.main.CalculateFrustumCorners(Camera.main.rect, Camera.main.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, outCorners);

            //renderers = new List<MeshRenderer>(FindObjectsOfType<MeshRenderer>());

            ////Debug.Log("Renderers:" + renderers.Count);

            //foreach (MeshRenderer renderer in renderers)
            //{
            //    //renderer.enabled = false;
            //}
        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("NearPlane:" + Camera.main.nearClipPlane);
            //Debug.Log("FarPlane:" + Camera.main.farClipPlane);

            Vector3 rightPoint = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * outCorners[2].z + Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up) * Mathf.Abs(outCorners[2].x);
            Vector3 leftPoint = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * outCorners[0].z + Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up) * outCorners[0].x;


            
            //Debug.Log("Left:" + leftPoint);
            //Debug.Log("Right:" + rightPoint);

            
        }

        //private void OnTriggerEnter(Collider other)
        //{
        //    Debug.Log("Other:" + other);
        //    MeshRenderer[] rends = other.GetComponentsInChildren<MeshRenderer>();
        //    foreach (MeshRenderer rend in rends)
        //        rend.enabled = true;

        //}

        //private void OnTriggerExit(Collider other)
        //{
        //    MeshRenderer[] rends = other.GetComponentsInChildren<MeshRenderer>();
        //    foreach (MeshRenderer rend in rends)
        //        rend.enabled = false;


        //}
    }

}
