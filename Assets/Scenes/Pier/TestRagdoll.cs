using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRagdoll : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    [SerializeField]
    Rigidbody spineRB;

    // Start is called before the first frame update
    void Start()
    {
        //fakeHead.SetActive(false);   
    }

    // Update is called once per frame
    void Update()
    {
    

        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(DoBite());
        }
    }

  
    IEnumerator DoBite()
    {
        animator.enabled = false;
        yield return new WaitForEndOfFrame();
        Rigidbody[] bones = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody bone in bones)
        {

            bone.AddForce(transform.up * 10, ForceMode.VelocityChange);

        }

        //spineRB.AddForce(Vector3.up * 120f, ForceMode.Impulse);

        yield break;
    }


}
