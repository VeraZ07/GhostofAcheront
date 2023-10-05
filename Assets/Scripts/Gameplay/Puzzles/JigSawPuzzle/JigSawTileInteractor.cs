using DG.Tweening;
using GOA;
using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigSawTileInteractor : Interactable
{
    JigSawPuzzleController puzzleController;
    int frameId;
    int tileId;

    bool selected = false;


    float moveTime = 0.25f;
    float moveDist = .05f;


    public override bool IsInteractionEnabled()
    {
        if (puzzleController.TileIsSelectable(frameId, tileId))
            return true;
        else
            return false;
    }

    public override void StartInteraction(PlayerController playerController)
    {
        if (IsInteractionEnabled())
            puzzleController.SelectTile(frameId, tileId);
        
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

    public void SwitchPosition(Vector3 newPosition)
    {
        transform.DOLocalMove(newPosition, moveTime, false);
    }

    

    public void Init(JigSawPuzzleController puzzleController, int frameId, int tileId)
    {
        this.puzzleController = puzzleController;
        this.frameId = frameId;
        this.tileId = tileId;
    }
}
