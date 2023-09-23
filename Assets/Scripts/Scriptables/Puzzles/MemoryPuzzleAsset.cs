using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    public class MemoryPuzzleAsset : PuzzleAsset
    {
        //[SerializeField]
        //List<CustomObjectAsset> tiles;
        //public List<CustomObjectAsset> Tiles
        //{
        //    get { return tiles; }
        //}

        [SerializeField]
        bool shuffleTiles = false;
        public bool ShuffleTiles
        {
            get { return shuffleTiles; }
        }

        [SerializeField]
        CustomObjectAsset frame;
        public CustomObjectAsset Frame
        {
            get { return frame; }
        }

        [SerializeField]
        int frameCount = 1;
        public int FrameCount
        {
            get { return frameCount; }
        }
    }

}
