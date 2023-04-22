using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class PictureInteractor : MonoBehaviour, IInteractable
    {
        #region fields
        PicturePuzzleController puzzleController;

        bool busy = false;

        Inventory inventory;
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

        #region private methods
        IEnumerator DoStartInteraction(PlayerController playerController)
        {
            busy = true;

            if (!inventory)
                inventory = new List<Inventory>(FindObjectsOfType<Inventory>()).Find(i => i.PlayerId == playerController.PlayerId);

            Debug.LogFormat("[PuzzleInteractor - Starting interaction - Inventory.Items.Count:{0}]", inventory.Items.Count);

            if (inventory.Items.Count > 0)
            {
                // Get the first item and remove it from inventory
                string itemName = inventory.Items[0].ToString();
                inventory.Items.Remove(itemName);
                // Insert item
                Debug.LogFormat("[PictureInteractor - Adding new item:{0}]", itemName);
                puzzleController.InsertPiece(itemName);

                yield return new WaitForSeconds(2f);

            }
                        

            busy = false;
        }
        #endregion


        #region iinteractable implementation

        public bool IsBusy()
        {
            return busy;
        }

        public bool IsInteractionEnabled()
        {
            return !puzzleController.Solved;
        }

        public void SetInteractionEnabled(bool value)
        {
            throw new System.NotImplementedException();
        }

        public void StartInteraction(PlayerController playerController)
        {
            StartCoroutine(DoStartInteraction(playerController));
        }

        public void StopInteraction(PlayerController playerController)
        {
            throw new System.NotImplementedException();
        }

        //public bool TryUseItem(string itemName)
        //{
        //    throw new System.NotImplementedException();
        //}
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
