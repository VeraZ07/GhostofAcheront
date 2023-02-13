using Fusion;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GOA.Level.LevelBuilder;

namespace GOA
{
    /// <summary>
    /// Manage a combination of objects ( for example levers ).
    /// </summary>
    public class MultiStatePuzzleController : PuzzleController
    {
        [UnitySerializeField]
        [Capacity(10)]
        [Networked(OnChanged = nameof(OnStateArrayChanged))] 
        NetworkArray<int> StateArray { get; } = default;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Spawned()
        {
            base.Spawned();

            if (!Runner.IsHostMigrationEnabled)
            {
                //if (Runner.IsServer)
                //{
                //    // Attach the puzzle previously created by the builder
                //    LevelBuilder builder = FindObjectOfType<LevelBuilder>();

                //    // Get the puzzle and set all the states
                //    MultiStatePuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as MultiStatePuzzle;

                //    List<int> objIds = new List<int>(puzzle.ElementsIds);
                //    foreach(int id in objIds)
                //    {
                        
                //    }
                //}

            }
            else
            {
                // Resume state controllers from the builder
            }
        }

        public static void OnStateArrayChanged(Changed<MultiStatePuzzleController> changed)
        {
            Debug.LogFormat("OnSolvedChanged:{0}", changed.Behaviour.StateArray);
            //OnSolvedChangedCallback?.Invoke(changed.Behaviour);
        }

        public override void Initialize()
        {
            // Attach the puzzle previously created by the builder
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();

            // Get the puzzle and set all the states
            MultiStatePuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as MultiStatePuzzle;

            List<int> objIds = new List<int>(puzzle.ElementsIds);
            //StateArray = new NetworkArray<int>[objIds.Count];
            for(int i=0; i<objIds.Count; i++)
            {
                StateArray.Set(i, 1);
            }


        }
    }

}
