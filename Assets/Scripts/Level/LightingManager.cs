using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{
    public class LightingManager : MonoBehaviour
    {
        [SerializeField]
        GameObject globalVolumePrefab;

        [SerializeField]
        GameObject localVolumetricFogPrefab;

        GameObject globalVolume;
        List<GameObject> localVolumetricFogList;

        Transform root;

       

        // Start is called before the first frame update
        void Start()
        {
            CreateLighting(FindObjectOfType<LevelBuilder>());
        }

        // Update is called once per frame
        void Update()
        {

        }

        void CreateGlobalVolume()
        {
            globalVolume = GameObject.Instantiate(globalVolumePrefab, root);
            globalVolume.transform.localPosition = Vector3.zero;
            globalVolume.transform.localRotation = Quaternion.identity;
        }

        void CreateRootTransform()
        {
            root = new GameObject("Lighting").transform;
            root.transform.position = Vector3.zero;
            root.transform.rotation = Quaternion.identity;
        }

        public void CreateLighting(LevelBuilder builder)
        {
            CreateRootTransform();

            CreateGlobalVolume();
        }


    }

}
