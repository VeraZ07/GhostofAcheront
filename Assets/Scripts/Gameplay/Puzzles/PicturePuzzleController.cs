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
        NetworkLinkedList<int> Pieces { get; } = default;

        GameObject pictureObject;
        List<GameObject> piecesObjects;

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

            // Get the picture object
            pictureObject = builder.CustomObjects[puzzle.PictureId].SceneObject;

            // Get all the pieces
            piecesObjects = new List<GameObject>();
            foreach(int pieceId in puzzle.PieceIds)
            {
                piecesObjects.Add(builder.CustomObjects[pieceId].SceneObject);
            }

            // Set network states
            for(int i=0; i<piecesObjects.Count; i++)
            {
                Pieces.Add(-1); // Empty
            }

      
        }
    }

}
