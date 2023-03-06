using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
  
    public class MultiStatePuzzleAsset: PuzzleAsset
    {
        [SerializeField]
        CustomObjectAsset elementAsset;

        public CustomObjectAsset ElementAsset
        {
            get { return elementAsset; }
        }
    }

}
