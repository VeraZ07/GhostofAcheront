using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class PictureInteractor : Interactable
    {
        #region fields
        [SerializeField]
        AudioSource audioSource;

        [SerializeField]
        AudioClip interactionClip;

        PicturePuzzleController puzzleController;
                
        Inventory inventory;
        #endregion

        #region native methods
      
        #endregion

        #region private methods
        IEnumerator DoStartInteraction(PlayerController playerController)
        {
            Debug.LogFormat("[PuzzleInteractor - Starting interaction - playerId:{0}]", playerController.PlayerId);

            puzzleController.Busy = true;

            //inventory = new List<Inventory>(FindObjectsOfType<Inventory>()).Find(i => i.PlayerId == playerController.PlayerId);
            inventory = new List<Inventory>(FindObjectsOfType<Inventory>()).Find(i => i.Object.InputAuthority == playerController.Object.InputAuthority);

            Debug.LogFormat("[PuzzleInteractor - Starting interaction - Inventory.Items.Count:{0}]", inventory.Items.Count);

            if (inventory.Items.Count > 0)
            {
                // Get the first item and remove it from inventory
                string itemName = inventory.Items[0].ToString();
                inventory.RemoveItem(itemName);
                // Insert item
                Debug.LogFormat("[PictureInteractor - Adding new item:{0}]", itemName);
                puzzleController.InsertPiece(itemName);

                if (audioSource)
                {
                    audioSource.clip = interactionClip;
                    audioSource.Play();
                }
                    

                yield return new WaitForSeconds(2f);

            }
            else
            {
                //inventory.OnEmptyInventory?.Invoke();
                inventory.RpcReportEmpty();
            }
            puzzleController.Busy = false;

        }
        #endregion


        #region iinteractable implementation

        public override bool IsInteractionEnabled()
        {
            return !puzzleController.Solved && !puzzleController.Busy;
        }

        public override void StartInteraction(PlayerController playerController)
        {
            StartCoroutine(DoStartInteraction(playerController));
        }

        
     
        #endregion



        #region public methods
        public void Init(int puzzleId)
        {
            Debug.LogFormat("[PuzzleInteractor - Initializing - PuzzleId:{0}]", puzzleId);

            // Set the puzzle controller
            puzzleController = new List<PicturePuzzleController>(GameObject.FindObjectsOfType<PicturePuzzleController>()).Find(p => p.PuzzleIndex == puzzleId);

          
        }
        #endregion
    }

}
