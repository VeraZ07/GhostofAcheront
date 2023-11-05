using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{
    public partial class LevelBuilder
    {
        [System.Serializable]
        public abstract class ObjectGroup
        {
            [SerializeField]
            ObjectGroupAsset asset;
            public ObjectGroupAsset Asset
            {
                get { return asset; }
            }
            [SerializeField]
            int sectorId;
            public int SectorId
            {
                get { return sectorId; }
            }
            LevelBuilder builder;
            public LevelBuilder Builder
            {
                get { return builder; }
            }

            public abstract void Build();

            public abstract void CreateSceneObjects();

            protected abstract void SpawnNetworkedObjectsImpl();

            public ObjectGroup(LevelBuilder builder, ObjectGroupAsset asset, int sectorId)
            {
                this.asset = asset;
                this.sectorId = sectorId;
                this.builder = builder;
            }

            

            public void SpawnNetworkedObjects()
            {
                if (SessionManager.Instance.Runner.IsSinglePlayer || SessionManager.Instance.Runner.IsSharedModeMasterClient)
                    SpawnNetworkedObjectsImpl();
            }
        }

        [SerializeReference]
        List<ObjectGroup> objectGroups = new List<ObjectGroup>();

        public void AddObjectGroup(ObjectGroup objectGroup)
        {
            objectGroups.Add(objectGroup);
        }
    }

}
