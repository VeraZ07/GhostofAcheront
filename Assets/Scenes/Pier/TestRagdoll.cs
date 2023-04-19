using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRagdoll : MonoBehaviour
{
    [SerializeField]
    GameObject bitePivot;

    [SerializeField]
    GameObject targetNode;

    [SerializeField]
    GameObject head;

    [SerializeField]
    GameObject fakeHead;

    // Start is called before the first frame update
    void Start()
    {
        fakeHead.SetActive(false);   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Bite(bitePivot, targetNode);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            bitePivot.GetComponent<FixedJoint>().anchor = Vector3.up * .5f;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            bitePivot.GetComponent<FixedJoint>().anchor = Vector3.down * .5f;
        }
    }

    void Bite(GameObject bitePivot, GameObject targetNode)
    {
        Debug.Log("Ragdolling...");
        // Disable target animator
        Joint joint = bitePivot.GetComponent<FixedJoint>();

        Vector3 oldPos = bitePivot.transform.position;
        bitePivot.transform.position = targetNode.transform.position;

        Vector3 dir = joint.transform.position - targetNode.transform.position;
        Debug.Log("Joint.Pos:" + joint.transform.position);
        Debug.Log("Target.Pos:" + targetNode.transform.position);
        Debug.Log("Dir:" + dir);


        //targetNode.transform.root.GetComponent<Animator>().enabled = false;
        /*yield return */
        //targetNode.transform.root.DOMove(targetNode.transform.root.position + dir, 0.1f, false).WaitForCompletion();
        joint.connectedBody = targetNode.transform.parent.GetComponent<Rigidbody>();

        //yield return new WaitForSeconds(.1f);
        targetNode.transform.root.GetComponent<Animator>().enabled = false;
        bitePivot.transform.position = oldPos;
        //yield return new WaitForSeconds(.1f);

        ////joint.anchor = Vector3.up;

        //yield return new WaitForSeconds(5f);
        //head.SetActive(false);
        //fakeHead.SetActive(true);
        //fakeHead.transform.parent = null;
        //joint.connectedBody = null;
    }


    void __Bite(GameObject bitePivot, GameObject targetNode)
    {
        Debug.Log("Ragdolling...");
        // Disable target animator
        Joint joint = bitePivot.GetComponent<ConfigurableJoint>();
        
        Vector3 dir = joint.transform.position - targetNode.transform.position;
        Debug.Log("Joint.Pos:" + joint.transform.position);
        Debug.Log("Target.Pos:" + targetNode.transform.position);
        Debug.Log("Dir:" + dir);


        //targetNode.transform.root.GetComponent<Animator>().enabled = false;
        /*yield return */
        //targetNode.transform.root.DOMove(targetNode.transform.root.position + dir, 0.1f, false).WaitForCompletion();
        joint.connectedBody = targetNode.transform.parent.GetComponent<Rigidbody>();

        //yield return new WaitForSeconds(.1f);
        targetNode.transform.root.GetComponent<Animator>().enabled = false;
        //yield return new WaitForSeconds(.1f);

        ////joint.anchor = Vector3.up;

        //yield return new WaitForSeconds(5f);
        //head.SetActive(false);
        //fakeHead.SetActive(true);
        //fakeHead.transform.parent = null;
        //joint.connectedBody = null;
    }


}
