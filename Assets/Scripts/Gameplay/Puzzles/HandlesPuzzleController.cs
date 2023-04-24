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
            
            foreach(HandlesPuzzle.Handle handle in puzzle.Handles)
            {
                CustomObject co = builder.CustomObjects[handle.Id];
                handles.Add(co.SceneObject);
                co.SceneObject.GetComponent<IHandleController>().Init(this);
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

        public int HandleGetId(GameObject handle)
        {
            return handles.IndexOf(handle);
        }
        
        public void HandleSetBusy(int handleId, bool value)
        {
            var busyList = BusyList;
            busyList[handleId] = value;
            
        }

        public void HandleSetState(int handleId, int state)
        {
            var states = States;
            states[handleId] = (byte)state;
        }

        public int HandleGetState(int handleId)
        {
            return States[handleId];
        }
        #endregion
    }

}
