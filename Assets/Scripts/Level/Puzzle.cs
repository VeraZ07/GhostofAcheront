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
                if(asset.GetType() == typeof(HandlesPuzzleAsset))
                {
                    return new HandlesPuzzle(builder, asset, sectorIndex);
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

            /// <summary>
            /// Creates all the objects in the scene
            /// </summary>
            public abstract void CreateSceneObjects();

            /// <summary>
            /// Spawns networked objects ( normally pickers and controllers ) and then attaches the corresponding objects.
            /// </summary>
            public virtual void SpawnNetworkedObjects()
            {
                // Spawn the puzzle controller
                SessionManager.Instance.Runner.Spawn(Asset.ControllerPrefab, Vector3.zero, Quaternion.identity, null,
                       (runner, obj) =>
                       {
                            //obj.GetComponent<PuzzleController>().PuzzleIndex = puzzles.IndexOf(puzzle);
                            obj.GetComponent<PuzzleController>().Initialize(builder.puzzles.IndexOf(this));
                       });
            }
        }

        public class HandlesPuzzle: Puzzle
        {
            
            List<int> handleIds = new List<int>();
            public ICollection<int> HandleIds
            {
                get { return handleIds.AsReadOnly(); }
            }

            public HandlesPuzzle(LevelBuilder builder, PuzzleAsset asset, int sectorId): base(builder, asset, sectorId)
            {

                HandlesPuzzleAsset hpa = asset as HandlesPuzzleAsset;
                int elementCount = hpa.Handles.Count;


                for (int i = 0; i < elementCount; i++)
                {
                    // Create custom objects
                    CustomObject co = new CustomObject(builder, hpa.Handles[i].Asset);

                    builder.customObjects.Add(co);
                    // Add the new index in the internal list
                    handleIds.Add(builder.customObjects.Count - 1);

                    // Attach to a random tile
                    co.AttachRandomly(sectorId);


                }
            }

            public override void CreateSceneObjects()
            {
                // Loop through all the ids
                for(int i=0; i<handleIds.Count; i++)
                {
                    builder.customObjects[handleIds[i]].CreateSceneObject();
                }
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

            /// <summary>
            /// This method creates all the scene objects
            /// </summary>
            public override void CreateSceneObjects()
            {
                builder.customObjects[pictureId].CreateSceneObject();
                foreach(int id in pieceIds)
                {
                    builder.customObjects[id].CreateSceneObject();
                }

                //// Init all the picture interactors
                //GameObject pictureObject = builder.CustomObjects[pictureId].SceneObject;
                //int puzzleId = builder.puzzles.IndexOf(this);
                //foreach (PieceInteractor interactor in pictureObject.GetComponentsInChildren<PieceInteractor>())
                //{
                //    interactor.Init(puzzleId);
                //}
   
            }

            public override void SpawnNetworkedObjects()
            {
                base.SpawnNetworkedObjects();

                // Spawn pickers
                
                for (int i = 0; i < pieceIds.Count; i++)
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
