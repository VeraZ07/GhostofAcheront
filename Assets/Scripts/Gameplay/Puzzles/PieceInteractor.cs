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

 

        PlayerController owner = null;
       
        public bool IsEmpty
        {
            get { return puzzleController.Pieces[pieceId] < 0; }
        }

        private void Awake()
        {
  
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

        //IEnumerator ResetBusy()
        //{
        //    yield return new WaitForSeconds(GameConfig.InteractionCooldown);
        //    busy = false;
        //}

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
                puzzleController.RemovePiece(this, playerController);

                //// Stop player update
                //playerController.InputDisabled = true;

                //// Called on the client
                //playerController.RpcOpenItemSelector();
                StopInteraction(playerController);
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

            //StartCoroutine(ResetBusy());
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

        }


    
    }
}

