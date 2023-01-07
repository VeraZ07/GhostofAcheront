using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOA.Assets;
using Fusion;

namespace GOA.Level
{
    public class LevelBuilder : MonoBehaviour
    {

        // From 0 to N, 0 is the smaller.
        public static int LevelSize = 0;

        

        class Tile
        {
            /// <summary>
            /// Code AABBCC
            /// AA: the position of the tile ( ex. TL for top left )
            /// BB: is the tile closed or for example is a door? Is the initial tile?
            /// CC: the type of tile ( simple wall? room? )
            /// Some codes:
            /// AA: tl, tc, tr, cl, cc, cr, bl, bc, br 
            /// BB: dd=default, cc=tile_connection, ca=start, cb=end
            /// CC: WA=, RA=room
            /// </summary>
            public string code = "AAddWA";
            public int sectorIndex;

            /// <summary>
            /// 0: default rot
            /// 1: 90 degrees
            /// 2: 180 degrees
            /// 3: 270 degrees
            /// -1: remove
            /// </summary>
            public float wall = 0;


            public override string ToString()
            {
                return string.Format(" {0}:{1} ", sectorIndex, code);
            }

        }

        [System.Serializable]
        class Connection
        {
            [SerializeField]
            public int sourceTileId = -1;
            [SerializeField]
            public int targetTileId = -1;
            
            public static Connection CreateNormalConnection(int sourceTileId, int targetTileId)
            {
                Connection c = new Connection();
                c.sourceTileId = sourceTileId;
                c.targetTileId = targetTileId;
                c.type = 0;
                return c;
            }

            public static Connection CreateInitialConnection(int sourceTileId)
            {
                Connection c = new Connection();
                c.sourceTileId = sourceTileId;
                c.type = 1;
                return c;
            }

            public static Connection CreateFinalConnection(int sourceTileId)
            {
                Connection c = new Connection();
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
        class Sector
        {
            [SerializeField]
            public List<int> tileIds = new List<int>();

            //[SerializeField]
            //public List<int> connectedSectorIds = new List<int>();

           
        }

        Tile[] tiles;

        Sector[] sectors;

        [SerializeField]
        List<Connection> connections = new List<Connection>();

        private void Awake()
        {
            
        }

        private void Start()
        {
            Create();
        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.A))
            {
                LevelSize = 0;
                Create();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                LevelSize = 1;
                Create();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                LevelSize = 2;
                Create();
            }
            if (Input.GetKeyDown(KeyCode.F))
            {
                LevelSize = 3;
                Create();
            }



        }

        void Create()
        {
            Debug.Log("Buildind level...");

            //
            // Init data
            //
            Init();

            //
            // Distribute tiles among sectors.
            // Just create the shape of each sector in the whole level.
            //
            ShapeSectors();


            //
            // Build sectors
            //
            BuildSectors();

            //
            // Set the starting point
            //
            ChooseEnteringAndExitingTiles();

            // 
            // Connect sectors
            //
            ConnectSectors();

            // 
            // Rotate walls
            //
            //RotateWalls();

            //
            // Create rooms
            //
            CreateRooms();

            DebugTiles();

            // 
            // Load geometry
            //
            BuildGeometry();


            
      
        }

        void BuildGeometry()
        {
            float tileSize = 4f;

            // Load assets
            List<TileAsset> assets = new List<TileAsset>(Resources.LoadAll<TileAsset>(TileAsset.ResourceFolder));

            Debug.Log("Assets.Count:" + assets.Count);

            // Create root
            GameObject root = new GameObject("Geometry");
            root.transform.position = Vector3.zero;
            root.transform.rotation = Quaternion.identity;

            int size = (int)Mathf.Sqrt(tiles.Length);

            // Create tiles
            for(int i=0; i<tiles.Length; i++)
            {
                TileAsset asset = assets.Find(t => t.name.ToLower() == tiles[i].code.ToLower());
                GameObject tile = Instantiate(asset.Prefab, root.transform);
                tile.transform.localPosition = new Vector3((i % size) * tileSize, 0f, -(i / size) * tileSize);
                tile.transform.localRotation = Quaternion.identity;

                if(tiles[i].wall != 0)
                {
                    Transform pivot = new List<Transform>(tile.GetComponentsInChildren<Transform>()).Find(t => t.tag == "WallPivot");
                    if(tiles[i].wall < 0)
                    {
                        DestroyImmediate(pivot.gameObject);
                    }
                    else
                    {
                        pivot.transform.eulerAngles = new Vector3(0f,90f*tiles[i].wall,0f);
                    }
                }
            }

        }

