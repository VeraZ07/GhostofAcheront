using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOA.Assets;
using Fusion;
using Unity.AI.Navigation;

namespace GOA.Level
{
    public partial class LevelBuilder : MonoBehaviour
    {

        
        // From 0 to N, 0 is the smallest one.
        public static int LevelSize = 1;


        [SerializeField]
        Tile[] tiles;

        [SerializeField]
        Sector[] sectors;

        [SerializeField]
        List<Connection> connections = new List<Connection>();
                
        [SerializeField]
        List<Room> rooms = new List<Room>();

        [SerializeField]
        List<CustomObject> customObjects = new List<CustomObject>();

        [SerializeField]
        List<Puzzle> puzzles = new List<Puzzle>();

        NetworkRunner runner;

        bool onClosedPathRemoveWall = false;

        int theme = 0;

        Transform geometryRoot;
        
        private void Awake()
        {
           
        }

        private void Start()
        {
            //int seed = FindObjectOfType<GameManager>().GameSeed;

            //Random.InitState(seed);
            //Debug.Log("Seed:" + seed);
            
            //Create();
            
        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                LevelSize = 0;
                Create();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                LevelSize = 1;
                Create();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                LevelSize = 2;
                Create();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
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
            // Add gates to connections
            //
            CreateGates();

            //
            // Create puzzles
            //
            CreatePuzzles();

            //
            // Create rooms
            //
            CreateRooms();

            DebugTiles();

            CheckForUnreachableTiles();

            // Create puzzles


            // 
            // Set the monster spawn tile
            //
            ChooseTheMonsterSpawnTile();

            // 
            // Load geometry
            //
            BuildGeometry();


            
      
        }

