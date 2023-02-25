using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOcclusion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //GetComponent<MeshRenderer>().forceRenderingOff = true;
    }

    private void LateUpdate()
    {
        //GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Other:" + other);
    }
}
