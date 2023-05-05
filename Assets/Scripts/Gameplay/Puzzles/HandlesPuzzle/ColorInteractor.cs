using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GOA.Level.LevelBuilder;

namespace GOA
{
    public class ColorInteractor : HandleInteractor
    {
        [SerializeField]
        List<Color> colors;



        private void Start()
        {
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            Puzzle puzzle = builder.GetPuzzle(PuzzleController.PuzzleIndex);
            
        }

        public override IEnumerator DoMoveImpl(int oldState, int newState)
        {
            throw new System.NotImplementedException();
        }

        public override void Init(int state)
        {
            throw new System.NotImplementedException();
        }
    }

}
