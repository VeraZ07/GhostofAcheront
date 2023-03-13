using Fusion;
using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{
    public partial class LevelBuilder : MonoBehaviour
    {
      
        static class PuzzleFactory
        {
            public static Puzzle CreatePuzzle(LevelBuilder builder, PuzzleAsset asset, int sectorIndex)
            {
                if(asset.GetType() == typeof(MultiStatePuzzleAsset))
                {
                    return new MultiStatePuzzle(builder, asset, sectorIndex);
                }
                if (asset.GetType() == typeof(PicturePuzzleAsset))
                {
                    return new PicturePuzzle(builder, asset, sectorIndex);
                }
                //return puzzle;
                throw new System.Exception(string.Format("PuzzleFactory - Puzzle '{0}' not found.", asset.name));
            }
        }

        [System.Serializable]
        public abstract class Puzzle
        {
            [SerializeField]
            PuzzleAsset asset;
            public PuzzleAsset Asset
            {
                get { return asset; }
            }

            int sectorId;


            protected LevelBuilder builder;

            public Puzzle(LevelBuilder builder, PuzzleAsset asset, int sectorId)
            {
                this.asset = asset;
                this.sectorId = sectorId;
                this.builder = builder;
            }

            public abstract void CreateSceneObjects();

            public abstract void SpawnInteractables();
        }

        public class MultiStatePuzzle: Puzzle
        {
            
            List<int> elementIds = new List<int>();
            public ICollection<int> ElementsIds
            {
                get { return elementIds.AsReadOnly(); }
            }

            public MultiStatePuzzle(LevelBuilder builder, PuzzleAsset asset, int sectorId): base(builder, asset, sectorId)
            {
                
                int elementCount = Random.Range(1, 4); // Should depend by the number of players


                for (int i = 0; i < elementCount; i++)
                {
                    // Create a new custom object to hold the handle ( or whatever it is )
                    CustomObject co = new CustomObject(builder, ((MultiStatePuzzleAsset)asset).ElementAsset);
                    builder.customObjects.Add(co);
                    // Add the new index in the internal list
                    elementIds.Add(builder.customObjects.Count - 1);

                    // Attach to a random tile
                    co.AttachRandomly(sectorId);


                }
            }

            public override void CreateSceneObjects()
            {
                // Loop through all the ids
                for(int i=0; i<elementIds.Count; i++)
                {
                    builder.customObjects[elementIds[i]].CreateSceneObject();
                }
            }

            public override void SpawnInteractables()
            {
                throw new System.NotImplementedException();
            }
        }


        public class PicturePuzzle : Puzzle
        {
            int pictureId;
            public int PictureId
            {
                get { return pictureId; }
            }

            List<int> pieceIds;
            public IList<int> PieceIds
            {
                get { return pieceIds.AsReadOnly(); }
            }

            public PicturePuzzle(LevelBuilder builder, PuzzleAsset asset, int sectorId) : base(builder, asset, sectorId)
            {
                // Create the picture device
                CustomObject co = new CustomObject(builder, (asset as PicturePuzzleAsset).Picture);
                // Add the new object to the object list
                builder.customObjects.Add(co);
                // Set the corresponding id
                pictureId = builder.customObjects.Count - 1;
                // Choose a free tile 
                co.AttachRandomly(sectorId);

                // Create all the pieces
                pieceIds = new List<int>();
                List<CustomObjectAsset> pList = new List<CustomObjectAsset>((asset as PicturePuzzleAsset).Pieces);
                foreach(CustomObjectAsset coa in pList)
                {
                    // Create a new custom object
                    co = new CustomObject(builder, coa);
                    // Add the new object to the object list
                    builder.customObjects.Add(co);
                    // Set the corresponding id
                    pieceIds.Add(builder.customObjects.Count - 1);
                    // Choose a free tile 
                    co.AttachRandomly(sectorId);
                }
            }

            public override void CreateSceneObjects()
            {
                builder.customObjects[pictureId].CreateSceneObject();
                foreach(int id in pieceIds)
                {
                    //// Create pickers
                    //Tile sTile = tiles[startingTileId];
                    //Vector3 pos = sTile.sceneObject.transform.position;
                    //NetworkObject no = SessionManager.Instance.Runner.Spawn(pickerPrefab, pos, Quaternion.identity, null,
                    //(r, o) =>
                    //{
                    //    o.GetComponent<Picker>().Init("Pic1TL", false);
                    //});


                    builder.customObjects[id].CreateSceneObject();
                }
            }

            public override void SpawnInteractables()
            {
                if (!SessionManager.Instance.Runner.IsServer)
                    return;

                // Spawn pickers
                for(int i=0; i<pieceIds.Count; i++)
                //foreach (int id in pieceIds)
                {
                    int id = pieceIds[i];
                    CustomObject co = builder.CustomObjects[id];
                    Tile tile = builder.tiles[co.TileId];
                    Vector3 pos = tile.GetPosition();
                    NetworkObject no = SessionManager.Instance.Runner.Spawn((Asset as PicturePuzzleAsset).PickerPrefab, pos, Quaternion.identity, null,
                    (r, o) =>
                    {
                        o.GetComponent<Picker>().Init(id, (Asset as PicturePuzzleAsset).Items[i].name, false);
                    });
                }
            }

        }

    }

}
