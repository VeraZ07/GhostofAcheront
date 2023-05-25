using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOA
{
    public class Player: NetworkBehaviour
    {
        public static UnityAction<Player> OnDespawned;
        public static UnityAction<Player> OnReadyChangedCallback;
        public static UnityAction<Player> OnNameChangedCallback;

        public static Player Local { get; private set; }

        [UnitySerializeField]
        [Networked(OnChanged = nameof(OnNameChanged))] public string Name { get; private set; }

        [Networked(OnChanged = nameof(OnReadyChanged))] public NetworkBool Ready { get; private set; }

        [Networked] public byte CharacterId { get; private set; } = 0;

        PlayerRef playerRef;
        public PlayerRef PlayerRef
        {
            get { return playerRef; }
        }
                

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (!HasInputAuthority)
                return;
           
        }

        /// <summary>
        /// Call when the server spawns this player
        /// </summary>
        public override void Spawned()
        {
            base.Spawned();

            playerRef = Object.InputAuthority;

            
            if (HasInputAuthority)
            {
                Name = string.Format("Player_{0}", Object.InputAuthority.PlayerId);
                RpcSetName(string.Format("Player_{0}", Object.InputAuthority.PlayerId));
                Local = this;

                /****************** Test ******************/
                if (SessionManager.Instance.Runner.IsClient)
                {
                    RpcSetCharacterId((byte)0);
                }


            }
                       
            
        }

        
        /// <summary>
        /// Called when the server despawns this player
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="hasState"></param>
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
          
            OnDespawned?.Invoke(this);
        }

        public override string ToString()
        {
            return string.Format("[Player - Name:{0}, Ready:{1}]", Name, Ready);
        }

        #region rpc calls

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        void RpcSetName(string name)
        {
            Name = name;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RpcSetReady(bool ready)
        {
            Ready = ready;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RpcSetCharacterId(byte characterId)
        {
            CharacterId = characterId;
        }
              

        #endregion

        #region networked OnChanged() calls
        
        public static void OnReadyChanged(Changed<Player> changed)
        {
            OnReadyChangedCallback?.Invoke(changed.Behaviour);
        }

        public static void OnNameChanged(Changed<Player> changed)
        {
            OnNameChangedCallback?.Invoke(changed.Behaviour);
        }

        #endregion

    }

}
