using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class LightManager : MonoBehaviour
    {
        List<GameObject> lightList;

        // Start is called before the first frame update
        void Start()
        {
            LevelBuilder builder = GameObject.FindObjectOfType<LevelBuilder>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