        void ConnectSectors()
        {
            if (sectors.Length == 1)
                return;

            if(sectors.Length == 2)
            {
                List<int> src = new List<int>();
                List<int> trg = new List<int>();
                Sector s = sectors[0];
                int size = (int)Mathf.Sqrt(tiles.Length);
                for(int i=0; i<s.tileIds.Count; i++)
                {
                    int tileId = s.tileIds[i];
                    
                    if (tileId % size == 0 || tileId % size == size - 1 || tileId / size == 0 || tileId / size == size - 1)
                        continue; // Tiles at the edge of the level

                    if(tiles[tileId - size].sectorIndex == 1)
                    {
                        src.Add(tileId);
                        trg.Add(tileId - size);
                    }
                    else if(tiles[tileId + size].sectorIndex == 1)
                    {
                        src.Add(tileId);
                        trg.Add(tileId + size);
                    }
                    else if(tiles[tileId - 1].sectorIndex == 1)
                    {
                        src.Add(tileId);
                        trg.Add(tileId - 1);
                    }
                    else if(tiles[tileId + 1].sectorIndex == 1)
                    {
                        src.Add(tileId);
                        trg.Add(tileId + 1);
                    }
                            
                     
                }

                int id = Random.Range(0, src.Count);
                int srcId = src[id];
                int trgId = trg[id];

                
                // Logic
                connections.Add(Connection.CreateNormalConnection(srcId, trgId));
                //connections.Add(Connection.CreateNormalConnection(trgId, srcId));
                
                // Geometry
                tiles[srcId].code = tiles[srcId].code.Substring(0, 2) + "CC" + tiles[srcId].code.Substring(4, 2);
                tiles[trgId].code = tiles[trgId].code.Substring(0, 2) + "CC" + tiles[trgId].code.Substring(4, 2);
            }

            if (sectors.Length > 2)
            {
                // Create al list with all the possible sources
                List<int> srcIds = new List<int>();
                for(int i=0;i<sectors.Length; i++)
                {
                    if (!IsFinalSector(i))
                        srcIds.Add(i);
                }

                // Create a list with all the possible targets
                // An element on this list can only be used once, so we can loop through until the list is empty
                List<int> trgIds = new List<int>();
                for (int i = 0; i < sectors.Length; i++)
                {
                    if (!IsInitialSector(i))
                        trgIds.Add(i);
                }

                Debug.Log("SrcIds.Count:" + srcIds.Count);

                while(srcIds.Count > 0)
                {
                    // Get a random source sector
                    int src = srcIds[Random.Range(0, srcIds.Count)];

                    // Create a temp list with all the possible targets
                    List<int> tmp = new List<int>();
                    foreach(int id in trgIds)
                    {
                        if (id == src)
                            continue;

                        if (!SectorsBorderOneOnother(src, id))
                            continue;

                        if (connections.Exists(c => c.sourceTileId == src && c.targetTileId == id))
                            continue;

                        tmp.Add(id);
                    }

                    // If the source has no available targets then remove it from the source list
                    if(tmp.Count == 0)
                    {
                        srcIds.Remove(src);
                        continue;
                    }

                    // Choose a random target from the list
                    int dst = tmp[Random.Range(0, tmp.Count)];

                    // Logic
                    int srcTileId, dstTileId;
                    TryGetRandomBorderBetweenSectors(src, dst, out srcTileId, out dstTileId);
                    connections.Add(Connection.CreateNormalConnection(srcTileId, dstTileId));
                    //connections.Add(Connection.CreateNormalConnection(dstTileId, srcTileId));

                    // Geometry
                    tiles[srcTileId].code = tiles[srcTileId].code.Substring(0, 2) + "CC" + tiles[srcTileId].code.Substring(4, 2);
                    tiles[dstTileId].code = tiles[dstTileId].code.Substring(0, 2) + "CC" + tiles[dstTileId].code.Substring(4, 2);

                    // Remove the target sector from the target list
                    trgIds.Remove(dst);

                    

                }


           

            }



        }


        void CreateRooms()
        {
            for(int i=0; i<sectors.Length;i++)
            {
                int min = 0;
                int max = sectors[i].tileIds.Count / 30;
                int roomCount = Random.Range(min, max + 1);
                if (roomCount > 0)
                    CreateRooms(roomCount, i);
            }
        }

        void CreateRooms(int roomCount, int sectorId)
        {
            // Copy all the available tiles into a new list
            List<int> ids = new List<int>();
            foreach(int id in sectors[sectorId].tileIds)
            {
                ids.Add(id);
            }

            List<int> roomsTiles = new List<int>();

            for(int i=0; i<roomCount; i++)
            {
                int maxWidth = Random.Range(2, 5);
                int maxHeight = Random.Range(2, 5);

                // Create a list with all the possible origins given the width and the height of the room
                List<int> tmp = new List<int>();
                foreach(int id in ids)
                {
                    //if(id + maxWidth < )
                }
            }
        }

