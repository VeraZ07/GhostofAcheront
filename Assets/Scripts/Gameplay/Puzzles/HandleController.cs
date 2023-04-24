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

        [SerializeField]
        bool circular = false;

        [SerializeField]
        bool stopOnFinalState = false;


        int stateCount, finalState;
        HandlesPuzzleController puzzleController;
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

        IEnumerator DoStartInteraction(PlayerController playerController)
        {
            int handleId = puzzleController.HandleGetId(gameObject);
            puzzleController.HandleSetBusy(handleId, true);

            int currentState = puzzleController.HandleGetState(handleId);
            currentState++;
            if (currentState >= stateCount)
                currentState = 0;
            else if (currentState < 0)
                currentState = stateCount - 1;

            puzzleController.HandleSetState(handleId, currentState);

            yield return new WaitForSeconds(2f);

            if(!stopOnFinalState || currentState != finalState)
                puzzleController.HandleSetBusy(handleId, false);
        }
        #endregion


        #region ihandlecontroller implementation
        public void Init(HandlesPuzzleController puzzleController)
        {
            this.puzzleController = puzzleController;
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            HandlesPuzzle puzzle = builder.GetPuzzle(puzzleController.PuzzleIndex) as HandlesPuzzle;
            int id = puzzleController.HandleGetId(gameObject);
            List<HandlesPuzzle.Handle> hs = new List<HandlesPuzzle.Handle>(puzzle.Handles);
            Init(hs[id].InitialState, hs[id].FinalState, hs[id].StateCount);


        }


        public void Move()
        {
            int handleId = puzzleController.HandleGetId(gameObject);
            int currentState = puzzleController.HandleGetState(handleId);
            handleObject.transform.DORotateQuaternion(Quaternion.AngleAxis(currentState * angle, handleObject.transform.forward), 1f);
        }
        #endregion

        #region iinteractable implementation

        public bool IsInteractionEnabled()
        {
            return !puzzleController.Solved && !puzzleController.HandleIsBusy(puzzleController.HandleGetId(gameObject));
        }

        public void StartInteraction(PlayerController playerController)
        {
            
            

            throw new System.NotImplementedException();
        }

        public void StopInteraction(PlayerController playerController)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }

}
