using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    
    public class TileAsset : ScriptableObject
    {
        public const string ResourceFolder = "Tiles";

        [SerializeField]
        GameObject prefab;

        public GameObject Prefab
        {
            get { return prefab; }
        }
    }

}
