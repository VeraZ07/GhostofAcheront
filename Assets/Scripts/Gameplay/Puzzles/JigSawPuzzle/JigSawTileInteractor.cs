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

    bool selected = false;

    Vector3 positionDefault;
    public Vector3 PositionDefault
    {
        get { return positionDefault; }
    }

    private void Awake()
    {
        positionDefault = transform.position;
    }

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
        if (!selected)
        {
            selected = true;
            
        }

    }

    public void Unselect()
    {
        if (selected)
        {
            selected = false;
        }
    }

    public void Init(JigSawPuzzleController puzzleController, int frameId, int tileId)
    {
        this.puzzleController = puzzleController;
        this.frameId = frameId;
        this.tileId = tileId;
    }
}
