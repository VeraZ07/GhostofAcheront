using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class MemoryTileInteractor : MonoBehaviour, IInteractable
    {
        MemoryPuzzleController puzzleController;
        int frameId;
        int tileId;

        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        public bool IsInteractionEnabled()
        {
            return puzzleController.TileIsSelectable(frameId, tileId);
        }

        public void StartInteraction(PlayerController playerController)
        {
     
            if (!IsInteractionEnabled())
                return;
            puzzleController.SelectTile(frameId, tileId);
        }

        public void Show()
        {
            // Rotate tile
            transform.localEulerAngles = Vector3.up * 180f;
        }

        public void Hide()
        {
            transform.localEulerAngles = Vector3.zero;
        }

        public void Init(MemoryPuzzleController puzzleController, int frameId, int tileId)
        {
            this.puzzleController = puzzleController;
            this.frameId = frameId;
            this.tileId = tileId;
        }



    }

}