        void ChooseEnteringAndExitingTiles()
        {
            // Get a random tile along the external edge 
            int size = (int)Mathf.Sqrt(tiles.Length);

            List<int> indices = new List<int>();
            for(int i=0; i<tiles.Length ; i++)
            {
                if (i == 0 || i == size - 1 || i == size * (size - 1) || i == size * size - 1)
                    continue;
                if (i < size || i > size * (size - 1) || i % size == 0 || i % size == size - 1)
                    indices.Add(i);
                    
            }

            for (int i = 0; i < indices.Count; i++)
                Debug.Log(indices[i]);

            // Set a random tile as the starting one
            int enteringTileIndex = indices[Random.Range(0, indices.Count)];
            
            // Geometry
            tiles[enteringTileIndex].code = tiles[enteringTileIndex].code.Substring(0, 2) + "CC" + tiles[enteringTileIndex].code.Substring(4, 2);

            // Logic
            connections.Add(Connection.CreateInitialConnection(enteringTileIndex));

            if (sectors.Length == 1)
            {
                // Keep only the opposite side
                if (enteringTileIndex % size == 0) // Left
                    indices.RemoveAll(i => i % size == 0);
                else if (enteringTileIndex % size == size - 2) // Right
                    indices.RemoveAll(i => i % size == size - 2);
                else if (enteringTileIndex / size == 0)
                    indices.RemoveAll(i => i / size == 0);
                else if (enteringTileIndex / size == size-1)
                    indices.RemoveAll(i => i / size == size-1);
            }
            else
            {
                // Remove only tiles from this sector
                indices.RemoveAll(i => tiles[i].sectorIndex == tiles[enteringTileIndex].sectorIndex);
            }

            int exitingTileIndex = indices[Random.Range(0, indices.Count)];
            // Geometry
            tiles[exitingTileIndex].code = tiles[exitingTileIndex].code.Substring(0, 2) + "CC" + tiles[exitingTileIndex].code.Substring(4, 2);
            // Logic
            connections.Add(Connection.CreateFinalConnection(exitingTileIndex));
        }

        void BuildSectors()
        {
            for (int i = 0; i < sectors.Length; i++)
                BuildSector(i);
        }

        void BuildSector(int sectorIndex)
        {
            Sector sector = sectors[sectorIndex];

            int size = (int)Mathf.Sqrt(tiles.Length); // Columns

            // Loop through each tile of the current sector
            for(int i=0; i<sector.tileIds.Count; i++)
            {
                int tileId = sector.tileIds[i];
                int col = tileId % size;
                int row = tileId / size;

                // Check whether the current tile is at the edge of the sector or not
                bool top = (row == 0 || tiles[tileId - size].sectorIndex != sectorIndex);
                bool left = (col == 0 || tiles[tileId - 1].sectorIndex != sectorIndex);
                bool right = (col == size - 1 || tiles[tileId + 1].sectorIndex != sectorIndex);
                bool bottom = (row == size - 1 || tiles[tileId + size].sectorIndex != sectorIndex);

                bool rotate = false;

                if(!top && !left && !right && !bottom)
                {
                    tiles[tileId].code = "CC" + tiles[tileId].code.Substring(2);
                    rotate = true;
                }
                else
                {
                    string code = "CC";
                    if (top)
                        code = "T" + code.Substring(1);
                    else if(bottom)
                        code = "B" + code.Substring(1);

                    if(left)
                        code = code.Substring(0,1) + "L";
                    else if (right)
                        code = code.Substring(0, 1) + "R";

                    if ((top && !right) || (left && !bottom))
                        rotate = true;

                
                    tiles[tileId].code = code + tiles[tileId].code.Substring(2);
                }

                if (rotate)
                {
                    List<int> rot = new List<int>(new int[] { 0, 1, 2, 3 });

                    
                    // Rotate 
                    int rotCode = rot[Random.Range(0, rot.Count)];
                    tiles[tileId].wall = rotCode;
                }
            }
        }

        void Init()
        {
            Transform g = new List<Transform>(FindObjectsOfType<Transform>()).Find(o => o.name == "Geometry");
            if (g)
                DestroyImmediate(g.gameObject);

            int tileCount = 0;
            int sectorCount = 0;

            switch (LevelSize)
            {
                case 0:
                    sectorCount = Random.Range(1, 3);
                    tileCount = 10 * 10;//Random.Range(30, 45);
                    break;
                case 1:
                    sectorCount = Random.Range(2, 5);
                    sectorCount = 3;
                    tileCount = 20 * 20;//Random.Range(50, 75);
                    break;
                case 2:
                    sectorCount = Random.Range(3, 6);
                    tileCount = 30 * 30;//Random.Range(60, 90);
                    break;
                case 3:
                    sectorCount = Random.Range(3, 6);
                    tileCount = 40 * 40;// Random.Range(70, 105);
                    break;

            }

            tiles = new Tile[tileCount];
            for (int i = 0; i < tiles.Length; i++)
                tiles[i] = new Tile();

            sectors = new Sector[sectorCount];
            for (int i = 0; i < sectorCount; i++)
                sectors[i] = new Sector();

            connections.Clear();
        }

