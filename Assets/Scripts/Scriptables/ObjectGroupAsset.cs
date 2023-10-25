using GOA.Interfaces;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    [System.Serializable]
    public class ObjectGroupAsset : ScriptableObject//, IObjectGroup
    {
        public const string ResourceFolder = "ObjectGroups";

        [SerializeField]
        List<CustomObjectAsset> assets;
        public IList<CustomObjectAsset> Assets
        {
            get { return assets.AsReadOnly(); }
        }

        [SerializeField]
        [Range(0,100)]
        int weight = 1;
        public int Weight
        {
            get { return weight; }
        }

        
    }

}
