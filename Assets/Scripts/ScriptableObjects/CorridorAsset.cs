using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    public class CorridorAsset : ScriptableObject
    {
        public const string ResourceFolder = "Corridors";

        [SerializeField]
        int connectionCount;

        [SerializeField]
        Vector2 size = Vector2.one;
    }

}
