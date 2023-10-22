//#define TEST_PUZZLE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOA.Assets;
using Fusion;
using Unity.AI.Navigation;
using GOA.Interfaces;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GOA.Level
{
    public partial class LevelBuilder : MonoBehaviour
    {
        public UnityAction OnLevelBuilt;
        
        // From 0 to N, 0 is the smallest one.
        //public static int LevelSize = 3;
        public static float TileSize = Tile.Size;
        public static float TileHeight = Tile.Height;

        [SerializeField]
        Tile[] tiles;
        
        [SerializeField]
        Sector[] sectors;
       
        [SerializeField]
        List<Connection> connections = new List<Connection>();
        public IList<Connection> Connections
        {
            get { return connections.AsReadOnly(); }
        }
        
                
        [SerializeField]
        List<Room> rooms = new List<Room>();

        [SerializeReference]
        List<CustomObject> customObjects = new List<CustomObject>();
        public IList<CustomObject> CustomObjects
        {
            get { return customObjects.AsReadOnly(); }
        }

        [SerializeReference]
        List<Puzzle> puzzles = new List<Puzzle>();

        [SerializeField]
        List<GameObject> triggers = new List<GameObject>();

        //[SerializeReference]
        //List<CustomObject> decals3D = new List<CustomObject>();

        //[SerializeReference]
        //List<CustomObject> decals2D = new List<CustomObject>();

        bool onClosedPathRemoveWall = false;

        int theme = 0;

        Transform geometryRoot;
        public Transform GeometryRoot
        {
            get { return geometryRoot; }
        }


        int monsterStartingTileId = 0;
              

        private void Awake()
        {
            PuzzleController.OnPuzzleControllerSpawned += HandleOnPuzzleControllerSpawned;
        }

        private void OnDestroy()
        {
            PuzzleController.OnPuzzleControllerSpawned -= HandleOnPuzzleControllerSpawned;
        }

        void HandleOnPuzzleControllerSpawned(PuzzleController puzzleController)
        {
            // Look for a gate connected to this puzzle controller
            Gate gate = customObjects.Find(c => c.GetType() == typeof(Gate) && (c as Gate).PuzzleIndex == puzzleController.PuzzleIndex) as Gate;
            GateController gateController = gate.SceneObject.GetComponent<GateController>();
            gateController.Init(puzzleController);
        }

        void Create()
        {
            
            System.DateTime startTime = System.DateTime.Now;

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
            // Create rooms
            //
            CreateRooms();

            

            CheckForUnreachableTiles();

#if !TEST_PUZZLE || !UNITY_EDITOR
            //
            // Create puzzles
            //
            CreatePuzzles();
#else
            //
            // Test puzzle
            //


            //TestPuzzle("ColorHandlePuzzleAsset", 0);
            //TestPuzzle("PicturePuzzleAsset", 0);
            //TestPuzzle("PictureHandlePuzzleAsset", 0);
            //TestPuzzle("SkeletonPuzzleAsset", 0);
            //TestPuzzle("JigSawPuzzleAsset", 0);
            //TestPuzzle("MemoryPuzzleAsset", 0);
            //TestPuzzle("FifteenPuzzleAsset", 0);
            //TestPuzzle("ArrowPuzzleAsset", 0);
            TestPuzzle("GlobeCoopPuzzleAsset", 0);
#endif

            CreateUniqueObjects();

            CreateDecals();

            

            // 
            // Set the monster spawn tile
            //
            ChooseMonsterSpawnTile();


            // 
            // Load geometry
            //
            BuildGeometry();
           
            // 
            // Create lighting
            //
            CreateLighting();

            //
            // Bake the navigation mesh
            //
            //BakeNavigationMesh();



            // 
            // Spawn monster
            //
#if !TEST_PUZZLE
            SpawnMonster();
#endif

#if TEST_PUZZLE
            //TestPuzzle(null, 1);
            Transform[] tl = FindObjectsOfType<Transform>();
            foreach(Transform t in tl)
            {
                if (t.gameObject.layer == LayerMask.NameToLayer("Wall"))
                    t.gameObject.SetActive(false);
            }
#endif
            //
            // Optimize decals
            //
            OptimizeDecals();

            Debug.LogFormat("LevelBuilder - Level built in {0} seconds.", (System.DateTime.Now - startTime).TotalSeconds);
        }

        void OptimizeDecals()
        {
            float drawDistance = 16f;
            UnityEngine.Rendering.HighDefinition.DecalProjector[] decals = FindObjectsOfType< UnityEngine.Rendering.HighDefinition.DecalProjector>();
            foreach(var decal in decals)
            {
                decal.drawDistance = drawDistance;
            }
            
        }

        void SpawnMonster()
        {
            List<MonsterAsset> assets = new List<MonsterAsset>(Resources.LoadAll<MonsterAsset>(System.IO.Path.Combine(MonsterAsset.ResourceFolder, theme.ToString()))).FindAll(a => !a.name.StartsWith("_"));
            MonsterAsset ma = assets[Random.Range(0, assets.Count)];
            Vector3 position = tiles[monsterStartingTileId].GetPosition() + 2f * ( Vector3.right + Vector3.back );

            if (SessionManager.Instance.Runner.IsServer || SessionManager.Instance.Runner.IsSharedModeMasterClient)
            {
                SessionManager.Instance.Runner.Spawn(ma.Prefab, position, Quaternion.identity, null,
                (r, o) =>
                {
                    o.GetComponent<MonsterController>().Init();
                });
            }
                
        }

        void CreateDecals() 
        {
            //// Fill the exclusion list
            //List<int> exclusionList = new List<int>();
            //foreach(CustomObject o in customObjects)
            //{
            //    exclusionList.Add(o.TileId);
            //}
            // Fill the weighted id list
            List<CustomObjectAsset> assets = new List<CustomObjectAsset>(Resources.LoadAll<CustomObjectAsset>(System.IO.Path.Combine(CustomObjectAsset.ResourceFolder, theme.ToString(), "Decals"))).FindAll(a => !a.name.StartsWith("_"));
            List<int> wIds = new List<int>();
            for(int i=0; i<assets.Count; i++)
                for(int j=0; j<assets[i].Weight; j++)
                    wIds.Add(i);
                
            // Loop through each sector
            for (int i = 0; i < sectors.Length; i++)
            {
                float min = sectors[i].TileIds.Count * .6f;
                float max = sectors[i].TileIds.Count * .8f;
                float count = Random.Range(min, max);
                //count = sectors[i].tileIds.Count * .1f; // TO REMOVE
                //int exclusionOffset = Mathf.FloorToInt(Mathf.Sqrt(sectors[i].tileIds.Count / count) - 1);

                

                for (int j = 0; j < count && assets.Count > 0; j++)
                {
                    CustomObjectAsset coa = assets[wIds[Random.Range(0, wIds.Count)]];
                    //assets.Remove(coa);
                    CustomObject co = new CustomObject(this, coa);

                    ObjectAlignment alignment = coa.Alignment;
                    if (alignment == ObjectAlignment.Both)
                        alignment = Random.Range(0, 2) == 0 ? ObjectAlignment.MiddleOnly : ObjectAlignment.SideOnly;


                    co.AttachRandomly(i, emptyTilesOnly:false, alignment);

                    if (co.TileId >= 0)
                    {
                        customObjects.Add(co);
                        //exclusionList.AddRange(GetTileIndexGroup(co.TileId, exclusionOffset));
                    }
                    else
                    {
                        Debug.LogWarningFormat("No room found for the custom object - object:{0}, tileId:{1}", co, co.TileId);
                    }

                }
            }
        }

        void CreateUniqueObjects()
        {
            
            
            for(int i=0; i<sectors.Length; i++)
            {
                List<CustomObjectAsset> assets = new List<CustomObjectAsset>(Resources.LoadAll<CustomObjectAsset>(System.IO.Path.Combine(CustomObjectAsset.ResourceFolder, theme.ToString(), "Uniques"))).FindAll(a => !a.name.StartsWith("_"));
                float min = sectors[i].TileIds.Count * .08f;
                float max = sectors[i].TileIds.Count * .12f;
                float count = Random.Range(min, max);
                //count = sectors[i].tileIds.Count * .1f; // TO REMOVE
                int exclusionOffset = Mathf.FloorToInt(Mathf.Sqrt(sectors[i].TileIds.Count / count) - 1);

                List<int> exclusionList = new List<int>();
              
                for (int j = 0; j < count && assets.Count > 0; j++)
                {
                    CustomObjectAsset coa = assets[Random.Range(0, assets.Count)];
                    assets.Remove(coa);
                    CustomObject co = new CustomObject(this, coa);

                    ObjectAlignment alignment = coa.Alignment;
                    if(alignment == ObjectAlignment.Both)
                        alignment = Random.Range(0, 2) == 0 ? ObjectAlignment.MiddleOnly : ObjectAlignment.SideOnly;
                    
                    
                    co.AttachRandomly(i, emptyTilesOnly:true, alignment, exclusionList );

                    if(co.TileId >= 0)
                    {
                        customObjects.Add(co);
                        exclusionList.AddRange(GetTileIndexGroup(co.TileId, exclusionOffset));
                    }
                    else
                    {
                        Debug.LogWarningFormat("No room found for the custom object - object:{0}, tileId:{1}", co, co.TileId);
                    }                    

                }
            }
        }

        void BuildGeometry()
        {

            // Create root
            geometryRoot = new GameObject("Geometry").transform;
            geometryRoot.position = Vector3.zero;
            geometryRoot.rotation = Quaternion.identity;

            int size = (int)Mathf.Sqrt(tiles.Length);

            // Create tiles
            for(int i=0; i<tiles.Length; i++)
            {
                
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
            int startingTileId = connections.Find(c => c.IsInitialConnection()).TargetTileId;
            int finalTileId = connections.Find(c => c.IsFinalConnection()).SourceTileId;


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
                gate.CreateSceneObject();
            }


            //
            // Check for pillars 
            //
            foreach(Tile tile in tiles)
            {
                tile.CheckForPillars();
            }


            //
            // Create puzzles object
            //
            
            foreach(Puzzle puzzle in puzzles)
            {
                // First create scene objects
                puzzle.CreateSceneObjects();
            }


            // Create all the objects that must be spawned
            if (SessionManager.Instance.Runner.IsServer || SessionManager.Instance.Runner.IsSharedModeMasterClient)
            {
                //
                // Create puzzle controllers
                //
                Player localPlayer = new List<Player>(FindObjectsOfType<Player>()).Find(p => p.HasStateAuthority);
                foreach (Puzzle puzzle in puzzles)
                {
                    puzzle.SpawnNetworkedObjects();

                }
            }
            
            // Create cosmetics
            foreach(CustomObject co in customObjects)
            {
                if (!co.SceneObject)
                    co.CreateSceneObject();
            }

            
            
        }

        void BakeNavigationMesh()
        {
            FindObjectOfType<NavMeshSurface>().BuildNavMesh();
        }

        void ConnectSectors()
        {
            if (sectors.Length == 1)
                return;

            if(sectors.Length == 2)
            {
                // Create two ordered lists of sources and targets ( src[n] => trg[n] )
                List<int> src = new List<int>();
                List<int> trg = new List<int>();
                Sector s = sectors[0];
                int size = (int)Mathf.Sqrt(tiles.Length);
                for(int i=0; i<s.TileIds.Count; i++)
                {
                    int tileId = s.TileIds[i];
                    
                    if (tileId % size == 0 || tileId % size == size - 1 || tileId / size == 0 || tileId / size == size - 1)
                        continue; // Tiles at the edge of the level

                    if(tiles[tileId - size].sectorIndex == 1) // Tile to the bottom
                    {
                        src.Add(tileId);
                        trg.Add(tileId - size);
                    }
                    else if(tiles[tileId + size].sectorIndex == 1) // Tile to the top
                    {
                        src.Add(tileId);
                        trg.Add(tileId + size);
                    }
                    else if(tiles[tileId - 1].sectorIndex == 1) // Tile to the left
                    {
                        src.Add(tileId);
                        trg.Add(tileId - 1);
                    }
                    else if(tiles[tileId + 1].sectorIndex == 1) // Tile to the right
                    {
                        src.Add(tileId);
                        trg.Add(tileId + 1);
                    }
                            
                     
                }

                // Choose a pair of src/trg
                int id = Random.Range(0, src.Count);
                int srcId = src[id];
                int trgId = trg[id];

                // We want to create a connection between the two sectors following the normal flow the player should walk
                // to find the exit ( so we want the initial sector to be the source of the connection )
                Connection initialConn = connections.Find(c => c.SourceTileId < 0);
                if(tiles[initialConn.TargetTileId].sectorIndex == 0)
                    connections.Add(Connection.CreateNormalConnection(this, srcId, trgId));
                else
                    connections.Add(Connection.CreateNormalConnection(this, trgId, srcId));
                
                

                // Set the openings
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
                
            }

            // Create a flow from initial to final sctor
            if (sectors.Length > 2)
            {
                // Create a list with all the possible sources
                List<int> srcIds = new List<int>();
                for(int i=0;i<sectors.Length; i++)
                {
                    
                    //if(!sectors[i].IsFinalSector() || sectors[i].IsInitialSector())
                    // Every sector can be a source for another sector, even the final sector.
                    srcIds.Add(i);
                }

                

                // Create a list with all the possible targets
                // Each element on this list can only be used once as target, so we can loop throughout until the list is empty
                List<int> trgIds = new List<int>();
                for (int i = 0; i < sectors.Length; i++)
                {
                    // The initial sector can not be a target because the player starts from there.
                    if(!sectors[i].IsInitialSector())
                        trgIds.Add(i);
                }


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

                        if (connections.Exists(c => c.SourceTileId >= 0 && c.TargetTileId >= 0 && tiles[c.SourceTileId].sectorIndex == src && tiles[c.TargetTileId].sectorIndex == id) ||
                            connections.Exists(c => c.SourceTileId >= 0 && c.TargetTileId >= 0 && tiles[c.SourceTileId].sectorIndex == id && tiles[c.TargetTileId].sectorIndex == src))
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
           
            if(sectors.Length > 1)
            {
                int total = connections.Count-1;
                List<Connection> orderedList = new List<Connection>();
                Connection conn = connections.Find(c => c.IsInitialConnection());
                orderedList.Add(conn);
                connections.Remove(conn);
                conn = null;
                total--;
                while (total > 0)
                {
                    foreach(Connection c in connections)
                    {
                        int srcId = c.SourceTileId;
                        if(orderedList.Exists(c2 => tiles[c2.TargetTileId].sectorIndex == tiles[srcId].sectorIndex && !c.IsFinalConnection()))
                        {
                            conn = c;
                            break;
                        }
                    }

                    orderedList.Add(conn);
                    connections.Remove(conn);
                    conn = null;
                    total--;
                    
                }

                orderedList.Add(connections[0]);// Only the final connection remains
                connections.Clear();
                connections = orderedList;
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
                    int maxWidth = Mathf.Min(3, maxTilesWidth);
                    int maxHeight = Mathf.Min(3, maxTilesHeight);

                    int minWidth = Mathf.Min(1, maxTilesWidth);
                    int minHeight = Mathf.Min(1, maxTilesHeight);

                    // We only create commons rooms for now
                    int w = Random.Range(minWidth, maxWidth + 1);
                    int h = Random.Range(minHeight, maxHeight + 1);
                   
                    if(w == 3 && h == 3)
                    {
                        if (Random.Range(0, 2) == 0)
                            w = 2;
                        else
                            h = 2;
                    }

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

                    // We also set not allowed all the adiacent tiles
                    int size = (int)Mathf.Sqrt(tiles.Count);
                    for(int i=0; i<room.tileIds.Count; i++)
                    {
                        bool isTop = i / room.width == 0;
                        bool isRight = i % room.width == room.width - 1;
                        bool isBottom = i / room.width == room.height - 1;
                        bool isLeft = i % room.width == 0;
                        int id = room.tileIds[i];
                        if (isTop && id / size > 0)
                            notAllowed.Add(id - size);
                        if (isRight && id % size < size - 1)
                            notAllowed.Add(id + 1);
                        if (isBottom && id / size < size - 1)
                            notAllowed.Add(id + size);
                        if (isLeft && id % size > 0)
                            notAllowed.Add(id - 1);
                        if (isTop && id / size > 0 && isRight && id % size < size - 1)
                            notAllowed.Add(id - size + 1);
                        if (isTop && id / size > 0 && isLeft && id % size > 0)
                            notAllowed.Add(id - size - 1);
                        if (isBottom && id / size < size - 1 && isRight && id % size < size - 1)
                            notAllowed.Add(id + size + 1);
                        if (isBottom && id / size < size - 1 && isLeft && id % size > 0)
                            notAllowed.Add(id + size - 1);
                    }

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
                else if (enteringTileIndex % size == size - 1) // Right
                    indices.RemoveAll(i => i % size == size - 1);
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

            GameManager gameManager = FindObjectOfType<GameManager>();
            switch (gameManager.LevelSize)
            {
                //case 0:
                //    sectorCount = Random.Range(1, 3);
                //    tileCount = 8 * 8;//Random.Range(30, 45);
                //    break;
                case 0:
                    sectorCount = Random.Range(1, 3);
                    sectorCount = 1;
                    //sectorCount = 3;
                    if(sectorCount == 2)
                        tileCount = 12 * 12;
                    else
                        tileCount = 9 * 9;
                    break;
                case 1:
                    sectorCount = Random.Range(2, 4);
                    sectorCount = 2;
                    if(sectorCount == 3)
                        tileCount = 16 * 16;
                    else
                        tileCount = 14 * 14;
                    break;
                case 2:
                    sectorCount = Random.Range(2, 4);
                    sectorCount = 3;
                    if(sectorCount == 3)
                        tileCount = 20 * 20;// Random.Range(70, 105);
                    else
                        tileCount = 18 * 18;// Random.Range(70, 105);
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
            puzzles.Clear();
            triggers.Clear();
        }

        void BuildSectors()
        {
            // 
            // Split tiles into sectors
            //
            switch (sectors.Length)
            {
                case 1:
                    for (int i = 0; i < tiles.Length; i++)
                        sectors[0].TileIds.Add(i);
                    break;
                case 2:
                    int cols = (int)Mathf.Sqrt(tiles.Length);
                    bool vertical = Random.Range(0, 2) == 0;
                                       
                    for(int i=0; i<tiles.Length; i++)
                    {
                        int id = 1;
                        if((vertical && i % cols < cols/2) || (!vertical && i / cols < cols / 2))
                            id = 0;

                        sectors[id].TileIds.Add(i);
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
                                
                                sectors[id].TileIds.Add(i);
                                tiles[i].sectorIndex = id;
                            }
                            break;
                        case 1: // 2 big, 1 small

                            break;

                    }
                    
                    break;
            }

            

            //
            // Set max width and max height for each sector
            //
            for (int i=0; i<sectors.Length; i++)
            {
                int size = (int)Mathf.Sqrt(tiles.Length);
                int minX = 0, maxX = 0, minZ = 0, maxZ = 0;
                Sector sector = sectors[i];
                // Get boundaries
                for(int j=0; j<sector.TileIds.Count; j++)
                {
                    int index = sector.TileIds[j];
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
            for (int i = 0; i < s1.TileIds.Count; i++)
            {
                int tileId = s1.TileIds[i];
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
            for (int i = 0; i < s1.TileIds.Count; i++)
            {
                int tileId = s1.TileIds[i];
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

          
        List<int> GetTilesForRoom(int sectorIndex, int width, int height, List<int> notAllowedTileIds)
        {
            int[] tmp = new int[sectors[sectorIndex].TileIds.Count];
            new List<int>(sectors[sectorIndex].TileIds).CopyTo(tmp);
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
                    if (!sectors[sectorIndex].TileIds.Contains(id))
                        break;
                        
                    if (notAllowedTileIds.Contains(id))
                        break;

                    if ((origin / size) + (i / width) != id / size)
                        break;

                    // If one of the corners of the candidate room falls in a north or south tile connection then break
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

        void ChooseMonsterSpawnTile()
        {
            
            // Get the starting tile
            Connection startConnection = connections.Find(c => c.IsInitialConnection());
            int tileId = startConnection.TargetTileId;

#if UNITY_EDITOR || TEST
            //monsterStartingTileId = tileId;
            //return;
#endif

            // Get the initial sector
            Sector sector = sectors[tiles[tileId].sectorIndex];

            int size = sector.width < sector.height ? sector.width : sector.height;
            size -= 2;
            List<int> notAvailables = GetTileIndexGroup(tileId, size);
            List<int> availables = new List<int>();
            foreach(int tid in sector.TileIds)
            {
                if (notAvailables.Contains(tid))
                    continue;
                if (customObjects.Exists(o => o.TileId == tid && o.Direction == Vector3.zero))
                    continue;
                if (tiles[tid].unreachable)
                    continue;

                availables.Add(tid);
            }

            monsterStartingTileId = availables[Random.Range(0, availables.Count)];
      
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

                int tileId = conn.SourceTileId;

                // Create the new gate 
                Gate co = new Gate(this, gateAssets[Random.Range(0, gateAssets.Count)]);
                customObjects.Add(co);
                co.Direction = tiles[tileId].openDirection;
                co.TileId = tileId;
                conn.gateIndex = customObjects.Count - 1;

            }
        }

        void CreatePuzzles()
        {
            
            List<PuzzleAsset> puzzleCollection = new List<PuzzleAsset>(Resources.LoadAll<PuzzleAsset>(System.IO.Path.Combine(PuzzleAsset.ResourceFolder, theme.ToString()))).FindAll(p=>!p.name.ToLower().StartsWith("_"));
            if (SessionManager.Instance.Runner.IsSinglePlayer)
            {
                // Remove coop only puzzles if we are playing in singleplayer
                puzzleCollection.RemoveAll(p => p.CoopOnly);
            }
           


            // Get all gates
            List<CustomObject> gates = customObjects.FindAll(g => g.GetType() == typeof(Gate));
            
            // In the target sector of the current connection we have the puzzle to open the next connection in the list
            for(int i=0; i<connections.Count-1; i++)
            {
                // The connection source tile must be in the sector this puzzle belongs to
                int trgId = connections[i].TargetTileId;
                int sectorId = tiles[trgId].sectorIndex;
                int gateId = connections[i+1].gateIndex; // The gate of the next connection in the list

                // Get a random puzzle
                PuzzleAsset asset = puzzleCollection[Random.Range(0, puzzleCollection.Count)];
#if UNITY_EDITOR
                //asset = puzzleCollection.Find(p => p.name.StartsWith("JigSaw"));
#endif
                // Build the puzzle
                Puzzle puzzle = PuzzleFactory.CreatePuzzle(this, asset, sectorId);
                puzzles.Add(puzzle);
                (customObjects[gateId] as Gate).PuzzleIndex = puzzles.Count - 1;
            }
            
        }

        public void Build(int seed)
        {
#if UNITY_EDITOR
            //seed = 1481849213;
            //seed = -440185720;
            //seed = -511521315;
#endif

            Random.InitState(seed);

            Debug.LogFormat("Building level using seed: {0}", seed);

            

            Create();

            OnLevelBuilt?.Invoke();
        }

        public Puzzle GetPuzzle(int puzzleId)
        {
            return puzzles[puzzleId];
        }

        public List<int> GetTileIndexGroup(int center, int offset)
        {
            List<int> indices = new List<int>();

            int size = (int)Mathf.Sqrt(tiles.Length);

            int count = offset * 2 + 1;
            for (int k = 0; k < count; k++)
            {
                int r = (k - (count - 1) / 2) * size;
                if (center + r > tiles.Length - 1 || center + r < 0)
                    continue;
                for (int j = 0; j < count; j++)
                {
                    int c = j - (count - 1) / 2;
                    if ((c < 0 && center % size < (center + c) % size) || (c > 0 && center % size > (center + c) % size))
                        continue;
                    indices.Add(center + r + c);

                }
            }


            return indices;
        }

        public Puzzle GetNextPuzzleToSolve()
        {
            List<PuzzleController> puzzleControllers = new List<PuzzleController>(FindObjectsOfType<PuzzleController>());

            for(int i=0; i<puzzles.Count; i++)
            {
                Puzzle puzzle = puzzles[i];
                PuzzleController pc = puzzleControllers.Find(p => p.PuzzleIndex == i && !p.Solved);
                if (pc)
                    return puzzle;
            }
            return null;
        }

        public Puzzle GetLastSolvedPuzzle()
        {
            List<PuzzleController> puzzleControllers = new List<PuzzleController>(FindObjectsOfType<PuzzleController>());

            for (int i = 0; i < puzzles.Count; i++)
            {
                int id = puzzles.Count - 1 - i;
                Puzzle puzzle = puzzles[id];
                PuzzleController pc = puzzleControllers.Find(p => p.PuzzleIndex == id && p.Solved);
                if (pc)
                    return puzzle;
            }
            return null;
        }

        public Sector GetCurrentPlayingSector()
        {
            Puzzle lastPuzzle = GetLastSolvedPuzzle();
            Tile tile = null;

            if (lastPuzzle == null)
            {
                // Get the first sector
                Connection c = new List<Connection>(Connections).Find(c => c.IsInitialConnection());
                tile = GetTile(c.TargetTileId);
            }
            else
            {
                int puzzleId = GetPuzzleId(lastPuzzle);
                int gateIndex = new List<CustomObject>(CustomObjects).FindIndex(g => g.GetType() == typeof(Gate) && (g as Gate).PuzzleIndex == puzzleId);
                Connection c = new List<Connection>(Connections).Find(c => c.gateIndex == gateIndex);
                // Get the target tile of the connection or the source if the target is -1 ( it happens with the last puzzle )
                tile = c.TargetTileId < 0 ? GetTile(c.SourceTileId) : tile = GetTile(c.TargetTileId); ;

            }

            return GetSector(tile.sectorIndex);
        }

        public int GetPuzzleId(Puzzle puzzle)
        {
            return puzzles.IndexOf(puzzle);
        }

        //public Tile GetTileByPosition(Vector3 position)
        //{
        //    float h = position.x / TileSize;
        //    float v = position.z / TileSize;

        //    return tiles[(int)h * (int)v];
        //}

        public Tile GetTile(int tileId)
        {
            return tiles[tileId];
        }

        public bool TryGetTileIdByWorldPosition(Vector3 position, out int tileId)
        {
            tileId = -1;
            int sizeSqrt = (int)Mathf.Sqrt(tiles.Length);
            if (position.x > 0f && position.x < TileSize * sizeSqrt && position.z < 0f && position.z > -TileSize * sizeSqrt)
            {
                // Get the tile id
                int h = (int)(position.x / TileSize);
                int v = (int)(-position.z / TileSize);
                tileId = h + v * sizeSqrt;
                return true;
            }

            return false;
        }
       
        /// <summary>
        /// Return the closest tile to the given position; is the position falls into the enter or exit room then the 
        /// corresponding enter or exit tile is returned ( in the boundaries ).
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        //public int GetTheClosestTileId(Vector3 position)
        //{
        //    int tileId = -1;
        //    if (TryGetTileIdByWorldPosition(position, out tileId))
        //    {
        //        Debug.Log($"Player died inside the labirinth - tile:{tileId}");
        //    }
        //    else
        //    {
        //        // Get the closest between the enter and exit tiles   
        //        int enterTileId = connections.Find(c => c.IsInitialConnection()).TargetTileId;
        //        int exitTileId = connections.Find(c => c.IsFinalConnection()).SourceTileId;
        //        Tile enterTile = tiles[enterTileId];
        //        Tile exitTile = tiles[exitTileId];
        //        if (Vector3.Distance(position, enterTile.GetPosition()) < Vector3.Distance(position, exitTile.GetPosition()))
        //        {
        //            // Enter is closer
        //            Debug.Log($"Player died near the entrance");
        //            tileId = enterTileId;
        //        }
        //        else
        //        {
        //            // Exit is closer
        //            Debug.Log($"Player died near the exit");
        //            tileId = exitTileId;
        //        }
        //    }
        //    return tileId;
        //}
       
        public Sector GetSector(int id)
        {
            return sectors[id];
        }

   

        //public Sector GetSectorByPuzzle()
        //{
        //    return sectors[0];
        //}

#if TEST_PUZZLE
        void TestPuzzle(string puzzleAssetName, int step)
        {
            

            if (step == 0)
            {
                List<PuzzleAsset> puzzleCollection = new List<PuzzleAsset>(Resources.LoadAll<PuzzleAsset>(System.IO.Path.Combine(PuzzleAsset.ResourceFolder, theme.ToString()))).FindAll(p => !p.name.ToLower().StartsWith("_"));

                // Get all gates
                List<CustomObject> gates = customObjects.FindAll(g => g.GetType() == typeof(Gate));

                // In the target sector of the current connection we have the puzzle to open the next connection in the list
                for (int i = 0; i < connections.Count - 1; i++)
                {
                    // The connection source tile must be in the sector this puzzle belongs to
                    int trgId = connections[i].TargetTileId;
                    int sectorId = tiles[trgId].sectorIndex;
                    int gateId = connections[i + 1].gateIndex; // The gate of the next connection in the list

                    // Get a random puzzle
                    PuzzleAsset asset = puzzleCollection.Find(p => p.name.ToLower().Equals(puzzleAssetName.ToLower()));
                    // Build the puzzle
                    Puzzle puzzle = PuzzleFactory.CreatePuzzle(this, asset, sectorId);
                    puzzles.Add(puzzle);
                    (customObjects[gateId] as Gate).PuzzleIndex = puzzles.Count - 1;
                }
            }
            else
            {
                Puzzle puzzle = puzzles[0];
                GameObject sp = GameObject.FindGameObjectWithTag("PlayerSpawnPoint");

                if(puzzle.GetType() == typeof(PicturePuzzle))
                {
                    float rOff = 0f;
                    CustomObject obj = customObjects[(puzzle as PicturePuzzle).PictureId];
                    obj.SceneObject.transform.position = sp.transform.position + sp.transform.forward * 7f + sp.transform.right * rOff;
                    obj.SceneObject.transform.forward = -sp.transform.forward;
                    rOff += -4f;
                        foreach (int id in (puzzle as PicturePuzzle).PieceIds)
                        {
                            obj = customObjects[id];
                            Picker picker = new List<Picker>(FindObjectsOfType<Picker>()).Find(p => p.CustomObjectId == id);
                            picker.transform.position = sp.transform.position + sp.transform.forward * 7f + sp.transform.right * rOff;
                            obj.SceneObject.transform.position = picker.transform.position;
                            rOff += 4f;
                        }
                    
                }

                if (puzzle.GetType() == typeof(HandlesPuzzle))
                {
                    float rOff = 0f;
                    for(int i=0; i< (puzzle as HandlesPuzzle).Handles.Count; i++)
                    {
                        CustomObject obj = customObjects[(puzzle as HandlesPuzzle).Handles[i].CustomObjectId];
                        obj.SceneObject.transform.position = sp.transform.position + sp.transform.forward * 12f + sp.transform.right * rOff;
                        rOff += 4f;
                    }
                    
                    
                }

                // Remove all the walls
                foreach (Tile tile in tiles)
                {
                    Transform wall = new List<Transform>(tile.sceneObject.GetComponentsInChildren<Transform>()).Find(t => t.name.ToLower().Equals("pv"));
                    if (wall)
                        Destroy(wall.gameObject);

                }
            }
        }
#endif
    }


}
