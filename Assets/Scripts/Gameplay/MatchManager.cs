using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public enum MatchState { Entering, KickOff, Playing, Goal, Exiting }

    public class MatchManager : NetworkBehaviour
    {
        static MatchManager instance;

        [Networked(OnChanged = nameof(OnStateChanged))] public byte State { get; private set; } = (byte)MatchState.Entering;

        private void Awake()
        {
            if (!instance)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }

        

        public static void OnStateChanged(Changed<MatchManager> changed)
        {
            Debug.LogFormat("OnStateChanged:{0}", changed.Behaviour.State);
            //OnControllerIdChangedCallback?.Invoke(changed.Behaviour);
        }
    }

}
