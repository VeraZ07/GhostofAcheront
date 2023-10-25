using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GOA.Level
{
    public partial class LevelBuilder : MonoBehaviour
    {
        [System.Serializable]
        public class DynamicObject
        {
            [SerializeField]
            ScriptableObject asset;
            public ScriptableObject Asset
            {
                get { return asset; }
            }

            [SerializeField]
            GameObject sceneObject;

            [SerializeField]
            int tileId;
            public int TileId
            {
                get { return tileId; }
            }

            public DynamicObject(ScriptableObject asset, int tileId)
            {
                this.asset = asset;
                this.tileId = tileId;
            }
        }

        [SerializeReference]
        List<DynamicObject> dynamicObjects = new List<DynamicObject>();
        
        public int DynamicObjectCount
        {
            get { return dynamicObjects.Count; }
        }
        
        public DynamicObject GetDynamicObjectById(int id)
        {
            return dynamicObjects[id];
        }

        public void AddDynamicObject(DynamicObject dynamicObject)
        {
            if (dynamicObjects.Contains(dynamicObject))
                return;
            dynamicObjects.Add(dynamicObject);
        }


        public bool TryGetDynamicObjectByAssetType(System.Type assetType, out DynamicObject result)
        {
            result = dynamicObjects.Find(d => d.Asset.GetType() == assetType);
            return result != null ? true : false;
        }
    }


}

