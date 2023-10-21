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
        bool coopOnly = false;
        public bool CoopOnly
        {
            get { return coopOnly; }
        }

        //[SerializeField]
        //bool threePlayersNeeded = false;
        //public bool ThreePlayerNeeded
        //{
        //    get { return threePlayersNeeded; }
        //}

        //[SerializeField]
        //bool fourPlayersNeeded = false;
        //public bool FourPlayerNeeded
        //{
        //    get { return fourPlayersNeeded; }
        //}

        [SerializeField]
        GameObject controllerPrefab; // The scene object prefab

        public GameObject ControllerPrefab
        {
            get { return controllerPrefab; }
        }

        //public bool IsCoop()
        //{
        //    return twoPlayersNeeded || threePlayersNeeded || fourPlayersNeeded;
        //}
    }


}
