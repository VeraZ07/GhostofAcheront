using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOA.Assets;
using Fusion;

namespace GOA.Level
{
    public class LevelBuilder: MonoBehaviour
    {

        List<Room> rooms = new List<Room>();
        List<Corridor> corridors = new List<Corridor>();
        List<Connection> connections = new List<Connection>();

        List<RoomAsset> roomAssetList = new List<RoomAsset>();
        List<CorridorAsset> corridorAssetList = new List<CorridorAsset>();

        private void Awake()
        {
            
        }

        private void Start()
        {
            if(FindObjectOfType<NetworkRunner>().IsServer)
                CreateLevel(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levelSize">0..N from small to large.</param>
        public void CreateLevel(int levelSize)
        {
            // Load assets
            roomAssetList = new List<RoomAsset>(Resources.LoadAll<RoomAsset>(RoomAsset.ResourceFolder));
            corridorAssetList = new List<CorridorAsset>(Resources.LoadAll<CorridorAsset>(CorridorAsset.ResourceFolder));

            int minRooms = 0;
            switch (levelSize)
            {
                case 0:
                    minRooms = 10;
                    break;
            }


        }

    }

}
