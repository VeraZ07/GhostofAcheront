using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Audio
{
    public class PuzzleGlobalAudio : MonoBehaviour
    {
        [SerializeField]
        AudioSource solvedAudioSource;

        
        void Awake()
        {
            PuzzleController.OnSolvedChangedCallback += PlaySolvedAudioSource;
        }

   

        void PlaySolvedAudioSource(PuzzleController puzzleController)
        {
            solvedAudioSource.Play();
        }
    }

}
