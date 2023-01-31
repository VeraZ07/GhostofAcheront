using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    public class CustomObjectAsset : ScriptableObject
    {
        public const string ResourceFolder = "CustomObjects";

        [SerializeField]
        GameObject prefab;

        public GameObject Prefab
        {
            get { return prefab; }
        }
    }

}
