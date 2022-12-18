using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOA
{
    public class SessionManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public UnityAction<SessionInfo> OnSessionCreated;
        public UnityAction<String> OnSessionCreationFailed;
        public UnityAction OnSessionQuit;

        public const int MaxPlayers = 6;

        public static SessionManager Instance { get; private set; }



        NetworkRunner runner;
        //public NetworkRunner Runner
        //{
        //    get { return runner; }
        //}
        NetworkSceneManagerDefault sceneManager;

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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

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
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.LogFormat("SessionManager - OnPlayerLeft: {0}", player);
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
            
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Destroy(runner);
            OnSessionQuit?.Invoke();
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
                SceneManager = sceneManager
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
                IsVisible = !isPrivate
            };

            StartSession(args);
        }

        public void JoinSession(String matchName)
        {

        }

        public void QuitSession()
        {
            if (runner.IsServer)
            {
                runner.Shutdown(false, ShutdownReason.GameClosed, true);
            }
            else
            {
                //runner.Shutdown(false, ShutdownReason., true)
                runner.Shutdown(false, ShutdownReason.Ok, true);
            }
        }
        #endregion


        #region private methods
        async void StartSession(StartGameArgs args)
        {
            // Create the runner
            runner = gameObject.AddComponent<NetworkRunner>();

            // Start the new session
            var result = await runner.StartGame(args);

            if (result.Ok)
            {
                Debug.LogFormat("SessionManager - StartSession succeeded");
                LogSession();
                OnSessionCreated?.Invoke(runner.SessionInfo);
            }
            else
            {
                Debug.LogErrorFormat("SessionManager - StartSession failed - ErrorMessage:{0}", result.ErrorMessage);
                OnSessionCreationFailed?.Invoke(result.ErrorMessage);
            }
        }


        #endregion
    }

}
