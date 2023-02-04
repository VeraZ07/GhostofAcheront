using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{
    public partial class LevelBuilder : MonoBehaviour
    {
        

        class Puzzle
        {
            PuzzleAsset asset;

            public Puzzle(PuzzleAsset asset)
            {
                this.asset = asset;
            }
        }
    }

}
