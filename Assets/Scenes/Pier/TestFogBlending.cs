using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class TestFogBlending : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;

    LocalVolumetricFog fog;

    private void Awake()
    {
        //gameObject.SetActive(false);
        fog = GetComponent<LocalVolumetricFog>();
        //fog.parameters.positiveFade.x = .88f;
        //fog.parameters.negativeFade.x = 2f;
        Debug.Log("PositiveFade:" + fog.parameters.positiveFade);

        //gameObject.SetActive(true);


        //LocalVolumetricFog fv = prefab.GetComponentInChildren<LocalVolumetricFog>();
        //Debug.Log("PositiveFade:" + fv.parameters.positiveFade);
        //fv.parameters.positiveFade.x = 2;
        //GameObject fo = Instantiate(prefab);

    }

    // Start is called before the first frame update
    void Start()
    {
        fog.parameters.positiveFade = Vector3.one * .5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            fog.enabled = false;
            fog.parameters.positiveFade.x = .6f;
            LocalVolumetricFog fog2 = Component.Instantiate(fog);
            fog2.parameters.positiveFade.x = 2f;
            Destroy(fog);
            fog.enabled = true;
        }
            
    }
}
