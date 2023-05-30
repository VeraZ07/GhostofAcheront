using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{
    public partial class LevelBuilder : MonoBehaviour
    {
        [System.Serializable]
        public class Connection
        {


            [SerializeField]
            int sourceTileId = -1;
            public int SourceTileId
            {
                get { return sourceTileId; }
            }

            [SerializeField]
            int targetTileId = -1;
            public int TargetTileId
            {
                get { return targetTileId; }
            }

            public int gateIndex = -1;

            LevelBuilder builder;

            public static Connection CreateNormalConnection(LevelBuilder builder, int sourceTileId, int targetTileId)
            {
                Connection c = new Connection();
                c.builder = builder;
                c.sourceTileId = sourceTileId;
                c.targetTileId = targetTileId;
                c.type = 0;
                return c;
            }

            public static Connection CreateInitialConnection(LevelBuilder builder, int sourceTileId)
            {
                Connection c = new Connection();
                c.builder = builder;
                c.targetTileId = sourceTileId;
                c.type = 1;
                return c;
            }

            public static Connection CreateFinalConnection(LevelBuilder builder, int sourceTileId)
            {
                Connection c = new Connection();
                c.builder = builder;
                c.sourceTileId = sourceTileId;
                c.type = 2;
                return c;
            }

            private Connection() { }

            /// <summary>
            /// 0: tile->tile
            /// 1: enter
            /// 2: exit
            /// </summary>
            [SerializeField]
            int type = 0;



            public bool IsInitialConnection()
            {
                return type == 1;
            }
            public bool IsFinalConnection()
            {
                return type == 2;
            }
            public bool IsCommonConnection()
            {
                return type == 0;
            }
        }

        [System.Serializable]
        public class Sector
        {
            LevelBuilder builder;

            [SerializeField]
            List<int> tileIds = new List<int>();

            public List<int> TileIds
            {
                get { return tileIds; }
            }


            public int width = 0;
            public int height = 0;

            public Sector(LevelBuilder builder)
            {
                this.builder = builder;
            }

            

            public void Build()
            {
                //Sector sector = sectors[sectorIndex];
                int sectorIndex = new List<Sector>(builder.sectors).FindIndex(s => s == this);

                int size = (int)Mathf.Sqrt(builder.tiles.Length); // Columns

                List<TileAsset> assets = new List<TileAsset>(Resources.LoadAll<TileAsset>(System.IO.Path.Combine(TileAsset.ResourceFolder, builder.theme.ToString()))).FindAll(t => t.name.ToLower().StartsWith("mazetile_"));

                // Loop through each tile of the current sector
                for (int i = 0; i < tileIds.Count; i++)
                {
                    int tileId = tileIds[i];
                    int col = tileId % size;
                    int row = tileId / size;

                    // Set the asset
                    builder.tiles[tileId].asset = assets[Random.Range(0, assets.Count)];

                    // Check whether the current tile is at the edge of the sector or not
                    builder.tiles[tileId].isUpperBorder = (row == 0 || builder.tiles[tileId - size].sectorIndex != sectorIndex);
                    builder.tiles[tileId].isLeftBorder = (col == 0 || builder.tiles[tileId - 1].sectorIndex != sectorIndex);
                    builder.tiles[tileId].isRightBorder = (col == size - 1 || builder.tiles[tileId + 1].sectorIndex != sectorIndex);
                    builder.tiles[tileId].isBottomBorder = (row == size - 1 || builder.tiles[tileId + size].sectorIndex != sectorIndex);
                    builder.tiles[tileId].isUpperBoundary = builder.tiles[tileId].isUpperBorder;
                    builder.tiles[tileId].isRightBoundary = builder.tiles[tileId].isRightBorder;
                    builder.tiles[tileId].isBottomBoundary = builder.tiles[tileId].isBottomBorder;
                    builder.tiles[tileId].isLeftBoundary = builder.tiles[tileId].isLeftBorder;

                    bool rotate = false;

                    if (!builder.tiles[tileId].isUpperBorder && !builder.tiles[tileId].isLeftBorder &&
                        !builder.tiles[tileId].isRightBorder && !builder.tiles[tileId].isBottomBorder)
                    {
                        //builder.tiles[tileId].type = (int)TileType.Center;

                        rotate = true;
                    }
                    else
                    {
                        if ((builder.tiles[tileId].isUpperBorder && !builder.tiles[tileId].isRightBorder) ||
                            (builder.tiles[tileId].isLeftBorder && !builder.tiles[tileId].isBottomBorder))
                            rotate = true;

                    }

                    // Rotate walls to shape the maze
                    if (rotate)
                    {
                        // Avoid overriding
                        List<int> avoidList = new List<int>();
                        if (tileIds.Contains(tileId - 1))
                        {
                            if (builder.tiles[tileId - 1].roteableWall == 1)
                                avoidList.Add(3);
                        }
                        if (tileIds.Contains(tileId - size))
                        {
                            if (builder.tiles[tileId - size].roteableWall == 2)
                                avoidList.Add(0);
                        }

                        List<int> rot = new List<int>(new int[] { 0, 1, 2, 3 });
                        rot.RemoveAll(t => avoidList.Contains(t));


                        // Rotate 
                        int rotCode = rot[Random.Range(0, rot.Count)];
                        builder.tiles[tileId].roteableWall = rotCode;
                    }

                }
            }


            public bool IsInitialSector()
            {
                return builder.tiles[builder.connections.Find(c => c.IsInitialConnection()).TargetTileId].sectorIndex == new List<Sector>(builder.sectors).IndexOf(this);

                //return index == tiles[connections.Find(c => c.IsInitialConnection()).targetTileId].sectorIndex;
            }

            public bool IsFinalSector()
            {
                return builder.tiles[builder.connections.Find(c => c.IsFinalConnection()).SourceTileId].sectorIndex == new List<Sector>(builder.sectors).IndexOf(this);
                //return index == tiles[connections.Find(c => c.IsFinalConnection()).sourceTileId].sectorIndex;
            }


        }
    }

}
