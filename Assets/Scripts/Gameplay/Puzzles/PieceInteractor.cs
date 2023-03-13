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
        int pieceIndex;

        PuzzleController puzzleController = null;

        public bool IsEmpty
        {
            get { return (puzzleController as PicturePuzzleController).Pieces[pieceIndex] < 0; }
        }
               
        // Start is called before the first frame update
        void Start()
        {
            if (!puzzleController)
                GameObject.Destroy(gameObject);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Interact(PlayerController playerController)
        {
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
            throw new System.NotImplementedException();
        }

        public void SetInteractionEnabled(bool value)
        {
            throw new System.NotImplementedException();
        }

        public void Init(int puzzleId)
        {
            puzzleController = new List<PuzzleController>(GameObject.FindObjectsOfType<PuzzleController>()).Find(p => p.PuzzleIndex == puzzleId);
        }
    }
}