        void ShapeSectors()
        {
            switch (sectors.Length)
            {
                case 1:
                    for (int i = 0; i < tiles.Length; i++)
                        sectors[0].tileIds.Add(i);
                    break;
                case 2:
                    int cols = (int)Mathf.Sqrt(tiles.Length);
                    bool vertical = Random.Range(0, 2) == 0;
                    Debug.Log("Builder - Vertical Distribution:" + vertical);
                   
                    for(int i=0; i<tiles.Length; i++)
                    {
                        int id = 1;
                        if((vertical && i % cols < cols/2) || (!vertical && i / cols < cols / 2))
                            id = 0;

                        sectors[id].tileIds.Add(i);
                        tiles[i].sectorIndex = id;
                    }
                   
                    break;

                case 3: // Three sectors
                    int type = Random.Range(0, 2);
                    type = 0;
                    switch (type)
                    {
                        case 0: // 1 big, 2 small
                            cols = (int)Mathf.Sqrt(tiles.Length);
                            for (int i = 0; i < tiles.Length; i++)
                            {
                                int id = 0;
                                if (i % cols >= cols / 2)
                                {
                                    if(i / cols < cols / 2)
                                        id = 1;
                                    else
                                        id = 2;

                                }
                                
                                sectors[id].tileIds.Add(i);
                                tiles[i].sectorIndex = id;
                            }
                            break;
                        case 1: // 2 big, 1 small

                            break;

                    }
                    
                    break;
            }
        }

        bool TryGetRandomBorderBetweenSectors(int sector1Id, int sector2Id, out int tile1Id, out int tile2Id)
        {
            tile1Id = -1;
            tile2Id = -1;

            List<int> list1 = new List<int>();
            List<int> list2 = new List<int>();

            int size = (int)Mathf.Sqrt(tiles.Length);

            Sector s1 = sectors[sector1Id];
            Sector s2 = sectors[sector2Id];
            for (int i = 0; i < s1.tileIds.Count; i++)
            {
                int tileId = s1.tileIds[i];
                if (tileId % size > 0 && tiles[tileId - 1].sectorIndex == sector2Id)
                {
                    list1.Add(tileId);
                    list2.Add(tileId - 1);
                }
                if (tileId % size < size - 1 && tiles[tileId + 1].sectorIndex == sector2Id)
                {
                    list1.Add(tileId);
                    list2.Add(tileId + 1);
                }
                if (tileId / size > 0 && tiles[tileId - size].sectorIndex == sector2Id)
                {
                    list1.Add(tileId);
                    list2.Add(tileId - size);
                }
                if (tileId / size < size - 1 && tiles[tileId + size].sectorIndex == sector2Id)
                {
                    list1.Add(tileId);
                    list2.Add(tileId + size);
                }
            }

            if (list1.Count == 0)
                return false;

            int id = Random.Range(0, list1.Count);
            tile1Id = list1[id];
            tile2Id = list2[id];
            return true;
        }

        bool SectorsBorderOneOnother(int sector1Id, int sector2Id)
        {

            int size = (int)Mathf.Sqrt(tiles.Length);

            Sector s1 = sectors[sector1Id];
            Sector s2 = sectors[sector2Id];
            for (int i = 0; i < s1.tileIds.Count; i++)
            {
                int tileId = s1.tileIds[i];
                if (tileId % size > 0 && tiles[tileId - 1].sectorIndex == sector2Id)
                {
                    return true;
                }
                if (tileId % size < size - 1 && tiles[tileId + 1].sectorIndex == sector2Id)
                {
                    return true;
                }
                if (tileId / size > 0 && tiles[tileId - size].sectorIndex == sector2Id)
                {
                    return true;
                }
                if (tileId / size < size - 1 && tiles[tileId + size].sectorIndex == sector2Id)
                {
                    return true;
                }
            }

            return false;

        }

        bool IsInitialSector(int index)
        {
            return index == tiles[connections.Find(c => c.IsInitialConnection()).sourceTileId].sectorIndex;
        }

        bool IsFinalSector(int index)
        {
            return index == tiles[connections.Find(c => c.IsFinalConnection()).sourceTileId].sectorIndex;
        }

        
        void DebugTiles()
        {
            int size = (int) Mathf.Sqrt(tiles.Length);
            string log = "";
            for(int i=0; i<size; i++)
            {
                for(int j=0; j<size; j++)
                {
                    if (j % size == 0)
                        log += "\n|";
                    log += tiles[i * size + j % size] + "|";
                }
            }

            Debug.Log(log);
        }


    }


}
