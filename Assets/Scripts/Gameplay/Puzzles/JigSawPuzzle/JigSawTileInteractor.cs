using GOA;
using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JigSawTileInteractor : MonoBehaviour, IInteractable
{
    JigSawPuzzleController puzzleController;
    int frameId;
    int tileId;

    public bool IsInteractionEnabled()
    {
        if (puzzleController.TileIsSelectable(frameId, tileId))
            return true;
        else
            return false;
    }

    public void StartInteraction(PlayerController playerController)
    {
        if (IsInteractionEnabled())
            puzzleController.SelectTile(frameId, tileId);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select()
    {

    }

    public void Unselect()
    {

    }

    public void Init(JigSawPuzzleController puzzleController, int frameId, int tileId)
    {
        this.puzzleController = puzzleController;
        this.frameId = frameId;
        this.tileId = tileId;
    }
}
