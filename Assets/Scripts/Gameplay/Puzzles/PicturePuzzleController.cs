using Fusion;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GOA.Level.LevelBuilder;

namespace GOA
{
    public class PicturePuzzleController : PuzzleController
    {
        [UnitySerializeField]
        [Networked]
        [Capacity(10)]
        public NetworkLinkedList<int> Pieces { get; } = default;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Initialize()
        {
            // Get the builder
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();

            // Get the puzzle
            PicturePuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as PicturePuzzle;

            // Set network states to synch
            for(int i=0; i<puzzle.PieceIds.Count; i++)
            {
                Pieces.Add(-1); // Empty
            }

      
        }
    }

}
