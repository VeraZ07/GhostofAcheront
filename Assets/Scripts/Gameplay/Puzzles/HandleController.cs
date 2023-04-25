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
        bool stopOnFinalState;
        PuzzleController puzzleController;

        IHandleManager handleManager;

        int handleId;
        #endregion


        #region native
        #endregion

        #region private methods

        

        IEnumerator DoMove()
        {
            
            int currentState = handleManager.GetHandleState(handleId);
            yield return handleObject.transform.DORotateQuaternion(Quaternion.AngleAxis(currentState * angle, handleObject.transform.forward), 1f).WaitForCompletion();
            HandlesPuzzle puzzle = FindObjectOfType<LevelBuilder>().GetPuzzle(puzzleController.PuzzleIndex) as HandlesPuzzle;
            handleManager.SetHandleBusy(handleId, false);
        }
        #endregion


        #region ihandlecontroller implementation
        public void Init(PuzzleController puzzleController, int handleId, int initialState, int finalState, int stateCount, bool stopOnFinalState)
        {
            this.puzzleController = puzzleController;
            handleManager = puzzleController as IHandleManager;
            this.handleId = handleId;
            this.stateCount = stateCount;
            this.finalState = finalState;
            this.stopOnFinalState = stopOnFinalState;

            handleObject.transform.rotation = Quaternion.AngleAxis(initialState * angle, handleObject.transform.forward);
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
            return !puzzleController.Solved && !handleManager.IsHandleBusy(handleId) && (handleManager.GetHandleState(handleId) != finalState || !stopOnFinalState);
        }

        public void StartInteraction(PlayerController playerController)
        {
            int currentState = handleManager.GetHandleState(handleId);
            currentState++;
            if (currentState >= stateCount)
                currentState = 0;

            handleManager.SetHandleBusy(handleId, true);
            handleManager.SetHandleState(handleId, currentState);
        }

 
        #endregion
    }

}
