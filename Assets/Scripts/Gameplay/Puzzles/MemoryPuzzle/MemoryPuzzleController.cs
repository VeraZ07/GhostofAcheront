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
            List<MemoryTileInteractor> tiles = new List<MemoryTileInteractor>();

            public Frame(List<MemoryTileInteractor> tiles)
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

                List<MemoryTileInteractor> tiles = new List<MemoryTileInteractor>(so.GetComponentsInChildren<MemoryTileInteractor>());
                frames.Add(new Frame(tiles));

                // Init tiles
                for(int j=0; j<tiles.Count; j++)
                {
                    tiles[j].Init(this, i, j);
                }
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

       
        public bool IsTileVisible(int frameId, int tileId)
        {
            return NetworkFrames[frameId].Tiles[tileId];
        }

        public void SetTileVisible(int frameId, int tileId, bool value)
        {
            var tiles = NetworkFrames[frameId].Tiles;
            tiles[tileId] = value;
        }

        #region fusion callbacks
        public static void OnNetworkFramesChanged(Changed<MemoryPuzzleController> changed)
        {
            Debug.Log("networkframes changed");

            // Show and hide tiles

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


