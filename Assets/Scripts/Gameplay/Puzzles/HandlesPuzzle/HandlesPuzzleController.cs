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
    public class HandlesPuzzleController : PuzzleController, IHandleManager
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
        GameObject clueHandle = null;
       
        #endregion

        #region native methods
       
        #endregion

        #region fusion native
        public override void Spawned()
        {
            base.Spawned();

            // Attach all the objects
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            HandlesPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as HandlesPuzzle;

            // Create the clue handle if needed ( the clue handle is just a fake handle with interaction disabled )
            if (puzzle.UseClueHandle)
            {
                HandlesPuzzle.Handle ch = puzzle.ClueHandle;
                clueHandle = builder.CustomObjects[ch.CustomObjectId].SceneObject;
                clueHandle.GetComponentInChildren<HandleInteractor>().Init(this, -1, ch.InitialState, ch.FinalState, 1, true);
            }

            // Create handles
            for(int i=0; i<puzzle.Handles.Count; i++)
            {
                HandlesPuzzle.Handle handle = puzzle.Handles[i];
                CustomObject co = builder.CustomObjects[handle.CustomObjectId];
                IHandle hc = co.SceneObject.GetComponentInChildren<IHandle>();
                handles.Add((hc as MonoBehaviour).gameObject);
                hc.Init(this, i, handle.InitialState, handle.FinalState, handle.StateCount, puzzle.StopHandleOnFinalState);
            }
        }
        #endregion

        #region override
        public override void Initialize(int puzzleIndex)
        {
            base.Initialize(puzzleIndex);

            // Attach the puzzle previously created by the builder
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();

            // Get the puzzle and set all the states
            HandlesPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as HandlesPuzzle;
            Debug.Log("Init puzzle:" + puzzleIndex);
            Debug.LogFormat("Puzzle - name:{0}, id:{1}", puzzle.Asset.name, PuzzleIndex);
            Debug.LogFormat("[HandlePuzzle - Initialize - Handle.Count:{0}]", puzzle.Handles.Count);
            
            for(int i=0; i< puzzle.Handles.Count; i++)
            {
                States.Add((byte) puzzle.Handles[i].InitialState);
                BusyList.Add(false);
            }
        }
        #endregion

        #region ihandlemanager implementation
        public void SetHandleBusy(int handleId, bool value)
        {
            var busyList = BusyList;
            busyList[handleId] = value;
        }

        public bool IsHandleBusy(int handleId)
        {
            return BusyList[handleId];
        }

        public int GetHandleState(int handleId)
        {
            return States[handleId];
        }

        public void SetHandleState(int handleId, int value)
        {
            var states = States;
            states[handleId] = (byte)value;
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
                    changed.Behaviour.handles[i].GetComponent<IHandle>().Move(oldState, state);
                }
            }

            if (solved)
                changed.Behaviour.Solved = true;
        }
        #endregion

   

    }

}
