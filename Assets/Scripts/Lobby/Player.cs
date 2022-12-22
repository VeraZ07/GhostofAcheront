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

        [UnitySerializeField]
        [Networked] public string Name { get; private set; }

        [Networked] public NetworkBool Ready { get; private set; }

        /// <summary>
        /// 0: none
        /// 1: home
        /// 2: away
        /// </summary>
        [Networked(OnChanged = nameof(OnTeamChanged))] public byte Team { get; private set; } = 0;


        private void Update()
        {
            if (!HasInputAuthority)
                return;

            if (Input.GetKeyDown(KeyCode.A))
            {

                RpcSetName("AAAAAAAAAAAA");
                
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                RpcSetName("BBBBBBB");
                
            }
        }

        /// <summary>
        /// Call when the server spawns this player
        /// </summary>
        public override void Spawned()
        {
            base.Spawned();
            
            if (Runner.IsClient)
            {
                // Add the player to the local session manager dictionary
                SessionManager.Instance.LoggedPlayers.Add(Object.InputAuthority, GetComponent<NetworkObject>());
            }
            
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
           
            if (Runner.IsClient)
            {
                // Remove from the local sassion manager dictionary
                if (SessionManager.Instance.LoggedPlayers.ContainsValue(GetComponent<NetworkObject>()))
                {
                    // We are no longer able to access the PlayerRef who has input authority over this object, so we need to 
                    // use the network object that still exists.
                    PlayerRef key = new List<PlayerRef>(SessionManager.Instance.LoggedPlayers.Keys).Find(k => SessionManager.Instance.LoggedPlayers[k] == GetComponent<NetworkObject>());

                    // Remove from dictionary
                    SessionManager.Instance.LoggedPlayers.Remove(key);

                }
            }
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
            Team = team;
        }

        #endregion

        #region networked OnChanged() calls
        public static void OnTeamChanged(Changed<Player> changed)
        {
            Debug.LogFormat("OnTeamChanged:{0}", changed.Behaviour.Name);
            OnTeamChangedCallback?.Invoke(changed.Behaviour);
        }
        #endregion

    }

}
