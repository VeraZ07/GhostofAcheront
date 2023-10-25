using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{
    public partial class LevelBuilder : MonoBehaviour
    {
        public enum TileType { TopLeft, TopCenter, TopRight, Left, Center, Right, BottomLeft, BottomCenter, BottomRight }


        [System.Serializable]
        public class Tile
        {
            public const float Size = 4f;
            public const float Height = 4f;

            LevelBuilder builder;


            public int sectorIndex;

            /// <summary>
            /// 0: default rot
            /// 1: 90 degrees
            /// 2: 180 degrees
            /// 3: 270 degrees
            /// -1: no wall
            /// </summary>
            public float roteableWall = -1;

            /// <summary>
            /// This means that the tile has walls in some directions.
            /// Having a fixed wall doesn't mean the tile is a border of its sector ( for example rooms have fixed walls )
            /// </summary>
            public bool isUpperBorder = false;
            public bool isLeftBorder = false;
            public bool isRightBorder = false;
            public bool isBottomBorder = false;

            /// <summary>
            /// This means the tile is a border for its sector ( a boundary with another sector )
            /// </summary>
            public bool isUpperBoundary = false;
            public bool isLeftBoundary = false;
            public bool isRightBoundary = false;
            public bool isBottomBoundary = false;

            /// <summary>
            /// Is it a regular tile or a room tile?
            /// </summary>            
            public bool isRoomTile = false;

            public Vector3 openDirection = Vector3.zero;

            public bool unreachable = false;


            public TileAsset asset;

            public GameObject sceneObject;

            public Tile(LevelBuilder builder)
            {
                this.builder = builder;
            }

           

            public bool IsBoundary()
            {
                return isBottomBoundary || isUpperBoundary || isRightBoundary || isLeftBoundary;
            }

            public bool IsBorder()
            {
                return isUpperBorder || isRightBorder || isLeftBorder || isBottomBorder;
            }

            public Vector3 GetPosition()
            {
                int index = new List<Tile>(builder.tiles).IndexOf(this);

                int levelSize = (int)Mathf.Sqrt(builder.tiles.Length);

                int col = index % levelSize;
                int row = index / levelSize;

                Vector3 pos = Vector3.zero;
                pos.x = col * Size;
                pos.z = -row * Size;

                return pos;
            }

            public void CheckForPillars()
            {
                int index = new List<Tile>(builder.tiles).FindIndex(t=>t == this);
                int size = (int)Mathf.Sqrt(builder.tiles.Length);

                GameObject obj = sceneObject;

                // Room tiles only
                if (isRoomTile)
                {
                    
                    if((isUpperBorder && isLeftBorder) || (!isUpperBorder && !isLeftBorder))
                    {
                        Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ul_pillar".Equals(t.name.ToLower()));
                        if (t)
                        {
                            DestroyImmediate(t.gameObject);
                        }
                    }
                    if ((isUpperBorder && isRightBorder) || (!isUpperBorder && !isRightBorder))
                    {
                        Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ur_pillar".Equals(t.name.ToLower()));
                        if (t)
                        {
                            DestroyImmediate(t.gameObject);
                        }
                    }
                  
                    if ((isBottomBorder && isRightBorder) || (!isBottomBorder && !isRightBorder))
                    {
                        Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "br_pillar".Equals(t.name.ToLower()));
                        if (t)
                        {
                            DestroyImmediate(t.gameObject);
                        }
                    }

                   
                    if ((isBottomBorder && isLeftBorder) || (!isBottomBorder && !isLeftBorder))
                    {
                        Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "bl_pillar".Equals(t.name.ToLower()));
                        if (t)
                        {
                            DestroyImmediate(t.gameObject);
                        }
                    }

                    return;
                }
              
                
                if (isUpperBorder && isLeftBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ul_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                        
                    }
                }
                if (isUpperBorder && isRightBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ur_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                        
                    }
                }
                if (isBottomBorder && isRightBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "br_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                        
                    }
                }
                if (isBottomBorder && isLeftBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "bl_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                        
                    }
                }

                int top = index - size >= 0 ? index - size : -1;
                int right = index % size < size - 1 ? index + 1 : -1;
                int bottom = index + size < builder.tiles.Length ? index + size : -1;
                int left = index % size > 0 ? index - 1 : -1;

                // Upper left
                if(top != -1 && left != -1 && builder.tiles[top].roteableWall == 3 && builder.tiles[top-1].roteableWall == 2)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ul_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                       
                    }
                }

                if (top != -1 && left != -1 && builder.tiles[top].roteableWall == 3 && builder.tiles[left].roteableWall == 0)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ul_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (top != -1 && left != -1 && builder.tiles[left].isRightBorder && ( builder.tiles[top-1].roteableWall == 1 || builder.tiles[top].roteableWall == 3))
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ul_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                       
                    }
                }

                if (top != -1 && builder.tiles[top].roteableWall == 3 && builder.tiles[index].isLeftBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ul_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (top != -1 && left != -1 && builder.tiles[top-1].roteableWall == 1 && builder.tiles[left].roteableWall == 0)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ul_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (left != -1 && builder.tiles[left].roteableWall == 0 && builder.tiles[index].isUpperBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ul_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (left != -1 && builder.tiles[left].isRightBorder && builder.tiles[index].isUpperBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ul_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (top != -1 && builder.tiles[top].isBottomBorder && builder.tiles[index].isLeftBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ul_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (top != -1 && left != -1 && builder.tiles[top].isBottomBorder && builder.tiles[left].roteableWall == 0)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ul_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (top != -1 && left != -1 && builder.tiles[left].isRightBorder && builder.tiles[top].isBottomBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ul_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                // Upper right
                if (top != -1 && builder.tiles[top].roteableWall == 3 && builder.tiles[index].roteableWall == 0)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ur_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                       
                    }
                }

                if (top != -1 && left != -1 && builder.tiles[top-1].roteableWall == 1 && builder.tiles[index].roteableWall == 0)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ur_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                       
                    }
                }

                if (top != -1 && left != -1 && builder.tiles[top - 1].roteableWall == 1 && builder.tiles[top].roteableWall == 2)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ur_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                      
                    }
                }

                if(top != -1 && right != -1 && builder.tiles[right].isLeftBorder && builder.tiles[top].roteableWall == 3)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ur_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                      
                    }
                }

                if (top != -1 && left != -1 && right != -1 && builder.tiles[right].isLeftBorder && builder.tiles[top-1].roteableWall == 1)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ur_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                       
                    }
                }

                if (top != -1 && left != -1 && builder.tiles[top - 1].roteableWall == 1 && builder.tiles[index].isRightBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ur_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (builder.tiles[index].roteableWall == 0 && builder.tiles[index].isUpperBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ur_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (builder.tiles[index].roteableWall == 0 && builder.tiles[index].isUpperBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ur_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (top != -1 && builder.tiles[top].isBottomBorder && builder.tiles[index].roteableWall == 0)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ur_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

               

                if (top != -1 && builder.tiles[top].isBottomBorder && builder.tiles[index].isRightBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ur_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (top != -1 && right != -1 && builder.tiles[right].isLeftBorder && builder.tiles[top].isBottomBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "ur_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                // Bottom right
                if (top != -1 && builder.tiles[top].roteableWall == 2 && builder.tiles[index].roteableWall == 3)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "br_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                      
                    }
                }

                if (top != -1 && left != -1 && builder.tiles[top].roteableWall == 2 && builder.tiles[left].roteableWall == 1)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "br_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                      
                    }
                }

                if (left != -1 && builder.tiles[left].roteableWall == 1 && builder.tiles[index].roteableWall == 0)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "br_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                      
                    }
                }

                if (right != -1 && builder.tiles[right].isLeftBorder && builder.tiles[index].roteableWall == 3)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "br_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                       
                    }
                }

                if (right != -1 && builder.tiles[right].isLeftBorder && builder.tiles[index].isUpperBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "br_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (left != -1 && right != -1 && builder.tiles[right].isLeftBorder && builder.tiles[index].roteableWall == 3 && builder.tiles[left].roteableWall == 1)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "br_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                       
                    }
                }



                if (left != -1 && builder.tiles[index].isRightBorder && builder.tiles[left].roteableWall == 1)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "br_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (bottom != -1 && top != -1 && builder.tiles[top].roteableWall == 2 && builder.tiles[bottom].isUpperBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "br_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                    }
                }

                if (bottom != -1 && builder.tiles[index].isRightBorder && builder.tiles[bottom].isUpperBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "br_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                    }
                }

                if (right != -1 && bottom != -1 && builder.tiles[bottom].isUpperBorder && builder.tiles[right].isLeftBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "br_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                    }
                }

                if (left != -1 && right != -1 && builder.tiles[left].roteableWall == 1 && builder.tiles[right].isLeftBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "br_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                    }
                }

                if (top != -1 && builder.tiles[top].roteableWall == 2 && builder.tiles[index].isBottomBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "br_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                    }
                }

                // Bottom left
                if (left != -1 && builder.tiles[left].roteableWall == 0 && builder.tiles[index].roteableWall == 3)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "bl_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                       
                    }
                }

                if (left != -1 && top != -1 && builder.tiles[top-1].roteableWall == 2 && builder.tiles[left].roteableWall == 1)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "bl_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                       
                    }
                }

                if (left != -1 && top != -1 && builder.tiles[top-1].roteableWall == 2 && builder.tiles[index].roteableWall == 3)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "bl_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                      
                    }
                }

                if (left != -1 && builder.tiles[left].isRightBorder && builder.tiles[index].roteableWall == 3)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "bl_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);
                      
                    }
                }

                if (builder.tiles[index].isLeftBorder && builder.tiles[index].roteableWall == 3)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "bl_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (bottom != -1 && left != -1 && top != -1 && builder.tiles[top - 1].roteableWall == 2 && builder.tiles[bottom].isUpperBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "bl_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

                if (left != -1 && top != -1 && builder.tiles[top - 1].roteableWall == 2 && builder.tiles[index].isBottomBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "bl_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }

          
                if (bottom != -1 && builder.tiles[bottom].isUpperBorder && builder.tiles[index].isLeftBorder)
                {
                    Transform t = new List<Transform>(obj.GetComponentsInChildren<Transform>()).Find(t => "bl_pillar".Equals(t.name.ToLower()));
                    if (t)
                    {
                        DestroyImmediate(t.gameObject);

                    }
                }
            }


            public override string ToString()
            {
                return string.Format(" {0}:{1} ", sectorIndex, asset.name);
            }

           
        }

        public bool TileIsFree(int tileId)
        {
            return !(customObjects.Exists(c => c.TileId == tileId) || dynamicObjects.Exists(d => d.TileId == tileId));
        }
    }

}
