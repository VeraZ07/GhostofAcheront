using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Audio
{
    public class PuzzleGlobalAudio : MonoBehaviour
    {
        [SerializeField]
        AudioSource solvedAudioSource;

        
        void OnEnable()
        {
            PuzzleController.OnSolvedChangedCallback += PlaySolvedAudioSource;
        }

        void OnDisable()
        {
            PuzzleController.OnSolvedChangedCallback -= PlaySolvedAudioSource;
        }


        void PlaySolvedAudioSource(PuzzleController puzzleController)
        {
            solvedAudioSource.Play();
        }
    }

}
