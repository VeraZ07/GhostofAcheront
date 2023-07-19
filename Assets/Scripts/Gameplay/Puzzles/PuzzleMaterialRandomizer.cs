using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class PuzzleMaterialRandomizer : MonoBehaviour
    {
        [System.Serializable]
        class RendererElement
        {
            [SerializeField]
            public Renderer renderer;

            [SerializeField]
            public int materialId;
        }

        [SerializeField]
        List<Material> materials;

        [SerializeField]
        List<RendererElement> elements;



        // Start is called before the first frame update
        void Start()
        {
            Material mat = materials[Random.Range(0, materials.Count)];

            foreach(RendererElement element in elements)
            {
                Material[] mats = element.renderer.materials;
                mats[element.materialId] = mat;
                element.renderer.materials = mats;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
