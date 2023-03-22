using GOA.Interfaces;
using GOA.Level;
using GOA.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class PieceInteractor : MonoBehaviour, IInteractable
    {
        [SerializeField]
        GameObject placeHolder;
        
        int pieceId = -1;

        PicturePuzzleController puzzleController = null;

        bool busy = false;

        List<PlayerController> playerControllers = new List<PlayerController>();

        Vector3 pivotLocalPositionDefault;
        Quaternion pivotLocalRotationDefault;

        PlayerController owner = null;
       
        public bool IsEmpty
        {
            get { return puzzleController.Pieces[pieceId] < 0; }
        }

        private void Awake()
        {
            pivotLocalPositionDefault = placeHolder.transform.parent.localPosition;
            pivotLocalRotationDefault = placeHolder.transform.parent.localRotation;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (!SessionManager.Instance.Runner || !PlayerController.Local || !puzzleController || busy)
                return;

            

        }

        void GetPlaceHolderPositionAndRotationDefault(out Vector3 defaultPosition, out Quaternion defaultRotation)
        {
            defaultPosition = pivotLocalPositionDefault;
            defaultRotation = pivotLocalRotationDefault;
        }

        #region interface implementation
        /// <summary>
        /// Server only
        /// </summary>
        /// <param name="playerController"></param>
        public void StartInteraction(PlayerController playerController)
        {
            Debug.LogFormat("Interaction request from player {0}", playerController);

            busy = true;
            owner = playerController;

            if (IsEmpty)
            {
                // Stop player update
                playerController.InputDisabled = true;
                
                // Called on the client
                playerController.RpcOpenItemSelector();
            }
            else
            {
                // Remove the current inserted object 
            }
        }

        /// <summary>
        /// Server only
        /// </summary>
        public void StopInteraction(PlayerController playerController)
        {
            if (owner != playerController)
                return;


            owner = null;
            busy = false;
            playerController.InputDisabled = false;
        }

        public bool IsInteractionEnabled()
        {
            return !puzzleController.Solved;
        }

        public void SetInteractionEnabled(bool value)
        {
            throw new System.NotImplementedException();
        }

        public bool IsBusy()
        {
            return busy;
        }

        public bool TryUseItem(string itemName)
        {
            //throw new System.NotImplementedException();
            return puzzleController.TryInsertPiece(pieceId, itemName);
            
        }
        #endregion

        public void Init(int puzzleId)
        {
            // Set the puzzle controller
            puzzleController = new List<PicturePuzzleController>(GameObject.FindObjectsOfType<PicturePuzzleController>()).Find(p => p.PuzzleIndex == puzzleId);

            // Set the piece id
            for(int i=0; i<transform.parent.childCount && pieceId < 0; i++)
            {
                if(transform.parent.GetChild(i) == transform)
                    pieceId = i;
            }

            // Set visibility
            if (puzzleController.Pieces[pieceId] < 0)
                Hide();
            else
                Show();
        }

        public void ResetPlaceHolderPositionAndRotation()
        {
            placeHolder.transform.parent.localPosition = pivotLocalPositionDefault;
            placeHolder.transform.parent.localRotation = pivotLocalRotationDefault;
        }

        public void SetPlaceHolderPositionAndRotation(Vector3 localPositon, Quaternion localRotation)
        {
            placeHolder.transform.parent.localPosition = localPositon;
            placeHolder.transform.parent.localRotation = localRotation;
        }

        public void Hide()
        {
            // We can implement fx here
            placeHolder.SetActive(false);
        }

        public void Show()
        {
            PieceInteractor target = puzzleController.GetInteractor(puzzleController.Pieces[pieceId]);

            Vector3 targetPos;
            Quaternion targetRot;
            GetPlaceHolderPositionAndRotationDefault(out targetPos, out targetRot);
            target.SetPlaceHolderPositionAndRotation(targetPos, targetRot);
            // We can implement fx here
            target.placeHolder.SetActive(true);
        }

    
    }
}

