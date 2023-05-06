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

        private void Awake()
        {
            Material mat = new Material(colorRenderer.material);
            colorRenderer.material = mat;
        }

        private void Start()
        {
           
            
        }

        public override IEnumerator DoMoveImpl(int oldState, int newState)
        {
            colorRenderer.material.SetColor("_EmissiveColor", colors[newState] * emissiveIntensity);
            yield break;
        }

        public override void Init(int state)
        {
            //LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            //Puzzle puzzle = builder.GetPuzzle(PuzzleController.PuzzleIndex);
            Debug.LogFormat("Color - Init - State:{0}", state);
            colorRenderer.material.SetColor("_EmissiveColor",  colors[state] * emissiveIntensity);
        }
    }

}
