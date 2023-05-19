using DG.Tweening;
using GOA.Interfaces;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GOA.Level.LevelBuilder;

namespace GOA
{
    public abstract class HandleInteractor : MonoBehaviour, IHandle, IInteractable
    {
        #region fields
        
        int stateCount, finalState;
        bool stopOnFinalState;
        PuzzleController puzzleController;
        public PuzzleController PuzzleController
        {
            get { return puzzleController; }
        }

        IHandleManager handleManager;

        int handleId;

        public int CurrentState
        {
            get { return handleManager.GetHandleState(handleId); }
        }

        public abstract IEnumerator DoMoveImpl(int oldState, int newState);

        public abstract void Init(int state);
        #endregion


        #region native
        #endregion

        #region private methods



        IEnumerator DoMove(int oldState, int newState)
        {
            yield return DoMoveImpl(oldState, newState);
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

            Init(initialState);
            
        }


        public void Move(int oldState, int newState)
        {
            StartCoroutine(DoMove(oldState, newState));
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
