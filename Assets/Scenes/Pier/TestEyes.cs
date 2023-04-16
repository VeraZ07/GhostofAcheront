using GOA.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEyes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(Dead());
        }
    }

    IEnumerator Dead()
    {
        Debug.Log("Dead started");
        yield return EyesEffect.Instance.CloseEyes();
        Camera.main.transform.position += Vector3.up * 3;
        yield return EyesEffect.Instance.OpenEyes();
        Debug.Log("Dead completed");
    }
}
