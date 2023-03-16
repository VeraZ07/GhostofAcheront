using Fusion;
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

        //[SerializeField]
        List<PieceInteractor> interactors = new List<PieceInteractor>();

      
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
            Transform root = new List<Transform>(pictureObject.GetComponentsInChildren<Transform>()).Find(t => t.gameObject.name.ToLower().Equals("triggers"));
            for (int i = 0; i < root.childCount; i++)
            {
                PieceInteractor interactor = root.GetChild(i).gameObject.GetComponent<PieceInteractor>();
                interactors.Add(interactor);
                interactor.Init(PuzzleIndex);
            }

            
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
                Pieces.Add(-1); // Empty
            }
            
        }

        public PieceInteractor GetInteractor(int id)
        {
            return interactors[id];
        }

        public static void OnPiecesChanged(Changed<PicturePuzzleController> changed)
        {
            for(int i=0; i<changed.Behaviour.Pieces.Count; i++)
            {
                changed.LoadOld();
                int oldValue = changed.Behaviour.Pieces[i];
                changed.LoadNew();
                int newValue = changed.Behaviour.Pieces[i];
              
                if (oldValue != newValue)
                {
                    if(newValue < 0)// It's empty
                    {
                        
                        //changed.Behaviour.interactors[oldValue].ResetPivot();
                        changed.Behaviour.interactors[oldValue].Hide();
                    }
                    else // It's full
                    {
                        changed.Behaviour.interactors[i].Show();
                        //Vector3 targetPos;
                        //Quaternion targetRot;
                        //changed.Behaviour.interactors[i].GetPivotPositionAndRotationDefault(out targetPos, out targetRot);
                        //changed.Behaviour.interactors[newValue].SetPivotPositionAndRotation(targetPos, targetRot);
                        //changed.Behaviour.interactors[newValue].Show();
                    }
                }
            }

        }
    }

}
