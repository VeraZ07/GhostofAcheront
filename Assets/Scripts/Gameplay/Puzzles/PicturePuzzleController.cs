using Fusion;
using GOA.Assets;
using GOA.Interfaces;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static GOA.Level.LevelBuilder;

namespace GOA
{
    public class PicturePuzzleController : PuzzleController
    {
        
        [UnitySerializeField]
        [Networked(OnChanged = nameof(OnPiecesChanged))]
        [Capacity(10)]
        public NetworkLinkedList<int> Pieces { get; } = default;

        PictureInteractor interactor;

        //[SerializeField]
        //List<PieceInteractor> interactors = new List<PieceInteractor>();

        List<GameObject> placeHolders = new List<GameObject>();

        //List<GameObject> pivots = new List<GameObject>();
      
        GameObject pictureObject;
        
        

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Spawned()
        {
            base.Spawned();

            // Get the picture scene object and set all the triggers, which are under the Trigger game object.
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            pictureObject = builder.CustomObjects[(builder.GetPuzzle(PuzzleIndex) as PicturePuzzle).PictureId].SceneObject;
            // Set the interactor
            interactor = pictureObject.GetComponentInChildren<PictureInteractor>();
            interactor.Init(PuzzleIndex);

            //Transform root = new List<Transform>(pictureObject.GetComponentsInChildren<Transform>()).Find(t => t.gameObject.name.ToLower().Equals("triggers"));
            //for (int i = 0; i < root.childCount; i++)
            //{
            //    PieceInteractor interactor = root.GetChild(i).gameObject.GetComponent<PieceInteractor>();
            //    interactors.Add(interactor);
            //    interactor.Init(PuzzleIndex);
            //}

            // Fill the placeholder list
            Transform root = new List<Transform>(pictureObject.GetComponentsInChildren<Transform>()).Find(t => t.gameObject.name.ToLower().Equals("placeholders"));
            for (int i = 0; i < root.childCount; i++)
            {
                GameObject placeHolder = root.GetChild(i).gameObject;
                placeHolders.Add(placeHolder);
                placeHolder.SetActive(false);
            }

            //// Fill the pivot list
            //root = new List<Transform>(pictureObject.GetComponentsInChildren<Transform>()).Find(t => t.gameObject.name.ToLower().Equals("pivots"));
            //for (int i = 0; i < root.childCount; i++)
            //{
            //    GameObject pivot = root.GetChild(i).gameObject;
            //    pivots.Add(pivot);
                
            //}
        }

        public override void Initialize(int puzzleIndex)
        {
            base.Initialize(puzzleIndex);
            
            // Get the builder
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();

            // Get the puzzle
            PicturePuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as PicturePuzzle;

            // Set network states to synch
            for(int i=0; i<puzzle.PieceIds.Count; i++)
            {
                Debug.Log("PC - intialize piece:" + i);
                Pieces.Add(-1); // Empty
                
            }
            
        }

        //public PieceInteractor GetInteractor(int id)
        //{
        //    return interactors[id];
        //}
        public PictureInteractor GetInteractor()
        {
            return interactor;
        }

        

        public static void OnPiecesChanged(Changed<PicturePuzzleController> changed)
        {
            bool solved = true;
            Debug.Log("PC - Num of pieces:" + changed.Behaviour.Pieces.Count);
            for(int i=0; i<changed.Behaviour.Pieces.Count; i++)
            {
                Debug.Log("PC - Loading old value:" + changed.Behaviour.Pieces[i]);
                changed.LoadOld();
                if (changed.Behaviour.Pieces.Count == 0)
                    return;
                int oldValue = changed.Behaviour.Pieces[i]; 
                Debug.Log("PC - old value:" + oldValue);
                changed.LoadNew();
                int newValue = changed.Behaviour.Pieces[i];

                if (newValue != i)
                    solved = false;

                if (oldValue != newValue)
                {
                    if(newValue < 0)// It's empty
                    {
                        changed.Behaviour.placeHolders[oldValue].SetActive(false);
                    }
                    else // It's full
                    {
                        GameObject placeHolder = changed.Behaviour.placeHolders[changed.Behaviour.Pieces[i]];
                        //GameObject pivot = changed.Behaviour.pivots[i];
                        //placeHolder.transform.localPosition = pivot.transform.localPosition;
                        //placeHolder.transform.localRotation = pivot.transform.localRotation;
                        placeHolder.SetActive(true);
       
                        
                    }
                }

                
            }

            if (solved)
                changed.Behaviour.Solved = true;
                

        }

        public void InsertPiece(string itemName)
        {
            // Get the puzzle asset 
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            Puzzle puzzle = builder.GetPuzzle(PuzzleIndex);
            PicturePuzzleAsset asset = puzzle.Asset as PicturePuzzleAsset;
            // Find the piece id 
            int pieceId = new List<ItemAsset>(asset.Items).FindIndex(i => i.name.ToLower().Equals(itemName.ToLower()));
            var pieces = Pieces;
            pieces[pieceId] = pieceId;
        }

        //public bool TryInsertPiece(int pieceId, string assetName)
        //{
        //    LevelBuilder builder = FindObjectOfType<LevelBuilder>();
        //    Puzzle puzzle = builder.GetPuzzle(PuzzleIndex);
        //    if((puzzle.Asset as PicturePuzzleAsset).Items[pieceId].name.ToLower().Equals(assetName.ToLower()))
        //    {
        //        int paramId = pieceId;
        //        paramId = new List<ItemAsset>((puzzle.Asset as PicturePuzzleAsset).Items).FindIndex(i => i.name.ToLower().Equals(assetName.ToLower()));
               
        //        var pieces = Pieces;
        //        pieces[pieceId] = paramId;
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
            
        //}

        //public void RemovePiece(PieceInteractor interactor, PlayerController playerController)
        //{
        //    // Get the interactor id
        //    int id = interactors.IndexOf(interactor);
            
        //    // Get the item asset
        //    LevelBuilder builder = FindObjectOfType<LevelBuilder>();
        //    Puzzle puzzle = builder.GetPuzzle(PuzzleIndex);
        //    ItemAsset itemAsset = (puzzle.Asset as PicturePuzzleAsset).Items[Pieces[id]];

        //    // Add the item to the inventory
        //    Inventory inventory = new List<Inventory>(FindObjectsOfType<Inventory>()).Find(i => i.PlayerId == playerController.PlayerId);
        //    inventory.AddItem(itemAsset.name);

        //    // Remove from the object
        //    placeHolders[Pieces[id]].SetActive(false);
        //    var pieces = Pieces;
        //    pieces[id] = -1;

        //}
        
    }

}
