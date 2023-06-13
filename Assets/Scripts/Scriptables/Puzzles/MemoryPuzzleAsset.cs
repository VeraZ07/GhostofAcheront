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
        CustomObjectAsset frame;
        public CustomObjectAsset Frame
        {
            get { return frame; }
        }

        [SerializeField]
        int framesCount = 1;
        public int FramesCount
        {
            get { return framesCount; }
        }
    }

}
