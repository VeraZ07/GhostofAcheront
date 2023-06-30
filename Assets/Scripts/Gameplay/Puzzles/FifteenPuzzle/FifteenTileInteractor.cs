using DG.Tweening;
using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class FifteenTileInteractor : MonoBehaviour, IInteractable
    {
        [SerializeField]
        bool black;

        FifteenPuzzleController puzzleController;
        int frameId;
        int tileId;

        bool selected = false;

        float moveTime = 0.25f;
        float moveDist = .05f;

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
            throw new System.NotImplementedException();
        }

        public void Select()
        {
            if (!selected)
            {
                selected = true;
                transform.DOLocalMoveZ(transform.localPosition.z - moveDist, moveTime, false);
                //transform.DOShakeRotation(moveTime);
                //transform.DOLocalRotate(Vector3.up * 180f, moveTime, RotateMode.Fast).SetEase(Ease.OutBounce);
            }
        }

        public void Unselect()
        {
            if (selected)
            {
                //selected = false;
                transform.DOLocalMoveZ(transform.localPosition.z + moveDist, moveTime, false).onComplete += () => { selected = false; };
            }
        }

        public void Init(FifteenPuzzleController puzzleController, int frameId, int tileId)
        {
            this.puzzleController = puzzleController;
            this.frameId = frameId;
            this.tileId = tileId;
        }
    }

}

