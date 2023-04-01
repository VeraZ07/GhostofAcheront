using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{
    public partial class LevelBuilder : MonoBehaviour
    {
        [System.Serializable]
        public class CustomObject
        {
            [SerializeField]
            GameObject sceneObject;
            public GameObject SceneObject
            {
                get { return sceneObject; }
            }

            [SerializeField]
            Vector3 direction;
            public Vector3 Direction
            {
                get { return direction; }
                set { direction = value; }
            }

            [SerializeField]
            int tileId;
            public int TileId
            {
                get { return tileId; }
                set { tileId = value; }
            }

            LevelBuilder builder;

            [SerializeField]
            CustomObjectAsset asset;

            public CustomObject(LevelBuilder builder, CustomObjectAsset asset)
            {
                this.builder = builder;
                this.asset = asset;
            }

            public void CreateSceneObject()
            {
                sceneObject = Instantiate(asset.Prefab, builder.GeometryRoot);
                sceneObject.transform.position = builder.tiles[tileId].GetPosition();
                if(direction != Vector3.zero)
                    sceneObject.transform.GetChild(0).transform.forward = direction;

            }

            public void AttachRandomly(int sector, ObjectAlignment forceAlignment = ObjectAlignment.Both, List<int> exclusionList = null)
            {
                if(forceAlignment == ObjectAlignment.MiddleOnly || forceAlignment == ObjectAlignment.SideOnly)
                {
                    if(forceAlignment == ObjectAlignment.MiddleOnly)
                        AttachRandomly(sector, true, exclusionList);
                    else
                        AttachRandomly(sector, false, exclusionList);
                }
                else
                {
                    switch (asset.Alignment)
                    {
                        case ObjectAlignment.Both:
                            AttachRandomly(sector, Random.Range(0, 2) == 0 ? true : false, exclusionList);
                            break;
                        case ObjectAlignment.MiddleOnly:
                            AttachRandomly(sector, true, exclusionList);
                            break;
                        case ObjectAlignment.SideOnly:
                            AttachRandomly(sector, false, exclusionList);
                            break;
                    }
                }

                
            }

       

            void AttachRandomly(int sectorId, bool inTheMiddle, List<int> exclusionList = null)
            {
                if ((asset.Alignment == ObjectAlignment.SideOnly && inTheMiddle) || (asset.Alignment == ObjectAlignment.MiddleOnly && !inTheMiddle))
                    throw new System.Exception(string.Format("CustomObject.AttachRandomly() - {0} alignement failed ({1})", asset.name, inTheMiddle));

                List<int> tileIds = new List<int>(builder.sectors[sectorId].tileIds);

                // Remove excluded tiles
                if (exclusionList == null)
                    exclusionList = new List<int>();

                foreach (int excludedId in exclusionList)
                    tileIds.Remove(excludedId);

                int count = tileIds.Count;

                int size = (int)Mathf.Sqrt(builder.tiles.Length);

                bool found = false;
                Vector3 direction = Vector3.zero;
                int id = -1;

                for(int i=0; i<count && !found; i++)
                {
                    // Get a random id
                    id = tileIds[Random.Range(0, tileIds.Count)];
                    tileIds.Remove(id);

                    //if (exclusionList != null && exclusionList.Contains(id))
                    //    continue;

                    Tile tile = builder.tiles[id];
                    if (tile.unreachable)
                        continue;


                    // If we want to add a new object in the middle of the current tile we just need to check if there is already
                    // another object in the same position
                    if (inTheMiddle)
                    {
                        if (builder.customObjects.Exists(c => c.tileId == id && c.direction == Vector3.zero))
                            continue;

                        found = true;
                        direction = Vector3.zero;
                        continue;
                    }
                    else // Not in the middle
                    {
                        List<Vector3> dirs = new List<Vector3>();

                        // Check upper direction
                        bool topClosedByOther = (id - size >= 0) && (builder.tiles[id - size].roteableWall == 3);
                        bool rightClosedByOther = (id - size >= 0) && (builder.tiles[id - size].roteableWall == 2);
                        bool bottomClosedByOther = (id-1 >= 0) && ((id - 1) % size < size - 1) && (builder.tiles[id - 1].roteableWall == 1);
                        bool leftClosedByOther = (id - 1 >= 0) && ((id - 1) % size < size - 1) && (builder.tiles[id - 1].roteableWall == 0);
                        if ((!builder.customObjects.Exists(c => c.tileId == id && c.direction == Vector3.forward)) &&
                           (tile.isUpperBorder || tile.isUpperBoundary || topClosedByOther) && tile.openDirection != Vector3.forward)
                        {
                            dirs.Add(Vector3.forward);
                        }
                        if ((!builder.customObjects.Exists(c => c.tileId == id && c.direction == Vector3.right)) &&
                           (tile.isRightBorder || tile.isRightBoundary || rightClosedByOther) && tile.openDirection != Vector3.right)
                        {
                            dirs.Add(Vector3.right);
                        }
                        if ((!builder.customObjects.Exists(c => c.tileId == id && c.direction == Vector3.back)) &&
                           (tile.isBottomBorder || tile.isBottomBoundary || bottomClosedByOther) && tile.openDirection != Vector3.back)
                        {
                            dirs.Add(Vector3.back);
                        }
                        if ((!builder.customObjects.Exists(c => c.tileId == id && c.direction == Vector3.left)) &&
                           (tile.isLeftBorder || tile.isLeftBoundary || leftClosedByOther) && tile.openDirection != Vector3.left)
                        {
                            dirs.Add(Vector3.left);
                        }

                        if (dirs.Count == 0)
                            continue;

                        // Get a random direction
                        found = true;
                        direction = dirs[Random.Range(0, dirs.Count)]; 

                      
                  
                    }

                    
                   
                    
                }

                tileId = id;
                this.direction = direction;

            }

           
           
        }

        [System.Serializable]
        public class Gate : CustomObject
        {
            [SerializeField]
            int puzzleIndex;
            public int PuzzleIndex
            {
                get { return puzzleIndex; }
                set { puzzleIndex = value; }
            }

            public Gate(LevelBuilder builder, CustomObjectAsset asset) : base(builder, asset)
            {
                
            }
        }

       
    }
}

