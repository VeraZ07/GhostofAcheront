using Fusion;
using GOA.Interfaces;
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
    public class HandlesPuzzleController : PuzzleController
    {
        #region fields   
        [UnitySerializeField]
        [Capacity(10)]
        [Networked(OnChanged = nameof(OnStatesChanged))]
        NetworkLinkedList<byte> States { get; } = default;

        List<GameObject> handles = new List<GameObject>();

        #endregion

        #region native methods
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion

        #region fusion native
        public override void Spawned()
        {
            base.Spawned();

            // Attach all the objects
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            HandlesPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as HandlesPuzzle;
            
            foreach(HandlesPuzzle.Handle handle in puzzle.Handles)
            {
                CustomObject co = builder.CustomObjects[handle.Id];
                handles.Add(co.SceneObject);
                co.SceneObject.GetComponent<IHandleController>().Init(handle.InitialState, handle.StateCount);
            }
        }
        #endregion

        #region override
        public override void Initialize(int puzzleIndex)
        {
            
            // Attach the puzzle previously created by the builder
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();

            // Get the puzzle and set all the states
            HandlesPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as HandlesPuzzle;

            List<int> objIds = new List<int>(puzzle.Handles.Count);
            Debug.LogFormat("[HandlePuzzle - Initialize - Handle.Count:{0}]", puzzle.Handles.Count);
            
            for(int i=0; i< puzzle.Handles.Count; i++)
            {
                States.Add((byte) new List<HandlesPuzzle.Handle>(puzzle.Handles)[i].InitialState);
            }
        }
        #endregion

        #region fusion callbacks
        public static void OnStatesChanged(Changed<HandlesPuzzleController> changed)
        {
            bool solved = true;
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            HandlesPuzzle puzzle = builder.GetPuzzle(changed.Behaviour.PuzzleIndex) as HandlesPuzzle;
            List<HandlesPuzzle.Handle> handles = new List<HandlesPuzzle.Handle>(puzzle.Handles);
            for(int i=0; i<changed.Behaviour.States.Count; i++)
            {
                int state = (int)changed.Behaviour.States[i];
                if (state != handles[i].FinalState)
                    solved = false;
            }

            if (solved)
                changed.Behaviour.Solved = true;
        }
        #endregion
    }

}
