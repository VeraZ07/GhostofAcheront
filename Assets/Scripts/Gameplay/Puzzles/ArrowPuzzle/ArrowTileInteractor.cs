using DG.Tweening;
using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class ArrowTileInteractor : MonoBehaviour, IInteractable
    {
        ArrowPuzzleController puzzleController;
        int frameId;
        int tileId;
        int value;

        float zDisp = .125f;
        float zTime = .1f;
        float zDefault;
        bool selected = false;
        float frameHalfSize = 1.2f; 

        private void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
            zDefault = transform.localPosition.z;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool IsInteractionEnabled()
        {
            return !selected && puzzleController.TileIsSelectable(frameId, tileId);
        }

        public void StartInteraction(PlayerController playerController)
        {

            if (!IsInteractionEnabled())
                return;
            puzzleController.SelectTile(frameId, tileId);
            selected = true;
        }

        public void Init(ArrowPuzzleController puzzleController, int frameId, int tileId)
        {
            this.puzzleController = puzzleController;
            this.frameId = frameId;
            this.tileId = tileId;
        }

        public void Hide()
        {
            int direction = puzzleController.GetTileDirection(frameId, tileId);
            Vector3 target = Vector3.zero;
            switch (direction)
            {
                case 0:
                    target = new Vector3(transform.localPosition.x, frameHalfSize, transform.localPosition.z);
                    break;
                case 1:
                    target = new Vector3(frameHalfSize,  transform.localPosition.y, transform.localPosition.z);
                    break;
                case 2:
                    target = new Vector3(transform.localPosition.x, -frameHalfSize, transform.localPosition.z);
                    break;
                case 3:
                    target = new Vector3(-frameHalfSize, transform.localPosition.y, transform.localPosition.z);
                    break;
            }

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOLocalMoveZ(-zDisp, zTime, false));
            seq.Append(transform.DOLocalMove(target, zTime, false));
            seq.Insert(0, transform.DOShakeRotation(seq.Duration(), 50));
            seq.OnComplete(()=> { selected = false; gameObject.SetActive(false); });

            
        }

        public void Select()
        {
            Debug.Log($"Selected frameId:{frameId}, tileId:{tileId}");
            //transform.DOLocalMoveZ(-zDisp, zTime, true);
        }

        public void Unselect()
        {
            Debug.Log($"Unselected frameId:{frameId}, tileId:{tileId}");

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOLocalMoveZ(-zDisp, zTime, false));
            seq.Append(transform.DOLocalMoveZ(zDefault, zTime, false));
            seq.Insert(0, transform.DOShakeRotation(seq.Duration(), 50));
            seq.OnComplete(() => { selected = false; });

        }
    }

}
