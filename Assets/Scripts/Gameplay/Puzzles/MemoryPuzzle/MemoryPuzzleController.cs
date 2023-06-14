using Fusion;
using GOA.Interfaces;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GOA
{
    public class MemoryPuzzleController : PuzzleController
    {
        [System.Serializable]
        public struct NetworkFrameStruct: INetworkStruct
        {
            //[UnitySerializeField][Networked] public int FrameIndex { get; set; }

            // False if the tile is hidden, otherwise true
            [UnitySerializeField] [Networked] [Capacity(16)] public NetworkLinkedList<NetworkBool> Tiles { get; }

            
        }


        [UnitySerializeField][Networked(OnChanged = nameof(OnNetworkFramesChanged))] [Capacity(5)] public NetworkLinkedList<NetworkFrameStruct> NetworkFrames { get; } = default;


        [System.Serializable]
        public class Frame
        {
            [SerializeField]
            List<IInteractable> tiles = new List<IInteractable>();

            public Frame(List<IInteractable> tiles)
            {
                this.tiles = tiles;
                Debug.Log("Tiles.Count:" + tiles.Count);
            }
        }

        [SerializeField]
        List<Frame> frames = new List<Frame>();

        //[UnitySerializeField]
        //[Networked(OnChanged = nameof(OnFlippedTilesChanged))]
        //[Capacity(10)]
        //public NetworkLinkedList<NetworkBool> FlippedTiles { get; } = default;


        public override void Spawned()
        {
            base.Spawned();

            // Get all the tiles
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            LevelBuilder.MemoryPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as LevelBuilder.MemoryPuzzle;

            for(int i=0; i<puzzle.FrameIds.Count; i++)
            {
                GameObject so = builder.CustomObjects[puzzle.FrameIds[i]].SceneObject;

                List<IInteractable> tiles = new List<IInteractable>(so.GetComponentsInChildren<IInteractable>());
                frames.Add(new Frame(tiles));
            }

            //GameObject sceneObject = puzzle.
            //
        }

        public override void Initialize(int puzzleIndex)
        {
            base.Initialize(puzzleIndex);

            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            LevelBuilder.MemoryPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as LevelBuilder.MemoryPuzzle;
            int tilesCount = puzzle.TilesCount;
            for (int i = 0; i < puzzle.FrameIds.Count; i++)
            {
                NetworkFrameStruct fs = new NetworkFrameStruct();
                //fs.FrameIndex = puzzle.FrameIds[i];
                for (int j = 0; j < tilesCount; j++)
                {
                    fs.Tiles.Add(false);
                }

                NetworkFrames.Add(fs);
            }

        }


        #region fusion callbacks
        public static void OnNetworkFramesChanged(Changed<MemoryPuzzleController> changed)
        {
            Debug.Log("networkframes changed");

            //bool solved = true;
            //for (int i = 0; i < changed.Behaviour.Pieces.Count; i++)
            //{
            //    changed.LoadOld();
            //    if (changed.Behaviour.Pieces.Count == 0)
            //        return;
            //    bool oldValue = changed.Behaviour.Pieces[i];
            //    changed.LoadNew();
            //    bool newValue = changed.Behaviour.Pieces[i];

            //    if (!newValue)
            //        solved = false;
            //    else
            //    {
            //        GameObject placeHolder = changed.Behaviour.placeHolders[i];
            //        placeHolder.SetActive(true);
            //    }


            //}

            //if (solved)
            //    changed.Behaviour.Solved = true;


        }
        #endregion
    }
}


