using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class GlobeInteractor : Interactable
    {
        [SerializeField]
        Renderer colorRenderer;

        [SerializeField]
        Color baseColor;

        float[] emissiveSteps = new float[] { .5f, 6f, 12f };

        GlobeCoopPuzzleController puzzleController;

        float lightTime = 0.25f;

        protected override void Awake()
        {
            base.Awake();

            // Set color
            Material mat = new Material(colorRenderer.material);
            colorRenderer.material = mat;
            colorRenderer.material.SetColor("_BaseColor", baseColor);
            colorRenderer.material.SetColor("_EmissiveColor", baseColor * emissiveSteps[0]);
        }

        private void OnEnable()
        {
            PuzzleController.OnSolvedChangedCallback += HandleOnPuzzleSolved;
        }

        private void OnDisable()
        {
            PuzzleController.OnSolvedChangedCallback -= HandleOnPuzzleSolved;
        }

        void HandleOnPuzzleSolved(PuzzleController puzzleController)
        {
            if (this.puzzleController != puzzleController)
                return;
            StartCoroutine(ExplodeInLight());
        }

        IEnumerator ExplodeInLight()
        {
            yield return new WaitForSeconds(1f);
            Debug.Log($"Explode index:{puzzleController.GetGlobeIndex(this)}");
            DOTween.To(() => colorRenderer.material.GetColor("_EmissiveColor"), (x) => colorRenderer.material.SetColor("_EmissiveColor", x), baseColor * emissiveSteps[2], .25f);
        }

        public override bool IsInteractionEnabled()
        {
            return puzzleController.GlobeIsInteractable(this);
        }

        public override void StartInteraction(PlayerController playerController)
        {
            puzzleController.ActivateGlobe(this);
        }

        public override void StopInteraction(PlayerController playerController)
        {
            puzzleController.DeactivateGlobe(this);
        }

        public override bool KeepPressed()
        {
            return true;
        }

        public void Init(PuzzleController puzzleController)
        {
            this.puzzleController = puzzleController as GlobeCoopPuzzleController;
        }
       
        public void LightUp()
        {
            Debug.Log($"LightUp index:{puzzleController.GetGlobeIndex(this)}");
            // Light up the globe
            DOTween.To(() => colorRenderer.material.GetColor("_EmissiveColor"), (x) => colorRenderer.material.SetColor("_EmissiveColor", x), baseColor * emissiveSteps[1], lightTime);
            
            colorRenderer.material.SetColor("_EmissiveColor", baseColor * emissiveSteps[puzzleController.GetActiveGlobeCount()]);
        }

        public void DimLight()
        {
            DOTween.To(() => colorRenderer.material.GetColor("_EmissiveColor"), (x) => colorRenderer.material.SetColor("_EmissiveColor", x), baseColor * emissiveSteps[0], lightTime);
            //colorRenderer.material.SetColor("_EmissiveColor", baseColor * emissiveSteps[puzzleController.GetActiveGlobeCount()]);
        }
    }

}