        void BuildGeometry()
        {


            //float tileSize = 4f;

            // Load assets
            //List<TileAsset> assets = new List<TileAsset>(Resources.LoadAll<TileAsset>(TileAsset.ResourceFolder));

            //Debug.Log("Assets.Count:" + assets.Count);

            // Create root
            geometryRoot = new GameObject("Geometry").transform;
            geometryRoot.position = Vector3.zero;
            geometryRoot.rotation = Quaternion.identity;

            int size = (int)Mathf.Sqrt(tiles.Length);

            // Create tiles
            for(int i=0; i<tiles.Length; i++)
            {
                Debug.Log("Creating TileId:" + i);
                //TileAsset asset = assets.Find(t => t.name.ToLower() == tiles[i].GetCode().ToLower());
                TileAsset asset = tiles[i].asset;
                GameObject tile = Instantiate(asset.Prefab, geometryRoot);
                tiles[i].sceneObject = tile; // Set the scene object reference
                tile.name = string.Format("{0}.{1}", i, tile.name); 
                tile.transform.localPosition = new Vector3((i % size) * Tile.Size, 0f, -(i / size) * Tile.Size);
                tile.transform.localRotation = Quaternion.identity;

                if(tiles[i].roteableWall != 0)
                {
                    Transform pivot = new List<Transform>(tile.GetComponentsInChildren<Transform>()).Find(t => "pv".Equals(t.name.ToLower()));
                    if(tiles[i].roteableWall < 0)
                    {
                        if(pivot)
                            DestroyImmediate(pivot.gameObject);
                    }
                    else
                    {
                        pivot.transform.eulerAngles = new Vector3(0f,90f*tiles[i].roteableWall,0f);
                    }
                }

                // Check fixed walls
                Transform[] children = new Transform[tile.transform.childCount];
                for (int k = 0; k < children.Length; k++)
                    children[k] = tile.transform.GetChild(k);

                for(int c=0; c<children.Length; c++)
                {
                    Debug.Log("Check tile " + children[c].name);
                    if (!tiles[i].isRoomTile)
                    {
                        if (children[c].name.ToLower().StartsWith("ub_"))
                        {
                            if ((!tiles[i].isUpperBoundary) ||
                                (tiles[i].openDirection == Vector3.forward && children[c].name.ToLower().EndsWith("_c")) ||
                                (tiles[i].openDirection != Vector3.forward && children[c].name.ToLower().EndsWith("_o")))
                            {
                                DestroyImmediate(children[c].gameObject);
                            }
                            continue;
                        }
                        if (children[c].name.ToLower().StartsWith("rb_"))
                        {
                            if ((!tiles[i].isRightBoundary) ||
                                (tiles[i].openDirection == Vector3.right && children[c].name.ToLower().EndsWith("_c")) ||
                                (tiles[i].openDirection != Vector3.right && children[c].name.ToLower().EndsWith("_o")))
                            {
                                DestroyImmediate(children[c].gameObject);
                            }
                            continue;
                        }
                        if (children[c].name.ToLower().StartsWith("bb_"))
                        {
                            if ((!tiles[i].isBottomBoundary) ||
                                (tiles[i].openDirection == Vector3.back && children[c].name.ToLower().EndsWith("_c")) ||
                                (tiles[i].openDirection != Vector3.back && children[c].name.ToLower().EndsWith("_o")))
                            {
                                DestroyImmediate(children[c].gameObject);
                            }
                            continue;
                        }
                        if (children[c].name.ToLower().StartsWith("lb_"))
                        {
                            if ((!tiles[i].isLeftBoundary) ||
                                (tiles[i].openDirection == Vector3.left && children[c].name.ToLower().EndsWith("_c")) ||
                                (tiles[i].openDirection != Vector3.left && children[c].name.ToLower().EndsWith("_o")))
                            {
                                DestroyImmediate(children[c].gameObject);
                            }
                            continue;
                        }
                    }
                    else // It's a room
                    {

                        if (children[c].name.ToLower().StartsWith("uw_") || children[c].name.ToLower().StartsWith("ub_"))
                        {
                            if ((tiles[i].openDirection == Vector3.forward && children[c].name.ToLower().EndsWith("_c")) ||
                                (tiles[i].openDirection != Vector3.forward && children[c].name.ToLower().EndsWith("_o")) ||
                                (!tiles[i].isUpperBorder))
                            {
                                DestroyImmediate(children[c].gameObject);
                            }
                            else
                            {
                                if (children[c].name.ToLower().StartsWith("uw_") && tiles[i].isUpperBoundary)
                                    DestroyImmediate(children[c].gameObject);
                            }
                            continue;
                        }
                        
                        if (children[c].name.ToLower().StartsWith("rw_") || children[c].name.ToLower().StartsWith("rb_"))
                        {
                            if ((tiles[i].openDirection == Vector3.right && children[c].name.ToLower().EndsWith("_c")) ||
                                (tiles[i].openDirection != Vector3.right && children[c].name.ToLower().EndsWith("_o")) ||
                                (!tiles[i].isRightBorder))
                            {
                                DestroyImmediate(children[c].gameObject);
                            }
                            else
                            {
                                if (children[c].name.ToLower().StartsWith("rw_") && tiles[i].isRightBoundary)
                                    DestroyImmediate(children[c].gameObject);
                            }
                            continue;
                        }
                        if (children[c].name.ToLower().StartsWith("bw_") || children[c].name.ToLower().StartsWith("bb_"))
                        {
                            if ((tiles[i].openDirection == Vector3.back && children[c].name.ToLower().EndsWith("_c")) ||
                                (tiles[i].openDirection != Vector3.back && children[c].name.ToLower().EndsWith("_o")) ||
                                (!tiles[i].isBottomBorder))
                            {
                                
                                DestroyImmediate(children[c].gameObject);
                            }
                            else
                            {
                                if(children[c].name.ToLower().StartsWith("bw_") && tiles[i].isBottomBoundary)
                                    DestroyImmediate(children[c].gameObject);
                            }
                            continue;
                        }
                        if (children[c].name.ToLower().StartsWith("lw_") || children[c].name.ToLower().StartsWith("lb_"))
                        {
                            if ((tiles[i].openDirection == Vector3.left && children[c].name.ToLower().EndsWith("_c")) ||
                                (tiles[i].openDirection != Vector3.left && children[c].name.ToLower().EndsWith("_o")) ||
                                (!tiles[i].isLeftBorder))
                            {
                                DestroyImmediate(children[c].gameObject);
                            }
                            else
                            {
                                if (children[c].name.ToLower().StartsWith("lw_") && tiles[i].isLeftBoundary)
                                    DestroyImmediate(children[c].gameObject);
                            }
                            continue;
                        }
                    }
                   
                   
                    
                   
                }
              

               
            }


            // Add starting and final rooms
            int startingTileId = connections.Find(c => c.IsInitialConnection()).sourceTileId;
            int finalTileId = connections.Find(c => c.IsFinalConnection()).sourceTileId;


            TileAsset tmpAsset = Resources.Load<TileAsset>(System.IO.Path.Combine(TileAsset.ResourceFolder, theme.ToString(), "startingRoom"));
            Vector3 dir = tiles[startingTileId].openDirection;

            GameObject tileObj = new List<Transform>(FindObjectsOfType<Transform>()).Find(t => t.name.ToLower().StartsWith(string.Format("{0}.", startingTileId))).gameObject;
            GameObject room = Instantiate(tmpAsset.Prefab, geometryRoot);
            room.transform.rotation = Quaternion.LookRotation(-dir);

            Vector3 move = Vector3.zero;
            if (dir == Vector3.forward)
                move = Vector3.right * Tile.Size;
            else if (dir == Vector3.right)
                move = Vector3.right * Tile.Size + Vector3.back * Tile.Size;
            else if(dir == Vector3.back)
                move = Vector3.back * Tile.Size;
            

            room.transform.position = tileObj.transform.position + move;// + dir * tileSize/* + Vector3.Cross(Vector3.up, dir) * tileSize*/;

            // Final room 
            tmpAsset = Resources.Load<TileAsset>(System.IO.Path.Combine(TileAsset.ResourceFolder, theme.ToString(), "finalRoom"));
            dir = tiles[finalTileId].openDirection;

            tileObj = new List<Transform>(FindObjectsOfType<Transform>()).Find(t => t.name.ToLower().StartsWith(string.Format("{0}.", finalTileId))).gameObject;
            room = Instantiate(tmpAsset.Prefab, geometryRoot);
            room.transform.rotation = Quaternion.LookRotation(-dir);

            move = Vector3.zero;
            if (dir == Vector3.forward)
                move = Vector3.right * Tile.Size;
            else if (dir == Vector3.right)
                move = Vector3.right * Tile.Size + Vector3.back * Tile.Size;
            else if (dir == Vector3.back)
                move = Vector3.back * Tile.Size;


            room.transform.position = tileObj.transform.position + move;

            
            //
            // Create gates
            //
            List<CustomObject> gates = new List<CustomObject>(customObjects).FindAll(c => c.GetType() == typeof(Gate));
            foreach (CustomObject gate in gates)
            {
                // Get the asset
                CustomObjectAsset gateAsset = gate.asset;
                // Instantiate the scene object
                GameObject gateObj = Instantiate(gateAsset.Prefab, geometryRoot);
                
                // Set date
                gate.sceneObject = gateObj;

                // Position the object
                Tile tile = tiles[gate.tileId];
                Vector3 position = tile.GetPosition();
                gateObj.transform.position = position;
                gateObj.transform.GetChild(0).forward = gate.direction;
            }

            //
            // Create puzzles object
            //
            NetworkRunner runner = FindObjectOfType<NetworkRunner>();
            Player localPlayer = new List<Player>(FindObjectsOfType<Player>()).Find(p => p.HasStateAuthority);
            foreach(Puzzle puzzle in puzzles)
            {
                runner.Spawn(puzzle.asset.Prefab, Vector3.zero, Quaternion.identity, null, 
                    (runner, obj)=> 
                    { 
                        obj.GetComponent<PuzzleController>().PuzzleIndex = puzzles.IndexOf(puzzle); 
                    });
            }

            // Bake navigation mesh
            FindObjectOfType<NavMeshSurface>().BuildNavMesh();
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

                
                
            }

