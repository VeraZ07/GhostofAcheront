using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDoTween : MonoBehaviour
{
    [SerializeField]
    GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DoSequence());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator DoSequence()
    {
        Debug.Log("Starting sequence...");
        Sequence seq = DOTween.Sequence();
        yield return seq.Append(target.transform.DOMoveX(4, 2)).WaitForCompletion();

        Debug.Log("Sequence completed.");
    }
}
