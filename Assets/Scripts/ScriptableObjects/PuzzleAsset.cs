using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    public class PuzzleAsset : ScriptableObject
    {
        public const string ResourceFolder = "Puzzles";

        //[SerializeField]
        //[Range(1,4)]
        //int minimumPlayers = 1;

        [SerializeField]
        GameObject prefab;

        public GameObject Prefab
        {
            get { return prefab; }
        }
    }

}