            List<int> notAllowed = new List<int>();
            Debug.Log("Rooms.Count:" + rooms.Count);
            // Create rooms
            for (int j = 0; j < rooms.Count; j++)
            {
                Room room = rooms[j];
                // Get available tiles
                List<int> tiles = GetTilesForRoom(room.sectorId, room.width, room.height, notAllowed);

               
                if (tiles.Count > 0)
                {
                    // We can't use the same tile twice
                    notAllowed.AddRange(tiles);

                    room.tileIds = tiles;

                    room.Create();
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
                tiles[enteringTileIndex].openDirection = Vector3.left;
            else if (enteringTileIndex % size == size - 1)
                tiles[enteringTileIndex].openDirection = Vector3.right;
            else if (enteringTileIndex / size == 0)
                tiles[enteringTileIndex].openDirection = Vector3.forward;
            else
                tiles[enteringTileIndex].openDirection = Vector3.back;
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
                tiles[exitingTileIndex].openDirection = Vector3.left;
            else if (exitingTileIndex % size == size - 1)
                tiles[exitingTileIndex].openDirection = Vector3.right;
            else if (exitingTileIndex / size == 0)
                tiles[exitingTileIndex].openDirection = Vector3.forward;
            else
                tiles[exitingTileIndex].openDirection = Vector3.back;

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
            customObjects.Clear();
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

            // Set width and height for each sector
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

                    if ((origin / size) + (i / width) != id / size)
                        break;

                    // If one of the corner of the candidate room falls in a north or south tile connection then break
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

        void ChooseTheMonsterSpawnTile()
        {
            // Get the starting tile
            Connection startConnection = connections.Find(c => c.IsInitialConnection());
            int tileId = startConnection.sourceTileId;
            Vector3 direction = tiles[tileId].openDirection * -1f;
            Sector sector = sectors[tiles[tileId].sectorIndex];

            List<int> candidates = new List<int>();
            // Get all the tiles at a minimum X from the start


            // Get all the tiles at a minimum Z from the start
        }

        

        void CheckForUnreachableTiles()
        {
            int size = (int)Mathf.Sqrt(tiles.Length);

          
            List<int> closedWalls = new List<int>();
            

            // Loop through all the wall tiles
            for(int i=0; i<tiles.Length; i++)
            {
                if (tiles[i].roteableWall < 0)
                    continue;

                if (tiles[i].unreachable)
                    continue;

                if (closedWalls.Contains(i))
                    continue;

                

                List<int> tl = new List<int>();
                // Starting from the current wall we try to draw a closed shape using wall from the other tiles
                
                bool closed = false;

                int current = i;

                List<int> candidates = new List<int>();
  

                while (true)
                {
                 
                    candidates.Add(current);
                    
                    int wallRot = (int)tiles[current].roteableWall;
                    int next = -1;
                    switch (wallRot)
                    {
                        case 0:
                            if(current / size > 0)
                                next = current - size;
                            break;
                        case 1:
                            if(current % size < size - 1)
                                next = current + 1;
                            break;
                        case 2:
                            if(current / size < size - 1)
                                next = current + size;
                            break;
                        case 3:
                            if(current % size > 0)
                                next = current - 1;
                            break;
                    }

                   

                    if (next < 0)
                        break;

                    if (next == i)
                    {
                        closed = true;
                        break;
                    }

                    if (candidates.Contains(next))
                        break;
                    
                    if (tiles[next].roteableWall < 0)
                    {
                        break;
                    }
                        
                    current = next;

                   
                }

                if (closed)
                {

                    if (!onClosedPathRemoveWall)
                    {
                        // Get the top left tile
                        int minCol = 0;
                        int minRow = 0;
                        int topLeft = -1;
                        for (int j = 0; j < candidates.Count; j++)
                        {
                            int col = candidates[j] % size;
                            int row = candidates[j] / size;
                            if (topLeft == -1 || col < minCol || (col == minCol && row < minRow))
                            {
                                topLeft = j;
                                minCol = col;
                                minRow = row;
                            }

                        }

                        // This is the first tile we can tell not reachable
                        int startTileId = candidates[topLeft] + size + 1;

                        Debug.Log("Start Tile Id:" + startTileId);
                        RecursiveSetUnreachable(startTileId);

                    }
                    else
                    {
                        tiles[candidates[Random.Range(0, candidates.Count)]].roteableWall = -1;
                    }
                    

                    //_testUnreachables.AddRange(candidates);
                    closedWalls.AddRange(candidates);
                }
            }

            

        }

        void RecursiveSetUnreachable(int startTileId)
        {
            Debug.Log("Recursive tile id:" + startTileId);
            tiles[startTileId].unreachable = true;
            int size = (int)Mathf.Sqrt(tiles.Length);
           
            // North
            if (!tiles[startTileId-size].unreachable && tiles[startTileId - size].roteableWall != 3 && tiles[startTileId - size - 1].roteableWall != 1)
                RecursiveSetUnreachable(startTileId - size);

            
            // East
            if (!tiles[startTileId + 1].unreachable && tiles[startTileId].roteableWall != 0 && tiles[startTileId-size].roteableWall != 2)
                RecursiveSetUnreachable(startTileId + 1);

            
            // South
            if (!tiles[startTileId + size].unreachable && tiles[startTileId].roteableWall != 3 && tiles[startTileId-1].roteableWall!=1)
                RecursiveSetUnreachable(startTileId + size);

            // West
            if (!tiles[startTileId - 1].unreachable && tiles[startTileId - 1].roteableWall != 0 && tiles[startTileId - size - 1].roteableWall != 2)
                RecursiveSetUnreachable(startTileId - 1);


        }

        void CreateGates()
        {
            

            List<CustomObjectAsset> gateAssets = new List<CustomObjectAsset>(Resources.LoadAll<CustomObjectAsset>(System.IO.Path.Combine(CustomObjectAsset.ResourceFolder, theme.ToString(), "Gates")));

            // Loop through every connection
            foreach (Connection conn in connections)
            {
                if (conn.IsInitialConnection())
                    continue;

                int tileId = conn.sourceTileId;

                // Create the new gate 
                Gate co = new Gate(this, gateAssets[Random.Range(0, gateAssets.Count)]);
                customObjects.Add(co);
                co.direction = tiles[tileId].openDirection;
                co.tileId = tileId;
                conn.gateIndex = customObjects.Count - 1;

            }
        }

        //[SerializeField]
        //List<Test> tests = new List<Test>();
        void CreatePuzzles()
        {
            
            List<PuzzleAsset> puzzleCollection = new List<PuzzleAsset>(Resources.LoadAll<PuzzleAsset>(System.IO.Path.Combine(PuzzleAsset.ResourceFolder, theme.ToString())));

            Resources.LoadAll <PuzzleAsset> ("");

            // Get all gates
            List<CustomObject> gates = customObjects.FindAll(g => g.GetType() == typeof(Gate));
            foreach(Gate gate in gates)
            {
                PuzzleAsset asset = puzzleCollection[Random.Range(0, puzzleCollection.Count)];
                Debug.Log("PuzzleAssets.Count" + puzzleCollection.Count);
                // We need to find the connection to get the sector this puzzle belongs to.
                int gateIndex = customObjects.IndexOf(gate);
                Connection connection = connections.Find(c => c.gateIndex == gateIndex);

                Puzzle puzzle = PuzzleFactory.CreatePuzzle(this, asset, tiles[connection.sourceTileId].sectorIndex);
                //puzzles.Add(puzzle);
                //gate.puzzleIndex = puzzles.Count - 1;

                
            }
        }

        public void Build(int seed)
        {
            Random.InitState(seed);

            Create();
        }

        //public int CreateCustomObject(GameObject prefab)
        //{
        //    // We must add the object to a tile 
        //    //CustomObject co = new CustomObject(this, )
        //    GameObject obj = Instantiate(prefab, geometryRoot);
        //    customObjects.Add(obj.GetComponent<CustomObject>());

        //}

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
