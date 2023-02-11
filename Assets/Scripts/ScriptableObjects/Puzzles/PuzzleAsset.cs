using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    [System.Serializable]
    public abstract class PuzzleAsset : ScriptableObject
    {
        public const string ResourceFolder = "Puzzles";

        //[SerializeField]
        //[Range(1,4)]
        //int minimumPlayers = 1;

        [SerializeField]
        GameObject prefab; // The scene object prefab

        public GameObject Prefab
        {
            get { return prefab; }
        }
    }


}
