using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TestOrbColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VisualEffect vfx = GetComponent<VisualEffect>();
        Color c = Color.red;
        Vector4 c2 = c;
        Debug.Log(c2);
        Debug.Log(vfx.GetVector4("Color"));

        vfx.SetVector4("Color", c2 * 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
