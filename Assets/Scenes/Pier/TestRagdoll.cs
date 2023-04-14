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
            StartCoroutine(Bite(bitePivot, targetNode));
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

    IEnumerator Bite(GameObject bitePivot, GameObject targetNode)
    {
        Debug.Log("Ragdolling...");
        // Disable target animator
        FixedJoint joint = bitePivot.GetComponent<FixedJoint>();
        
        Vector3 dir = joint.transform.position - targetNode.transform.position;
        //yield return targetNode.transform.root.DOMove(targetNode.transform.root.position + dir, 0.2f).WaitForCompletion();
        
        targetNode.transform.root.position += dir;
        yield return new WaitForEndOfFrame();

        joint.connectedBody = targetNode.GetComponent<Rigidbody>();
        targetNode.transform.root.GetComponent<Animator>().enabled = false;
        //joint.anchor = Vector3.up;

        yield return new WaitForSeconds(5f);
        head.SetActive(false);
        fakeHead.SetActive(true);
        fakeHead.transform.parent = null;
        joint.connectedBody = null;
    }
}
