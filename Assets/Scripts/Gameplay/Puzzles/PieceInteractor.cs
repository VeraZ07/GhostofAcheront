using GOA.Interfaces;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class PieceInteractor : MonoBehaviour, IInteractable
    {
        [SerializeField]
        GameObject pieceObject;
        
        int pieceId = -1;

        PicturePuzzleController puzzleController = null;

        bool busy = false;

        List<PlayerController> playerControllers = new List<PlayerController>();

        Vector3 pivotLocalPositionDefault;
        Quaternion pivotLocalRotationDefault;

        public bool IsEmpty
        {
            get { return puzzleController.Pieces[pieceId] < 0; }
        }

        private void Awake()
        {
            pivotLocalPositionDefault = pieceObject.transform.parent.localPosition;
            pivotLocalRotationDefault = pieceObject.transform.parent.localRotation;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (!PlayerController.Local)
                return;

            if (!puzzleController)
                return;

            if (SessionManager.Instance.Runner.IsServer)
            {
                // Check distance
            }
            else
            {
                // Only local for visual clue

            }

            // Check for pieces visibility
            //int pieceValue = puzzleController.Pieces[pieceId];
            //if(pieceValue < 0)
            //{
            //    pieceObject.SetActive(false);
            //}
            //else
            //{
            //    pieceObject.SetActive(true);
            //}
        }

        

        public void Interact(PlayerController playerController)
        {
            busy = true;

            if (IsEmpty)
            {
                // Open the inventory and select an object to be inserted
            }
            else
            {
                // Remove the current inserted object 
            }
        }

        public bool IsInteractionEnabled()
        {
            return !busy;
        }

        public void SetInteractionEnabled(bool value)
        {
            throw new System.NotImplementedException();
        }

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
        }

        public void ResetPivot()
        {
            pieceObject.transform.parent.localPosition = pivotLocalPositionDefault;
            pieceObject.transform.parent.localRotation = pivotLocalRotationDefault;
        }

        public void SetPivotPositionAndRotation(Vector3 localPositon, Quaternion localRotation)
        {
            pieceObject.transform.parent.localPosition = localPositon;
            pieceObject.transform.parent.localRotation = localRotation;
        }

        public void GetPivotPositionAndRotationDefault(out Vector3 defaultPosition, out Quaternion defaultRotation)
        {
            defaultPosition = pivotLocalPositionDefault;
            defaultRotation = pivotLocalRotationDefault;
        }

        public void Hide()
        {
            pieceObject.SetActive(false);
        }

        public void Show()
        {
            pieceObject.SetActive(true);
        }
    }
}

