using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class MaterialSetter : MonoBehaviour
    {
        [SerializeField]
        int materialId;

        [SerializeField]
        Material homeMaterial;

        [SerializeField]
        Material awayMaterial;

        Renderer rend;
        

        private void Awake()
        {
            rend = GetComponent<Renderer>();
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetHomeMaterial()
        {
            SetMaterial(homeMaterial);
        }

        public void SetAwayMaterial()
        {
            SetMaterial(awayMaterial);
        }

        void SetMaterial(Material material)
        {
            Material[] mats = rend.materials;
            mats[materialId] = material;
            rend.materials = mats;
        }
    }

}
