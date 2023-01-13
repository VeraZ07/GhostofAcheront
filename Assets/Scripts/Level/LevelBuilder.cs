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

        public enum TileType { TopLeft, TopCenter, TopRight, Left, Center, Right, BottomLeft, BottomCenter, BottomRight}

        

        class Tile
        {


            LevelBuilder builder;

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
            //public string code = "AAddWA";
            //public string Code
            //{
            //    get { return GetCode(); }
            //}
            public int sectorIndex;

            /// <summary>
            /// 0: default rot
            /// 1: 90 degrees
            /// 2: 180 degrees
            /// 3: 270 degrees
            /// -1: no wall
            /// </summary>
            public float wall = 0;

            public int type;

            /// <summary>
            /// 0: wall - default
            /// 1: room
            /// </summary>
            public int style = 0; 

            public Vector3 openDirection = Vector3.zero;

            public Tile(LevelBuilder builder)
            {
                this.builder = builder;
            }

            public string GetCode()
            {
                string code = "";
                switch (type)
                {
                    case (int)TileType.TopLeft:
                        code = "tl";
                        break;
                    case (int)TileType.TopCenter:
                        code = "tc";
                        break;
                    case (int)TileType.TopRight:
                        code = "tr";
                        break;
                    case (int)TileType.Left:
                        code = "ll";
                        break;
                    case (int)TileType.Right:
                        code = "rr";
                        break;
                    case (int)TileType.Center:
                        code = "cc";
                        break;
                    case (int)TileType.BottomLeft:
                        code = "bl";
                        break;
                    case (int)TileType.BottomCenter:
                        code = "bc";
                        break;
                    case (int)TileType.BottomRight:
                        code = "br";
                        break;
                }

                code += (openDirection != Vector3.zero) ? "cc" : "dd";

                code += style == 0 ? "wa" : "ra";

                return code;
            }

            public override string ToString()
            {
                return string.Format(" {0}:{1} ", sectorIndex, GetCode());
            }

        }

        [System.Serializable]
        class Connection
        {
            

            [SerializeField]
            public int sourceTileId = -1;
            [SerializeField]
            public int targetTileId = -1;


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
                c.sourceTileId = sourceTileId;
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
        class Sector
        {
            LevelBuilder builder;

            [SerializeField]
            public List<int> tileIds = new List<int>();


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

                // Loop through each tile of the current sector
                for (int i = 0; i < tileIds.Count; i++)
                {
                    int tileId = tileIds[i];
                    int col = tileId % size;
                    int row = tileId / size;

                    // Check whether the current tile is at the edge of the sector or not
                    bool top = (row == 0 || builder.tiles[tileId - size].sectorIndex != sectorIndex);
                    bool left = (col == 0 || builder.tiles[tileId - 1].sectorIndex != sectorIndex);
                    bool right = (col == size - 1 || builder.tiles[tileId + 1].sectorIndex != sectorIndex);
                    bool bottom = (row == size - 1 || builder.tiles[tileId + size].sectorIndex != sectorIndex);

                    bool rotate = false;

                    if (!top && !left && !right && !bottom)
                    {
                        //builder.tiles[tileId].code = "CC" + builder.tiles[tileId].code.Substring(2);
                        builder.tiles[tileId].type = (int)TileType.Center;
                        rotate = true;
                    }
                    else
                    {
                       
                        if (top)
                        {
                            if (left)
                                builder.tiles[tileId].type = (int)TileType.TopLeft;
                            else if(right)
                                builder.tiles[tileId].type = (int)TileType.TopRight;
                            else
                                builder.tiles[tileId].type = (int)TileType.TopCenter;
                        }
                        else
                        {
                            if (bottom)
                            {
                                if (left)
                                    builder.tiles[tileId].type = (int)TileType.BottomLeft;
                                else if (right)
                                    builder.tiles[tileId].type = (int)TileType.BottomRight;
                                else
                                    builder.tiles[tileId].type = (int)TileType.BottomCenter;
                            }
                            else
                            {
                                if (left)
                                    builder.tiles[tileId].type = (int)TileType.Left;
                                else if (right)
                                    builder.tiles[tileId].type = (int)TileType.Right;
                                else
                                    builder.tiles[tileId].type = (int)TileType.Center;

                            }
                        }
                        if ((top && !right) || (left && !bottom))
                            rotate = true;


                        //builder.tiles[tileId].code = code + builder.tiles[tileId].code.Substring(2);
                    }

                    // Rotate walls to shape the maze
                    if (rotate)
                    {
                        List<int> rot = new List<int>(new int[] { 0, 1, 2, 3 });


                        // Rotate 
                        int rotCode = rot[Random.Range(0, rot.Count)];
                        builder.tiles[tileId].wall = rotCode;
                    }
                }
            }

        }

        Tile[] tiles;

        Sector[] sectors;

        [SerializeField]
        List<Connection> connections = new List<Connection>();

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

                Debug.LogFormat("[Room W:{0}, H:{1}, Tiles.Count:{2}, Sector:{3}", width, height, tileIds.Count, sectorId);
                string s = "";
                for (int i = 0; i < tileIds.Count; i++)
                {
                    int id = tileIds[i];
                    Tile tile = builder.tiles[id];

                    tile.wall = 0; // Eventually reset the wall to avoid builder trying to rotate it
                    tile.style = 1;

                    // Set the type of tile
                    if (i == 0)
                        tile.type = (int)TileType.TopLeft;
                    else if (i == width - 1)
                        tile.type = (int)TileType.TopRight;
                    else if (i == width * (height - 1))
                        tile.type = (int)TileType.BottomLeft;
                    else if (i == width * (height - 1) + width - 1)
                        tile.type = (int)TileType.BottomRight;
                    else if (i % width == 0)
                        tile.type = (int)TileType.Left;
                    else if (i % width == width-1)
                        tile.type = (int)TileType.Right;
                    else if (i / width == 0)
                        tile.type = (int)TileType.TopCenter;
                    else if (i / width == height - 1)
                        tile.type = (int)TileType.BottomCenter;
                    else
                        tile.type = (int)TileType.Center;

                    // Check adjacent tiles
                    if (i % width == 0 && builder.sectors[sectorId].tileIds.Contains(id - 1))
                        builder.tiles[id - 1].wall = -1;
                    if (i / width == 0 && builder.sectors[sectorId].tileIds.Contains(id - size))
                        builder.tiles[id - size].wall = -1;
                    if (i / width == 0 && i % width == 0 && builder.sectors[sectorId].tileIds.Contains(id - size - 1))
                        builder.tiles[id - size - 1].wall = -1;
                }


            }
        }

        List<Room> rooms = new List<Room>();

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
                Debug.Log("TileCode:" + tiles[i].GetCode().ToLower());
                TileAsset asset = assets.Find(t => t.name.ToLower() == tiles[i].GetCode().ToLower());
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
                connections.Add(Connection.CreateNormalConnection(this, srcId, trgId));
                //connections.Add(Connection.CreateNormalConnection(trgId, srcId));

                // Geometry
                Vector3 dir = Vector3.zero;
                if(srcId < trgId)
                {
                    if (srcId == trgId - 1)
                        dir = Vector3.right;
                    else
                        dir = Vector3.back;
                }
                else
                {
                    if (srcId == trgId + 1)
                        dir = Vector3.left;
                    else
                        dir = Vector3.forward;
                }
                tiles[srcId].openDirection = dir;
                tiles[trgId].openDirection = -dir;
                //tiles[srcId].code = tiles[srcId].code.Substring(0, 2) + "CC" + tiles[srcId].code.Substring(4, 2);
                //tiles[trgId].code = tiles[trgId].code.Substring(0, 2) + "CC" + tiles[trgId].code.Substring(4, 2);
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
                    int srcId, trgId;
                    TryGetRandomBorderBetweenSectors(src, dst, out srcId, out trgId);
                    connections.Add(Connection.CreateNormalConnection(this, srcId, trgId));
                    //connections.Add(Connection.CreateNormalConnection(dstTileId, srcTileId));

                    // Geometry
                    Vector3 dir = Vector3.zero;
                    if (srcId < trgId)
                    {
                        if (srcId == trgId - 1)
                            dir = Vector3.right;
                        else
                            dir = Vector3.back;
                    }
                    else
                    {
                        if (srcId == trgId + 1)
                            dir = Vector3.left;
                        else
                            dir = Vector3.forward;
                    }
                    tiles[srcId].openDirection = dir;
                    tiles[trgId].openDirection = -dir;
                    //tiles[srcTileId].code = tiles[srcTileId].code.Substring(0, 2) + "CC" + tiles[srcTileId].code.Substring(4, 2);
                    //tiles[dstTileId].code = tiles[dstTileId].code.Substring(0, 2) + "CC" + tiles[dstTileId].code.Substring(4, 2);

                    // Remove the target sector from the target list
                    trgIds.Remove(dst);

                    

                }


           

            }



        }


        void CreateRooms()
        {
            for(int i=0; i<sectors.Length;i++)
            {
                Sector sector = sectors[i];
                int sizeInTiles = sector.width * sector.height; // In tiles

                // Max room tiles
                int maxTiles = sizeInTiles * 20 / 100;
                int maxTilesWidth = sector.width;
                int maxTilesHeight = sector.height;

                // Try to create the max number of rooms
                while(maxTiles > 0)
                {
                    int maxWidth = Mathf.Min(6, maxTilesWidth);
                    int maxHeight = Mathf.Min(6, maxTilesHeight);

                    int minWidth = Mathf.Min(3, maxTilesWidth);
                    int minHeight = Mathf.Min(3, maxTilesHeight);

                    // We only create commons rooms for now
                    int w = Random.Range(minWidth, maxWidth + 1);
                    int h = Random.Range(minHeight, maxHeight + 1);
                    Debug.Log("W:" + w);
                    Debug.Log("H:" + h);
                    Debug.Log("MaxTiles:" + maxTiles);
                    if (w * h > maxTiles)
                    {
                        if(w < h)
                            h = maxTiles / w;
                        else
                            w = maxTiles / h;
                    }
                    
                    if (h > 1 && w > 1)
                    {
                        Room room = new Room(this, i, w, h);
                        rooms.Add(room);
                    }
                    
                    maxTiles -= (w * h);

                    if (maxTiles < 4)
                        maxTiles = 0;
                }

                List<int> notAllowed = new List<int>();
                // Create rooms
                for(int j= 0; j<rooms.Count; j++)
                {
                    Room room = rooms[j];
                    // Get available tiles
                    List<int> tiles = GetTilesForRoom(room.sectorId, room.width, room.height, notAllowed);

                    if(tiles.Count > 0)
                    {
                        // We can't use the same tile twice
                        notAllowed.AddRange(tiles);

                        room.tileIds = tiles;

                        room.Create();
                    }
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
            if (enteringTileIndex % size == 0)
                tiles[enteringTileIndex].openDirection = Vector3.right;
            else if (enteringTileIndex % size == size - 1)
                tiles[enteringTileIndex].openDirection = Vector3.left;
            else if (enteringTileIndex / size == 0)
                tiles[enteringTileIndex].openDirection = Vector3.back;
            else
                tiles[enteringTileIndex].openDirection = Vector3.forward;
            //tiles[enteringTileIndex].code = tiles[enteringTileIndex].code.Substring(0, 2) + "CC" + tiles[enteringTileIndex].code.Substring(4, 2);

            // Logic
            connections.Add(Connection.CreateInitialConnection(this, enteringTileIndex));

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

            if (exitingTileIndex % size == 0)
                tiles[exitingTileIndex].openDirection = Vector3.right;
            else if (exitingTileIndex % size == size - 1)
                tiles[exitingTileIndex].openDirection = Vector3.left;
            else if (exitingTileIndex / size == 0)
                tiles[exitingTileIndex].openDirection = Vector3.back;
            else
                tiles[exitingTileIndex].openDirection = Vector3.forward;

            // Geometry
            //tiles[exitingTileIndex].code = tiles[exitingTileIndex].code.Substring(0, 2) + "CC" + tiles[exitingTileIndex].code.Substring(4, 2);
            // Logic
            connections.Add(Connection.CreateFinalConnection(this, exitingTileIndex));
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
                tiles[i] = new Tile(this);

            sectors = new Sector[sectorCount];
            for (int i = 0; i < sectorCount; i++)
                sectors[i] = new Sector(this);

            connections.Clear();
            rooms.Clear();
        }

        void BuildSectors()
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

            // Set width and heigh for each sector
            for(int i=0; i<sectors.Length; i++)
            {
                int size = (int)Mathf.Sqrt(tiles.Length);
                int minX = 0, maxX = 0, minZ = 0, maxZ = 0;
                Sector sector = sectors[i];
                // Get boundaries
                for(int j=0; j<sector.tileIds.Count; j++)
                {
                    int index = sector.tileIds[j];
                    int w = index % size;
                    int h = index / size;
                    if (j == 0)
                    {
                        minX = maxX = w;
                        minZ = maxZ = h;
                    }
                    else
                    {
                        if (minX > w) minX = w;
                        if (maxX < w) maxX = w;
                        if (minZ > h) minZ = h;
                        if (maxZ < h) maxZ = h;
                    }
                }

                // Compute width and heigh
                sector.width = maxX - minX + 1;
                sector.height = maxZ - minZ + 1;

                sectors[i].Build();
            }

            //for (int i = 0; i < sectors.Length; i++)
            //    sectors[i].Create(this);
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

        bool IsBorderTile(int tileId)
        {

            int size = (int)Mathf.Sqrt(tiles.Length);
            int sectorId = tiles[tileId].sectorIndex;

            if (tileId % size > 0 && tiles[tileId - 1].sectorIndex != sectorId)
            {
                return true;
            }
            if (tileId % size < size - 1 && tiles[tileId + 1].sectorIndex != sectorId)
            {
                return true;
            }
            if (tileId / size > 0 && tiles[tileId - size].sectorIndex != sectorId)
            {
                return true;
            }
            if (tileId / size < size - 1 && tiles[tileId + size].sectorIndex != sectorId)
            {
                return true;
            }

            return false;
        }
        
        List<int> GetTilesForRoom(int sectorIndex, int width, int height, List<int> notAllowedTileIds)
        {
            int[] tmp = new int[sectors[sectorIndex].tileIds.Count];
            sectors[sectorIndex].tileIds.CopyTo(tmp);
            List<int> tileIds = new List<int>(tmp);
            tileIds.RemoveAll(id => notAllowedTileIds.Contains(id));

            int size = (int)Mathf.Sqrt(tiles.Length);

            List<int> ret = new List<int>();

            bool found = false;
            while(!found && tileIds.Count > 0)
            {
                // Extract a random tile
                int origin = tileIds[Random.Range(0, tileIds.Count)];
                tileIds.Remove(origin);

               
                ret.Clear();
                // From top-left to bottom-rigth
                for(int i=0; i<width * height; i++)
                {
                    int id = origin + i % width + i / width * size;
                    if (!sectors[sectorIndex].tileIds.Contains(id))
                        break;
                        
                    if (notAllowedTileIds.Contains(id))
                        break;

                    // If one of the corner of the candidate room falls in a north or south tile connection then continue
                    if ((i == 0 || i == width - 1) && tiles[id].openDirection != Vector3.zero)
                        break;
                    if ((i == width * (height - 1) || i == width * (height - 1) + width - 1) && tiles[id].openDirection != Vector3.zero)
                        break;

                    ret.Add(id);
                }

                if(ret.Count == width * height)
                    found = true;
            }

            return ret;
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
