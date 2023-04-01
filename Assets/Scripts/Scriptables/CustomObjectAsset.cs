using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    public enum ObjectAlignment { Both, MiddleOnly, SideOnly }

    public class CustomObjectAsset : ScriptableObject
    {
        

        public const string ResourceFolder = "CustomObjects";

        [SerializeField]
        ObjectAlignment alignment = ObjectAlignment.Both;
        public ObjectAlignment Alignment
        {
            get { return alignment; }
        }

        [SerializeReference]
        int weight = 1;
        public int Weight
        {
            get { return weight; }
        }

        [SerializeField]
        GameObject prefab;

        public GameObject Prefab
        {
            get { return prefab; }
        }


    }



    
}
