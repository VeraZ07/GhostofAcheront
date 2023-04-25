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

        [Networked]
        [Capacity(10)]
        NetworkLinkedList<NetworkBool> BusyList { get; } = default; 

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
            
            for(int i=0; i<puzzle.Handles.Count; i++)
            {
                HandlesPuzzle.Handle handle = puzzle.Handles[i];
                CustomObject co = builder.CustomObjects[handle.CustomObjectId];
                IHandleController hc = co.SceneObject.GetComponentInChildren<IHandleController>();
                handles.Add((hc as MonoBehaviour).gameObject);
                hc.Init(this, i);
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

            Debug.LogFormat("[HandlePuzzle - Initialize - Handle.Count:{0}]", puzzle.Handles.Count);
            
            for(int i=0; i< puzzle.Handles.Count; i++)
            {
                States.Add((byte) puzzle.Handles[i].InitialState);
                BusyList.Add(false);
            }
        }
        #endregion

        #region fusion callbacks
        public static void OnStatesChanged(Changed<HandlesPuzzleController> changed)
        {
            bool solved = true;
            
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            HandlesPuzzle puzzle = builder.GetPuzzle(changed.Behaviour.PuzzleIndex) as HandlesPuzzle;
            List<HandlesPuzzle.Handle> pHandles = new List<HandlesPuzzle.Handle>(puzzle.Handles);
            for(int i=0; i<changed.Behaviour.States.Count; i++)
            {
                changed.LoadOld();
                if (changed.Behaviour.States.Count == 0)
                    return;

                int oldState = changed.Behaviour.States[i];
                changed.LoadNew();
                int state = (int)changed.Behaviour.States[i];
                if (state != pHandles[i].FinalState)
                    solved = false;

                if(oldState != state)
                {
                    changed.Behaviour.handles[i].GetComponent<IHandleController>().Move();
                }
            }

            if (solved)
                changed.Behaviour.Solved = true;
        }
        #endregion

        #region public methods
        public bool HandleIsBusy(int handleId)
        {
            return BusyList[handleId];
        }

               
        public void HandleSetBusy(int handleId, bool value)
        {
            var busyList = BusyList;
            busyList[handleId] = value;
            
        }


        public void HandleSwitchState(int handleId)
        {
            HandlesPuzzle puzzle = FindObjectOfType<LevelBuilder>().GetPuzzle(PuzzleIndex) as HandlesPuzzle;
            int currentState = States[handleId];
            if (puzzle.StopHandleOnFinalState && currentState == puzzle.Handles[handleId].FinalState)
                return;

            currentState++;
            if (currentState >= puzzle.Handles[handleId].StateCount)
                currentState = 0;
            else if (currentState < 0)
                currentState = puzzle.Handles[handleId].StateCount - 1;
         
            var states = States;
            states[handleId] = (byte)currentState;

        }

        public bool HandleIsBlocked(int handleId)
        {
            HandlesPuzzle puzzle = FindObjectOfType<LevelBuilder>().GetPuzzle(PuzzleIndex) as HandlesPuzzle;
            int currentState = States[handleId];
            if (puzzle.StopHandleOnFinalState && currentState == puzzle.Handles[handleId].FinalState)
                return true;
            else 
                return false;
        }

        public int HandleGetState(int handleId)
        {
            return States[handleId];
        }
        #endregion
    }

}
