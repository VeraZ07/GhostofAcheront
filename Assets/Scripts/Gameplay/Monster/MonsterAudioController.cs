using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Audio
{
    public class MonsterAudioController : NetworkBehaviour
    {

        [SerializeField]
        AudioSource mainSource;

        [SerializeField]
        List<AudioClip> breathClips;

        [SerializeField]
        List<AudioClip> growlClips;

        int state = -1;

        MonsterController monsterController;

        float time = 0;
        float minBreathTime = 4f;
        float maxBreathTime = 5f;

        float minGrowlTime = 3f;
        float maxGrowlTime = 4f;

        private void Awake()
        {
            monsterController = GetComponent<MonsterController>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            LoopState();
        }

        void LoopState()
        {
            int monsterState = monsterController.State;

            switch (monsterState)
            {
                case (int)MonsterState.Idle:
                case (int)MonsterState.Moving:
                case (int)MonsterState.PlayerEscaped:
                case (int)MonsterState.PlayerLost:
                    LoopBreathState();
                    break;
                case (int)MonsterState.PlayerSpotted:
                case (int)MonsterState.Hunting:
                    LoopGrowlState();
                    break;

            }
        }

        void LoopBreathState()
        {
            if (state == 0)
            {
                // Loop state
                time -= Time.deltaTime;
                if(time < 0)
                {
                    // Reset time
                    time = Random.Range(minBreathTime, maxBreathTime);

                    // Breath
                    RpcBreath();
                }
            }
            else
            {
                // Init state
                state = 0;
                time = Random.Range(minBreathTime, maxBreathTime);
            }
            
        }

        void LoopGrowlState()
        {
            if (state == 1)
            {
                // Loop
                // Loop state
                time -= Time.deltaTime;
                if (time < 0)
                {
                    // Reset time
                    time = Random.Range(minGrowlTime, maxGrowlTime);

                    // Breath
                    RpcGrowl();
                }
            }
            else
            {
                // Set state
                state = 1;
                // Growl soon
                time = 0;
            }
            
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        void RpcBreath()
        {
            // Breath
            AudioClip clip = breathClips[Random.Range(0, breathClips.Count)];
            mainSource.clip = clip;
            mainSource.Play();
        }

        void RpcGrowl()
        {
            // Breath
            AudioClip clip = growlClips[Random.Range(0, growlClips.Count)];
            mainSource.clip = clip;
            mainSource.Play();
        }
    }

}
