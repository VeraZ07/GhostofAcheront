using DG.Tweening;
using GOA.Interfaces;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GOA.Level.LevelBuilder;

namespace GOA
{
    public class HandleController : MonoBehaviour, IHandleController, IInteractable
    {
        #region fields
        [SerializeField]
        GameObject handleObject;

        [SerializeField]
        float angle = 90f;


        int stateCount, finalState;
        HandlesPuzzleController puzzleController;
        int handleId;
        #endregion


        #region native
        #endregion

        #region private methods
        void Init(int initialState, int finalState, int stateCount)
        {
            this.stateCount = stateCount;
            this.finalState = finalState;
            handleObject.transform.rotation = Quaternion.AngleAxis(initialState * angle, handleObject.transform.forward);
        }

        

        IEnumerator DoMove()
        {
            int currentState = puzzleController.HandleGetState(handleId);
            puzzleController.HandleSetBusy(handleId, true);
            yield return handleObject.transform.DORotateQuaternion(Quaternion.AngleAxis(currentState * angle, handleObject.transform.forward), 1f).WaitForCompletion();
            HandlesPuzzle puzzle = FindObjectOfType<LevelBuilder>().GetPuzzle(puzzleController.PuzzleIndex) as HandlesPuzzle;
            puzzleController.HandleSetBusy(handleId, false);
        }
        #endregion


        #region ihandlecontroller implementation
        public void Init(HandlesPuzzleController puzzleController, int handleId)
        {
            this.puzzleController = puzzleController;
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            HandlesPuzzle puzzle = builder.GetPuzzle(puzzleController.PuzzleIndex) as HandlesPuzzle;
            this.handleId = handleId;
            List<HandlesPuzzle.Handle> hs = new List<HandlesPuzzle.Handle>(puzzle.Handles);
            Init(hs[handleId].InitialState, hs[handleId].FinalState, hs[handleId].StateCount);


        }


        public void Move()
        {
            Debug.Log("Move handle:" + gameObject);
            StartCoroutine(DoMove());
        }
        #endregion

        #region iinteractable implementation

        public bool IsInteractionEnabled()
        {
            return !puzzleController.Solved && !puzzleController.HandleIsBusy(handleId) && !puzzleController.HandleIsBlocked(handleId);
        }

        public void StartInteraction(PlayerController playerController)
        {
            HandlesPuzzle puzzle = FindObjectOfType<LevelBuilder>().GetPuzzle(puzzleController.PuzzleIndex) as HandlesPuzzle;
            puzzleController.HandleSwitchState(handleId);
        }

        public void StopInteraction(PlayerController playerController)
        {
            
        }
        #endregion
    }

}
