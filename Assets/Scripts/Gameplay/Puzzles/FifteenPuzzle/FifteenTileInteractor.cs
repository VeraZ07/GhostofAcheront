using DG.Tweening;
using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class FifteenTileInteractor : MonoBehaviour, IInteractable
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

