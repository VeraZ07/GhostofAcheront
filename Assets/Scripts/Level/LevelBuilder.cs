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
            public string code = "--";
            public int sectorIndex;

            public override string ToString()
            {
                return string.Format(" {0}:{1} ", sectorIndex, code);
            }
        }

        [System.Serializable]
        class Sector
        {
            [SerializeField]
            public List<int> tileIds = new List<int>();

        }

        Tile[] tiles;
        
        Sector[] sectors;
        

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
            // Distribute tiles among sectors
            //
            ShapeSectors();


            //
            // Build sectors
            //
            BuildSectors();



            DebugTiles();
      
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

                if(!top && !left && !right && !bottom)
                {
                    tiles[tileId].code = "M-";
                }
                else
                {
                    string code = "--";
                    if (top)
                        code = "T" + code.Substring(1);
                    else if(bottom)
                        code = "B" + code.Substring(1);

                    if(left)
                        code = code.Substring(0,1) + "L";
                    else if (right)
                        code = code.Substring(0, 1) + "R";

                    tiles[tileId].code = code;
                }
            }
        }

        void Init()
        {
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
