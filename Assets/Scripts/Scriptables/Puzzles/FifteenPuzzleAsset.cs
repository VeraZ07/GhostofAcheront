using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    public class FifteenPuzzleAsset : PuzzleAsset
    {
        [SerializeField]
        int frameCount;
        public int FrameCount
        {
            get { return frameCount; }
        }

        [SerializeField]
        CustomObjectAsset frame;
        public CustomObjectAsset Frame
        {
            get { return frame; }
        }
    }

}
