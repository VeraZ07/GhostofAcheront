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
        public struct FrameStruct: INetworkStruct
        {
            [UnitySerializeField][Networked] public int FrameId { get; set; }
        }

        [Networked][Capacity(5)] public NetworkArray<FrameStruct> Frames { get; }


        //List<IInteractable> tiles = new List<IInteractable>();

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
            //GameObject sceneObject = 
            //tiles = new List<IInteractable>(GetComponentsInChildren<IInteractable>());
        }

        public override void Initialize(int puzzleIndex)
        {
            base.Initialize(puzzleIndex);

            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            LevelBuilder.MemoryPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as LevelBuilder.MemoryPuzzle;

            for (int i = 0; i < puzzle.FrameIds.Count; i++)
            {
                FrameStruct fs = new FrameStruct();
                fs.FrameId = puzzle.FrameIds[i];
                Frames.Set(i, fs);
            }

        }


        #region fusion callbacks
        public static void OnFlippedTilesChanged(Changed<MemoryPuzzleController> changed)
        {
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


