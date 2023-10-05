using DG.Tweening;
using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class FifteenTileInteractor : Interactable
    {
        //[SerializeField]
        //bool black;
        //public bool Black
        //{
        //    get { return black; }
        //}

        FifteenPuzzleController puzzleController;
        int frameId;
        int tileId;

        bool selected = false;

        float moveTime = 0.25f;
        float moveDist = .1f;

      
        public override bool IsInteractionEnabled()
        {
            return puzzleController.TileIsSelectable(frameId, tileId);
        }

        public override void StartInteraction(PlayerController playerController)
        {
            if (IsInteractionEnabled())
                puzzleController.SelectTile(frameId, tileId);
        }
               

        public void Init(FifteenPuzzleController puzzleController, int frameId, int tileId)
        {
            this.puzzleController = puzzleController;
            this.frameId = frameId;
            this.tileId = tileId;
        }
    }

}

