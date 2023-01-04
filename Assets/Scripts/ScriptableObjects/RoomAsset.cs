using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    
    public class RoomAsset : ScriptableObject
    {
        public const string ResourceFolder = "Rooms";

        [SerializeField]
        int connectionCount;

        [SerializeField]
        Vector2 size = Vector2.one;

    }

}
