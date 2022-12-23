using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace GOA
{
    public class SessionManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public static UnityAction<NetworkRunner, PlayerRef> OnPlayerJoinedCallback;
        public static UnityAction<NetworkRunner, PlayerRef> OnPlayerLeftCallback;
        public static UnityAction<String> OnStartSessionFailed;
        public static UnityAction<NetworkRunner, ShutdownReason> OnShutdownCallback;
        public static UnityAction<NetworkRunner, List<SessionInfo>> OnSessionListUpdatedCallback;
        public static UnityAction<SessionLobby> OnLobbyJoint;
        public static UnityAction<SessionLobby, string> OnLobbyJoinFailed;
        
        [SerializeField]
        NetworkObject playerPrefab;
    
        public const int MaxPlayers = 2;

        public static SessionManager Instance { get; private set; }


        NetworkRunner runner;
        
        
        NetworkSceneManagerDefault sceneManager;

        List<SessionInfo> sessionList = new List<SessionInfo>();
        public IList<SessionInfo> SessionList
        {
            get { return sessionList.AsReadOnly(); }
        }


        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                
                sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();

              
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void LogSession()
        {
            SessionInfo si = runner.SessionInfo;
            Debug.LogFormat("[Session - Name:{0}, IsOpen:{1}, IsVisible:{2}, MaxPlayers:{3}, Region:{4}]", si.Name, si.IsOpen, si.IsVisible, si.MaxPlayers, si.Region);
        }

        #region INetworkRunnerCallbacks implementation
        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.LogFormat("SessionManager - OnConnectedToServer.");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.LogErrorFormat("SessionManager - OnConnectFailed: {0}", reason.ToString());
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.LogFormat("SessionManager - OnDisconnectedToServer.");
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.LogFormat("SessionManager - OnPlayerJoint: {0}", player);

            // The server spawns the new player
            if(runner.IsServer)
            {
                NetworkObject playerObj = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
            }
            
            OnPlayerJoinedCallback?.Invoke(runner, player);
        }

        

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
        {
            Debug.LogFormat("SessionManager - OnPlayerLeft: {0}", playerRef);

            // The server despawns the player
            if (runner.IsServer)
            {
                Player player = new List<Player>(FindObjectsOfType<Player>()).Find(p=>p.PlayerRef == playerRef);
                if (player)
                    runner.Despawn(player.GetComponent<NetworkObject>());
              
            }

            OnPlayerLeftCallback?.Invoke(runner, playerRef);
            
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
            
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            this.sessionList = sessionList;
            Debug.LogFormat("SessionManager - OnSessionListUpdated - Counts:{0}", sessionList.Count);
            OnSessionListUpdatedCallback?.Invoke(runner, sessionList);
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            // Destroy all the player objects
            Player[] players = FindObjectsOfType<Player>();
            for(int i=0; i<players.Length; i++)
            {
                Destroy(players[i].gameObject);
            }

       
            Destroy(runner);
            OnShutdownCallback?.Invoke(runner, shutdownReason);
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            
        }
        #endregion


        #region public methods
        public void PlaySolo()
        {
            StartGameArgs args = new StartGameArgs()
            {
                GameMode = GameMode.Single,
                SessionName = "SoloGame",
                //MatchmakingMode = Fusion.Photon.Realtime.MatchmakingMode.FillRoom,
                //PlayerCount = 1,
                SceneManager = sceneManager,
                //DisableNATPunchthrough = false 
            };

            StartSession(args);
        }

        public void HostSession(bool isPrivate)
        {
            StartGameArgs args = new StartGameArgs()
            {
                GameMode = GameMode.Host,
                //SessionName = "",
                MatchmakingMode = Fusion.Photon.Realtime.MatchmakingMode.FillRoom,
                PlayerCount = MaxPlayers,
                SceneManager = sceneManager,
                DisableNATPunchthrough = true,
                IsVisible = !isPrivate
            };

            StartSession(args);
        }

        public void JoinSession(String sessionName)
        {
            StartGameArgs args = new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = sessionName,
                //MatchmakingMode = Fusion.Photon.Realtime.MatchmakingMode.FillRoom,
                //PlayerCount = 1,
                //SceneManager = sceneManager,
                DisableNATPunchthrough = true 
            };

            StartSession(args);
        }

        public void QuitSession()
        {
            
            Task t = runner.Shutdown(false, ShutdownReason.Ok, true).ContinueWith((t) =>
            {
                if (t.IsCompleted)
                {
                    Debug.Log("SessionManager - QuitSession succeeded.");
                }
                else
                {
                    Debug.Log("SessionManager - QuitSession failed.");
                }
            });
           
        }

        public void JoinDefaultLobby()
        {
            JoinLobby(SessionLobby.ClientServer);
        }

        public void LeaveLobby()
        {
            sessionList.Clear();
            runner.Shutdown(false, ShutdownReason.Ok, true);
        }

        

        #endregion


        #region private methods
        async void StartSession(StartGameArgs args)
        {
            // Create the runner
            if(!runner)
                runner = gameObject.AddComponent<NetworkRunner>();

            // Start the new session
            var result = await runner.StartGame(args);

            if (result.Ok)
            {
                Debug.LogFormat("SessionManager - StartSession succeeded");
                LogSession();
            }
            else
            {
                Debug.LogErrorFormat("SessionManager - StartSession failed - ErrorMessage:{0}", result.ErrorMessage);
                OnStartSessionFailed?.Invoke(result.ErrorMessage);
            }
        }

        async void JoinLobby(SessionLobby sessionLobby)
        {
            sessionList.Clear();

            if (!runner)
                runner = gameObject.AddComponent<NetworkRunner>();

            

            var result = await runner.JoinSessionLobby(sessionLobby);

            if (result.Ok)
            {
                Debug.LogFormat("SessionManager - Joined to lobby {0}", sessionLobby);
                OnLobbyJoint?.Invoke(sessionLobby);
            }
            else
            {
                Debug.LogFormat("SessionManager - Join to lobby failed - ErrorMessage:{0}", result.ErrorMessage);
                OnLobbyJoinFailed?.Invoke(sessionLobby, result.ErrorMessage);
            }
        }

        #endregion
    }

}
