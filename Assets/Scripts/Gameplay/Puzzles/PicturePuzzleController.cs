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
        #region fields

        [UnitySerializeField]
        [Networked(OnChanged = nameof(OnPiecesChanged))]
        [Capacity(10)]
        public NetworkLinkedList<NetworkBool> Pieces { get; } = default;

        [Networked]
        public NetworkBool Busy { get; set; } = false;

        PictureInteractor interactor;
        
        List<GameObject> placeHolders = new List<GameObject>();

        GameObject pictureObject;
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

        #region fusion overrides
        public override void Spawned()
        {
            base.Spawned();

            // Get the picture scene object and set all the triggers, which are under the Trigger game object.
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            pictureObject = builder.CustomObjects[(builder.GetPuzzle(PuzzleIndex) as PicturePuzzle).PictureId].SceneObject;
            // Set the interactor
            interactor = pictureObject.GetComponentInChildren<PictureInteractor>();
            interactor.Init(PuzzleIndex);

            // Fill the placeholder list
            Transform root = new List<Transform>(pictureObject.GetComponentsInChildren<Transform>()).Find(t => t.gameObject.name.ToLower().Equals("placeholders"));
            for (int i = 0; i < root.childCount; i++)
            {
                GameObject placeHolder = root.GetChild(i).gameObject;
                placeHolders.Add(placeHolder);
                placeHolder.SetActive(false);
            }

        }
        #endregion

      

      

        #region public methods
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
                Pieces.Add(false); // Empty
                
            }
            
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
            pieces[pieceId] = true;
        }
        #endregion

        #region fusion callbacks
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
                bool oldValue = changed.Behaviour.Pieces[i]; 
                Debug.Log("PC - old value:" + oldValue);
                changed.LoadNew();
                bool newValue = changed.Behaviour.Pieces[i];

                if (!newValue)
                    solved = false;
                else
                {
                    GameObject placeHolder = changed.Behaviour.placeHolders[i];
                    placeHolder.SetActive(true);
                }
                
                
            }

            if (solved)
                changed.Behaviour.Solved = true;
                

        }
        #endregion

        


        
    }

}
