using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class TestLight : MonoBehaviour
{
    private HDAdditionalLightData data;

    
    private void Awake()
    {
        data = GetComponent<HDAdditionalLightData>();
        Debug.Log("data.intensity:" + data.intensity);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            //data.lightUnit = LightUnit.Lumen;
            data.intensity += 20;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            //data.lightUnit = LightUnit.Lumen;
            data.intensity -= 20;
        }
    }
}
