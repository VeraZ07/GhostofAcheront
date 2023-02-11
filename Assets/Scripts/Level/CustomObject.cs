using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{
    public partial class LevelBuilder : MonoBehaviour
    {
        [System.Serializable]
        class CustomObject
        {

            LevelBuilder builder;

            public GameObject sceneObject;


            public Vector3 direction;

            public int tileId;

            //protected string codePrefix;

            //public string style = "000";
            public CustomObjectAsset asset;

            public CustomObject(LevelBuilder builder, CustomObjectAsset asset)
            {
                this.builder = builder;
                this.asset = asset;
            }

            public void AttachRandomly(int sectorId)
            {
                List<int> tileIds = new List<int>(builder.sectors[sectorId].tileIds);
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


                    Tile tile = builder.tiles[id];
                    if (tile.unreachable)
                        continue;
                        
                    if (builder.customObjects.Exists(c => c.tileId == id))
                        continue;

                    List<Vector3> dirs = new List<Vector3>();
                    dirs.Add(Vector3.forward);
                    dirs.Add(Vector3.right);
                    dirs.Add(Vector3.back);
                    dirs.Add(Vector3.left);
                    int dirCount = dirs.Count;
                    for(int j=0; j<dirCount && !found; j++)
                    {
                        Vector3 dir = dirs[Random.Range(0, dirs.Count)];
                        dirs.Remove(dir);
                        if (tile.openDirection == dir)
                            continue;
                        if(((tile.isUpperBorder || tile.isUpperBoundary) && dir == Vector3.forward) ||
                           ((tile.isRightBorder || tile.isRightBoundary) && dir == Vector3.right) ||
                           ((tile.isBottomBorder || tile.isBottomBoundary) && dir == Vector3.back) ||
                           ((tile.isLeftBorder || tile.isLeftBoundary) && dir == Vector3.left))
                        {
                            found = true;
                            direction = dir;
                            continue;
                        }
                        if((tile.roteableWall == 0 && dir == Vector3.right) ||
                           (tile.roteableWall == 3 && dir == Vector3.back))
                        {
                            found = true;
                            direction = dir;
                            continue;
                        }

                        int tmpId = id - size;
                        if (tmpId >= 0)//  builder.sectors[sectorId].tileIds.Contains(tmpId))
                        {
                            Tile tmpTile = builder.tiles[tmpId];
                            if((tmpTile.roteableWall == 3 && dir == Vector3.forward) ||
                               (tmpTile.roteableWall == 2 && dir == Vector3.right))
                            {
                                found = true;
                                direction = dir;
                                continue;
                            }
                        }
                        tmpId = id - 1;
                        if(tmpId % size < size - 1)// && builder.sectors[sectorId].tileIds.Contains(tmpId))
                        {
                            Tile tmpTile = builder.tiles[tmpId];
                            if ((tmpTile.roteableWall == 0 && dir == Vector3.left) ||
                                (tmpTile.roteableWall == 1 && dir == Vector3.back))
                            {
                                found = true;
                                direction = dir;
                                continue;
                            }
                        }
                    }
                }

                tileId = id;
                this.direction = direction;

            }

           
           
        }

        [System.Serializable]
        class Gate : CustomObject
        {
            public int puzzleIndex;

            public Gate(LevelBuilder builder, CustomObjectAsset asset) : base(builder, asset)
            {
                
            }
        }

       
    }
}

