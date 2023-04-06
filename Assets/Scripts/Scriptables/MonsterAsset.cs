using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    public class MonsterAsset : ScriptableObject
    {
        public const string ResourceFolder = "Monsters";

        [SerializeField]
        NetworkObject prefab;
        public NetworkObject Prefab
        {
            get { return prefab; }
        }
    }
}

