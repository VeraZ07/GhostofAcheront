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
                if (asset.GetType() == typeof(MemoryPuzzleAsset))
                {
                    return new MemoryPuzzle(builder, asset, sectorIndex);
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
                            obj.GetComponent<PuzzleController>().Initialize(builder.puzzles.IndexOf(this));
                       });
            }
        }

        public class MemoryPuzzle : Puzzle
        {
            
          
            List<int> frameIds = new List<int>();
            public IList<int> FrameIds
            {
                get { return frameIds.AsReadOnly(); }
            }

            public int TilesCount
            {
                get { return int.Parse((Asset as MemoryPuzzleAsset).Frame.name.Substring((Asset as MemoryPuzzleAsset).Frame.name.LastIndexOf("_") + 1)); }
            }

            public MemoryPuzzle(LevelBuilder builder, PuzzleAsset asset, int sectorId) : base(builder, asset, sectorId)
            {
                // Add custom objects to the builder
                MemoryPuzzleAsset mpa = asset as MemoryPuzzleAsset;
                for(int i=0; i<mpa.FramesCount; i++)
                {
                    
                    // Create the frame custom object
                    CustomObject co = new CustomObject(builder, mpa.Frame);
                    builder.customObjects.Add(co);
                    frameIds.Add(builder.CustomObjects.Count - 1);
                    co.AttachRandomly(sectorId);

                    
                }

            }

            public override void CreateSceneObjects()
            {
                foreach(int frameId in frameIds)
                {
                    builder.CustomObjects[frameId].CreateSceneObject();

                    // Get the scene object
                    GameObject sceneObject = builder.customObjects[frameId].SceneObject;

                    // Get the tile container
                    Transform tileGroup = sceneObject.transform.GetChild(0).GetChild(0);
                    
                    // Rearrange tiles
                    int numOfTiles = tileGroup.childCount;
                    List<int> indices = new List<int>();
                    List<Vector3> positions = new List<Vector3>();
                    List<Quaternion> rotations = new List<Quaternion>();

                    for (int j = 0; j < numOfTiles; j++)
                    {
                        indices.Add(j);
                        positions.Add(tileGroup.GetChild(j).position);
                        rotations.Add(tileGroup.GetChild(j).rotation);
                    }
                    // Rearranging
                    for (int j = 0; j < numOfTiles; j++)
                    {
                        
                        // Get the new position you want to move the tile at index j
                        int newIndex = indices[Random.Range(0, indices.Count)];
                        indices.Remove(newIndex);
                        tileGroup.GetChild(j).position = positions[newIndex];
                        tileGroup.GetChild(j).rotation = rotations[newIndex];
                    }
                }
                
            }

            
        }

        public class HandlesPuzzle: Puzzle
        {
            public class Handle
            {
                int customObjectId;
                public int CustomObjectId
                {
                    get { return customObjectId; }
                }
                int initialState;
                public int InitialState
                {
                    get { return initialState; }
                }
                int finalState;
                public int FinalState
                {
                    get { return finalState; }
                }

                int stateCount;
                public int StateCount
                {
                    get { return stateCount; }
                }
                public Handle(int id, int initialState, int finalState, int stateCount)
                {
                    this.customObjectId = id;
                    this.initialState = initialState;
                    this.finalState = finalState;
                    this.stateCount = stateCount;
                }
            }
            

            List<Handle> handles = new List<Handle>();
            public IList<Handle> Handles
            {
                get { return handles.AsReadOnly(); }
            }

            bool stopHandleOnFinalState = false;
            public bool StopHandleOnFinalState
            {
                get { return stopHandleOnFinalState; }
            }

            Handle clueHandle = null;
            public Handle ClueHandle
            {
                get { return clueHandle; }
            }

            bool useClueHandle = false;
            public bool UseClueHandle
            {
                get { return useClueHandle; }
            }

            public HandlesPuzzle(LevelBuilder builder, PuzzleAsset asset, int sectorId): base(builder, asset, sectorId)
            {

                HandlesPuzzleAsset hpa = asset as HandlesPuzzleAsset;
                int elementCount = hpa.Handles.Count;
                stopHandleOnFinalState = hpa.StopHandleOnFinalState;

                // We compute a unique final state if required
                int commonFinalState = hpa.UseClueHandle ? Random.Range(0, hpa.Handles[0].StateCount) : -1;

                useClueHandle = false;
                // Create the clue handle if required
                if (hpa.UseClueHandle)
                {
                    useClueHandle = true;

                    // Create the custom objecT
                    CustomObject co = new CustomObject(builder, hpa.ClueHandle.Asset);
                    builder.customObjects.Add(co);

                    clueHandle = new Handle(builder.customObjects.Count - 1, commonFinalState, commonFinalState, 1);

                    co.AttachRandomly(sectorId);
                }

                for (int i = 0; i < elementCount; i++)
                {
                    // Create custom objects
                    CustomObject co = new CustomObject(builder, hpa.Handles[i].Asset);

                    builder.customObjects.Add(co);

                    int finalState = 0;
                    
                    // Check if we need the same final state for every handle
                    if (hpa.UseClueHandle)
                    {
                        finalState = commonFinalState;
                    }
                    else
                    {
                        finalState = hpa.Handles[i].FinalState < 0 ? Random.Range(0, hpa.Handles[i].StateCount) : hpa.Handles[i].FinalState;
                    }

                    List<int> candidates = new List<int>();
                    for (int j = 0; j < hpa.Handles[i].StateCount - 1; j++)
                    {
                        if (j != finalState)
                            candidates.Add(j);
                    }

                    // Add the new index in the internal list
                    //int initialState = hpa.Handles[i].InitialState < 0 ? Random.Range(0, hpa.Handles[i].StateCount) : hpa.Handles[i].InitialState;
                    int initialState = hpa.Handles[i].InitialState < 0 ? candidates[Random.Range(0, candidates.Count)] : hpa.Handles[i].InitialState;

                    // Create a new handle
                    Handle h = new Handle(builder.customObjects.Count - 1, initialState, finalState, hpa.Handles[i].StateCount);

                    handles.Add(h);

                    // Attach to a random tile
                    co.AttachRandomly(sectorId);
                }

                

            }

            public override void CreateSceneObjects()
            {
                // Create the clue handle scene object if required
                if (useClueHandle)
                {
                    builder.customObjects[clueHandle.CustomObjectId].CreateSceneObject();
                }

                // Loop through all the ids
                for(int i=0; i<handles.Count; i++)
                {
                    builder.customObjects[handles[i].CustomObjectId].CreateSceneObject();
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
