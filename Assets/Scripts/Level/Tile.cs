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
        class Tile
        {
            public const float Size = 4;

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

            public override string ToString()
            {
                return string.Format(" {0}:{1} ", sectorIndex, asset.name);
            }

        }
    }

}
