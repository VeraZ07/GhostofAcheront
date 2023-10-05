using DG.Tweening;
using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class MemoryTileInteractor : Interactable
    {
        MemoryPuzzleController puzzleController;
        int frameId;
        int tileId;

        bool selected = false;

        float zDefault = 0;
        float moveTime = 0.25f;
        float moveDist = .05f;

        protected override void Awake()
        {
            zDefault = transform.localPosition.z;
        }

    
        public override bool IsInteractionEnabled()
        {
            return !selected && puzzleController.TileIsSelectable(frameId, tileId);
        }

        public override void StartInteraction(PlayerController playerController)
        {
     
            if (!IsInteractionEnabled())
                return;
            puzzleController.SelectTile(frameId, tileId);
        }

        public void Select()
        {
            if (!selected)
            {
                selected = true;
                transform.DOLocalMoveZ(zDefault - moveDist, moveTime, false);
                transform.DOLocalRotate(Vector3.up * 180f, moveTime, RotateMode.Fast).SetEase(Ease.OutBounce);
                //transform.localEulerAngles = Vector3.up * 180f;
            }
            
        }

        public void Unselect()
        {
            if (selected)
            {
                
                transform.DOLocalMoveZ(zDefault, moveTime, false).OnComplete( () => { selected = false; });
                transform.DOLocalRotate(Vector3.zero, moveTime, RotateMode.Fast).SetEase(Ease.OutBounce);
                
                //transform.localEulerAngles = Vector3.zero;
            }
            
        }

        public void Show()
        {
            selected = true;
            transform.DOLocalMoveZ(zDefault, moveTime, false);
            transform.DOLocalRotate(Vector3.up * 180f, moveTime, RotateMode.Fast).SetEase(Ease.InOutElastic);
            //transform.localEulerAngles = Vector3.up * 180f;
        }

        public void Init(MemoryPuzzleController puzzleController, int frameId, int tileId)
        {
            this.puzzleController = puzzleController;
            this.frameId = frameId;
            this.tileId = tileId;
        }



    }

}
