using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOA
{
    public class Player: NetworkBehaviour
    {
        public static UnityAction<Player> OnTeamChangedCallback;
        public static UnityAction<Player> OnDespawned;
        public static UnityAction<Player> OnReadyChangedCallback;

        [UnitySerializeField]
        [Networked] public string Name { get; private set; }

        [Networked(OnChanged = nameof(OnReadyChanged))] public NetworkBool Ready { get; private set; }

        /// <summary>
        /// 0: none
        /// 1: home
        /// 2: away
        /// </summary>
        [Networked(OnChanged = nameof(OnTeamChanged))] public byte TeamId { get; private set; } = 0;

        public bool HasTeam
        {
            get { return TeamId > 0; }
        }

        PlayerRef playerRef;
        public PlayerRef PlayerRef
        {
            get { return playerRef; }
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
                RpcSetTeam(0);

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
        public void RpcSetTeam(byte team)
        {
            TeamId = team;
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RpcSetReady(bool ready)
        {
            Ready = ready;
        }

        #endregion

        #region networked OnChanged() calls
        public static void OnTeamChanged(Changed<Player> changed)
        {
            Debug.LogFormat("OnTeamChanged:{0}", changed.Behaviour.Name);
            OnTeamChangedCallback?.Invoke(changed.Behaviour);
        }

        public static void OnReadyChanged(Changed<Player> changed)
        {
            Debug.LogFormat("OnTeamChanged:{0}", changed.Behaviour.Name);
            OnReadyChangedCallback?.Invoke(changed.Behaviour);
        }
        #endregion

    }

}
