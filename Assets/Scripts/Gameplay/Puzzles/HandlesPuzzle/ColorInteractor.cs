using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GOA.Level.LevelBuilder;

namespace GOA
{
    public class ColorInteractor : HandleInteractor
    {
        [SerializeField]
        Renderer colorRenderer;
        
        [SerializeField]
        List<Color> colors;

        [SerializeField]
        float emissiveIntensity = 10f;

        protected override void Awake()
        {
            base.Awake();
            Material mat = new Material(colorRenderer.material);
            colorRenderer.material = mat;
        }

        private void Start()
        {
           
            
        }

        public override IEnumerator DoMoveImpl(int oldState, int newState)
        {
            colorRenderer.material.SetColor("_EmissiveColor", colors[newState] * emissiveIntensity);
            colorRenderer.material.SetColor("_BaseColor", colors[newState]);
            yield break;
        }

        public override void Init(int state)
        {
            //LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            //Puzzle puzzle = builder.GetPuzzle(PuzzleController.PuzzleIndex);
            colorRenderer.material.SetColor("_EmissiveColor",  colors[state] * emissiveIntensity);
            colorRenderer.material.SetColor("_BaseColor", colors[state]);
        }
    }

}
