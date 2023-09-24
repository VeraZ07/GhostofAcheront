using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    public class ArrowPuzzleAsset : PuzzleAsset
    {


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