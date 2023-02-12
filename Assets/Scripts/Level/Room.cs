using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{
    public partial class LevelBuilder : MonoBehaviour
    {

        [System.Serializable]
        class Room
        {
            public List<int> tileIds = new List<int>();

            public Room(LevelBuilder builder, int sectorId, int width, int height)
            {
                this.sectorId = sectorId;
                this.width = width;
                this.height = height;
                this.builder = builder;
            }

            public int width;
            public int height;
            public int sectorId;

            LevelBuilder builder;

            public void Create()
            {
                int size = (int)Mathf.Sqrt(builder.tiles.Length);

                List<TileAsset> assets = new List<TileAsset>(Resources.LoadAll<TileAsset>(System.IO.Path.Combine(TileAsset.ResourceFolder, builder.theme.ToString()))).FindAll(t => t.name.ToLower().StartsWith("roomtile_"));

                for (int i = 0; i < tileIds.Count; i++)
                {
                    int id = tileIds[i];
                    Tile tile = builder.tiles[id];

                    // Set the asset
                    tile.asset = assets[Random.Range(0, assets.Count)];

                    tile.roteableWall = -1;
                    tile.isRoomTile = true;

                    // Set the type of tile
                    if (i == 0)
                    {
                        tile.isUpperBorder = tile.isLeftBorder = true;
                    }
                    else if (i == width - 1)
                    {
                        tile.isUpperBorder = tile.isRightBorder = true;
                    }
                    else if (i == width * (height - 1))
                    {
                        tile.isBottomBorder = tile.isLeftBorder = true;
                    }
                    else if (i == width * (height - 1) + width - 1)
                    {
                        tile.isBottomBorder = tile.isRightBorder = true;
                    }
                    else if (i % width == 0)
                    {
                        tile.isLeftBorder = true;
                    }
                    else if (i % width == width - 1)
                    {
                        tile.isRightBorder = true;
                    }
                    else if (i / width == 0)
                    {
                        tile.isUpperBorder = true;
                    }
                    else if (i / width == height - 1)
                    {
                        tile.isBottomBorder = true;
                    }
                    //else
                    //    tile.type = (int)TileType.Center;

                    // Check adjacent tiles
                    if (i % width == 0 && builder.sectors[sectorId].tileIds.Contains(id - 1))
                        builder.tiles[id - 1].roteableWall = -1;
                    if (i / width == 0 && builder.sectors[sectorId].tileIds.Contains(id - size))
                        builder.tiles[id - size].roteableWall = -1;
                    if (i / width == 0 && i % width == 0 && builder.sectors[sectorId].tileIds.Contains(id - size - 1))
                        builder.tiles[id - size - 1].roteableWall = -1;
                }

                // Check for entrances
                // At this point we can only have opening to another sector ( or from the initial or to the final room ), so we
                // must at least add a new opening to the sector the room belongs to
                //int openingCount = tileIds.FindAll(t => builder.tiles[t].openDirection != Vector3.zero).Count;
                // Get all the possible tiles for a new opening
                List<int> candidates = tileIds.FindAll(id => !builder.tiles[id].IsBoundary() && builder.tiles[id].IsBorder());
                candidates.RemoveAll(id => builder.tiles[id].openDirection != Vector3.zero);

                int openingCount = Random.Range(1, Mathf.Max(2, candidates.Count / 2));
                while (candidates.Count > 0 && openingCount > 0)
                {
                    // Create a new opening
                    // Get a random tile
                    int id = candidates[Random.Range(0, candidates.Count)];
                    candidates.Remove(id);

                    Sector sector = builder.sectors[sectorId];
                    if (sector.tileIds.Contains(id - 1) && !tileIds.Contains(id - 1)) // Open left
                    {
                        //builder.connections.Add(Connection.CreateNormalConnection(builder, id, id - 1));
                        builder.tiles[id].openDirection = Vector3.left;
                        openingCount--;
                        continue;
                    }
                    if (sector.tileIds.Contains(id + 1) && !tileIds.Contains(id + 1)) // Open right
                    {
                        //builder.connections.Add(Connection.CreateNormalConnection(builder, id, id + 1));
                        builder.tiles[id].openDirection = Vector3.right;
                        openingCount--;
                        continue;
                    }
                    if (sector.tileIds.Contains(id - size) && !tileIds.Contains(id - size)) // Open up
                    {
                        //builder.connections.Add(Connection.CreateNormalConnection(builder, id, id - size));
                        builder.tiles[id].openDirection = Vector3.forward;
                        openingCount--;
                        continue;
                    }
                    if (sector.tileIds.Contains(id + size) && !tileIds.Contains(id + size)) // Open down
                    {
                        //builder.connections.Add(Connection.CreateNormalConnection(builder, id, id + size));
                        builder.tiles[id].openDirection = Vector3.back;
                        openingCount--;
                        continue;
                    }
                }



            }
        }

    }

}
