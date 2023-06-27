using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Audio
{
    public class PlayerAudioController : MonoBehaviour
    {
        [SerializeField]
        AudioSource stepAudioSource;

        [SerializeField]
        List<AudioClip> stepClips;

        NetworkCharacterControllerPrototypeCustom cc;

        private void Awake()
        {
            cc = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnStepSound()
        {
            // If the audio is playing do nothing
            if (stepAudioSource.isPlaying)
                return;

            if (cc.Velocity.magnitude < 0.1f)
                return;

            // Get a random audio clip
            AudioClip clip = stepClips[Random.Range(0, stepClips.Count)];
            stepAudioSource.clip = clip;
            stepAudioSource.Play();
        }
    }

}
