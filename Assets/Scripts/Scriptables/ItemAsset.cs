using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    public class ItemAsset : ScriptableObject
    {
        public const string ResourceFolder = "Items";

        [SerializeField]
        Sprite icon;
        public Sprite Icon
        {
            get { return icon; }
        }

        [SerializeField]
        string description;

        [SerializeField]
        string displayName;

        //[SerializeField]
        //GameObject sceneObjectPrefab;
        //public GameObject SceneObjectPrefab
        //{
        //    get { return sceneObjectPrefab; }
        //}
    }

}
