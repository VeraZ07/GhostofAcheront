using GOA.Assets;
using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOA.Level;

namespace GOA.Level
{
    public partial class LevelBuilder
    {
        [System.Serializable]
        public class BullObjectGroup : ObjectGroup
        {

            CustomObjectAsset bullAsset;

            [SerializeField]
            int bullId;

            public BullObjectGroup(LevelBuilder builder, ObjectGroupAsset asset, int sectorId) : base(builder, asset, sectorId) { }

            public override void Build()
            {
                // Set bull asset
                bullAsset = Asset.Assets[0];

                // Get all free tiles
                List<int> tileIds = Builder.GetSector(SectorId).TileIds;
                List<int> freeTileIds = new List<int>();
                foreach (int tileId in tileIds)
                {
                    LevelBuilder.Tile tile = Builder.GetTile(tileId);
                    if (tile.unreachable || !Builder.TileIsFree(tileId))
                        continue;

                    freeTileIds.Add(tileId);
                }
                int tId = freeTileIds[Random.Range(0, freeTileIds.Count)];
                // Create a new dynamic object
                Builder.AddDynamicObject(new DynamicObject(bullAsset, tId));
                // Store the id
                bullId = Builder.DynamicObjectCount - 1;
            }

            public override void CreateSceneObjects()
            {

            }

            protected override void SpawnNetworkedObjectsImpl()
            {
                // Get the 
                LevelBuilder.DynamicObject bull = Builder.GetDynamicObjectById(bullId);
                Vector3 spawnPoint = Builder.GetTile(bull.TileId).GetPosition() + 2f * (Vector3.right + Vector3.back);
                SessionManager.Instance.Runner.Spawn((bull.Asset as CustomObjectAsset).Prefab, spawnPoint, Quaternion.identity, null);
            }
        }
    }

    

}


