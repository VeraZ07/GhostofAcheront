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
        public ObjectAlignment alignment = ObjectAlignment.Both;
        public ObjectAlignment Alignment
        {
            get { return alignment; }
        }

        [SerializeField]
        GameObject prefab;

        public GameObject Prefab
        {
            get { return prefab; }
        }


    }

}
