using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    public class GlobeCoopPuzzleAsset : PuzzleAsset
    {
        [SerializeField]
        CustomObjectAsset globeAsset;
        public CustomObjectAsset GlobeAsset
        {
            get { return globeAsset; }
        }


    }

}
