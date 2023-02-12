using Fusion;
using Fusion.Sockets;
using GOA.Assets;
using GOA.Level;
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
        // Callbacks
        public static UnityAction<NetworkRunner, PlayerRef> OnPlayerJoinedCallback;
        public static UnityAction<NetworkRunner, PlayerRef> OnPlayerLeftCallback;
        public static UnityAction<String> OnStartSessionFailed;
        public static UnityAction<NetworkRunner, ShutdownReason> OnShutdownCallback;
        public static UnityAction<NetworkRunner, List<SessionInfo>> OnSessionListUpdatedCallback;
        public static UnityAction<SessionLobby> OnLobbyJoint;
        public static UnityAction<SessionLobby, string> OnLobbyJoinFailed;
        
        [SerializeField]
        NetworkObject playerPrefab;

        [SerializeField]
        NetworkObject gameManagerPrefab;
    
        public const int MaxPlayers = 3;

        public static SessionManager Instance { get; private set; }


        NetworkRunner runner;
        
        
        NetworkSceneManagerDefault sceneManager;
        
        List<SessionInfo> sessionList = new List<SessionInfo>();
        public IList<SessionInfo> SessionList
        {
            get { return sessionList.AsReadOnly(); }
        }

        bool loading = false;

        bool resumingFromHostMigration = false;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                
                sceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void LateUpdate()
        {
            // Check for the match to start
            if (runner != null && runner.IsServer && (runner.SessionInfo.IsValid || runner.GameMode == GameMode.Single ) && !loading)
            {
                // Is the room full?
                if (ReadyToPlay())
                    StartCoroutine(StartMatch());
            }
    
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                //if(GameManager.Instance)
                //    Debug.Log("GameSeed:" + GameManager.Instance.GameSeed);
                //else
                //    Debug.Log("ERROR - GameManager not found");
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                //if (GameManager.Instance)
                //    runner.Despawn(GameManager.Instance.GetComponent<NetworkObject>());
                //else
                //    Debug.Log("ERROR - GameManager not found");
            }

            
        }

        IEnumerator StartMatch()
        {
            //MatchManager.Instance.MatchSeed = (int)UnityEngine.Random.Range(-Mathf.Infinity, Mathf.Infinity);
            
            
            
            loading = true;
            float delay = 2f;
            yield return new WaitForSeconds(delay);
            
            if (ReadyToPlay())
            {
                FindObjectOfType<GameManager>().CreateNewSeed();
                // Load game scene
                runner.SetActiveScene(1);
            }
            else
            {
                loading = false;
            }
        }

        bool ReadyToPlay()
        {

            // Check if all the players are ready
            Player[] players = FindObjectsOfType<Player>();

            // Is the room full?
            // If a player quit the game the session info is not updated soon, so we must check the actual number of players 
            if (runner.GameMode != GameMode.Single && ( runner.SessionInfo.PlayerCount < runner.SessionInfo.MaxPlayers || players.Length < runner.SessionInfo.MaxPlayers))
                return false;
            
            foreach (Player player in players)
            {
                if (!player.Ready)
                    return false;
            }

            return true;
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

        public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.LogFormat("SessionManager - OnHostMigration");

            await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

            runner = gameObject.AddComponent<NetworkRunner>();
            
            StartGameResult result = await runner.StartGame(new StartGameArgs()
            {
                HostMigrationToken = hostMigrationToken,   // contains all necessary info to restart the Runner
                                                           //HostMigrationResume = HostMigrationResume, // this will be invoked to resume the simulation
                                                           // other args
                HostMigrationResume = HostMigrationResume
            }); 
        }

        
        void HostMigrationResume(NetworkRunner runner)
        {
            Debug.Log("HostMigrationResume");

            resumingFromHostMigration = true;

            // Destroy the game manager
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm)
                DestroyImmediate(gm.gameObject);

            // Destroy all the puzzle controllers
            PuzzleController[] pcl = FindObjectsOfType<PuzzleController>();
            foreach(PuzzleController pc in pcl)
            {
                DestroyImmediate(pc.gameObject);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            
            if(PlayerInput.Instance)
                input.Set(PlayerInput.Instance.GetInput());
            
            
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.LogFormat("SessionManager - OnPlayerJoint: {0}", player);

            if (!resumingFromHostMigration)
            {
                // The server spawns the new player
                if (runner.IsServer)
                {
                    NetworkObject playerObj = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);

                    // Create the game manager
                    GameManager gm = FindObjectOfType<GameManager>();
                    if (!gm)
                        runner.Spawn(gameManagerPrefab, Vector3.zero, Quaternion.identity, player);
                }

                OnPlayerJoinedCallback?.Invoke(runner, player);
            }
            else
            {
                if (runner.IsServer)
                {
                    Debug.Log("ResumeSnapshot.Count:" + new List<NetworkObject>(runner.GetResumeSnapshotNetworkObjects()).Count);

                    //new List<NetworkObject>(runner.GetResumeSnapshotNetworkObjects()).FindAll(o=>o.GetBehaviour<)
                    foreach (var resNO in runner.GetResumeSnapshotNetworkObjects())
                    {
                        // The old player-0 becomes the new host, so the old player-1 becomes the new player-0 and so on.
                        int resNOPlayerId = 0;
                        if (player.PlayerId < runner.SessionInfo.MaxPlayers)
                            resNOPlayerId = player.PlayerId + 1;
                            
                        // Player
                        if (resNO.TryGetBehaviour<Player>(out var ppOut))
                        {
                            Debug.Log("Found player to resume -> playerId:" + resNO.InputAuthority.PlayerId);
                            if (resNO.InputAuthority.PlayerId == resNOPlayerId)
                            {
                                runner.Spawn(resNO, inputAuthority: player,
                                    onBeforeSpawned: (runner, newNO) =>
                                    {

                                        // One key aspects of the Host Migration is to have a simple way of restoring the old NetworkObjects state
                                        // If all state of the old NetworkObject is all what is necessary, just call the NetworkObject.CopyStateFrom
                                        newNO.CopyStateFrom(resNO);

                                        // and/or

                                        // If only partial State is necessary, it is possible to copy it only from specific NetworkBehaviours
                                        if (resNO.TryGetBehaviour<NetworkBehaviour>(out var myCustomNetworkBehaviour))
                                        {
                                            newNO.GetComponent<NetworkBehaviour>().CopyStateFrom(myCustomNetworkBehaviour);
                                        }
                                    });

                                //NetworkObject playerObj = runner.Spawn(resNO, Vector3.zero, Quaternion.identity, player);
                            }
                        }

                        // Character controller
                        if (resNO.TryGetBehaviour<NetworkCharacterControllerPrototypeCustom>(out var pOut))
                        {

                            Debug.Log("Found player to resume -> playerId:" + resNO.InputAuthority.PlayerId);
                            if (resNO.InputAuthority.PlayerId == resNOPlayerId)
                            {
                                runner.Spawn(resNO, position: pOut.ReadPosition(), rotation: pOut.ReadRotation(), inputAuthority: player,
                                    onBeforeSpawned: (runner, newNO) =>
                                        {

                                            // One key aspects of the Host Migration is to have a simple way of restoring the old NetworkObjects state
                                            // If all state of the old NetworkObject is all what is necessary, just call the NetworkObject.CopyStateFrom
                                            newNO.CopyStateFrom(resNO);

                                            // and/or

                                            // If only partial State is necessary, it is possible to copy it only from specific NetworkBehaviours
                                            if (resNO.TryGetBehaviour<NetworkBehaviour>(out var myCustomNetworkBehaviour))
                                            {
                                                newNO.GetComponent<NetworkBehaviour>().CopyStateFrom(myCustomNetworkBehaviour);
                                            }
                                        });

                                
                            }
                        }

                        // Game manager
                        if (resNO.TryGetBehaviour<GameManager>(out var gmOut))
                        {
                            Debug.Log("Found Object: GameManager");
                            if (player.PlayerId >= runner.SessionInfo.MaxPlayers)
                            {
                                runner.Spawn(resNO, inputAuthority: player,
                                    onBeforeSpawned: (runner, newNO) =>
                                    {

                                        // One key aspects of the Host Migration is to have a simple way of restoring the old NetworkObjects state
                                        // If all state of the old NetworkObject is all what is necessary, just call the NetworkObject.CopyStateFrom
                                        newNO.CopyStateFrom(resNO);

                                        // and/or

                                        // If only partial State is necessary, it is possible to copy it only from specific NetworkBehaviours
                                        if (resNO.TryGetBehaviour<NetworkBehaviour>(out var myCustomNetworkBehaviour))
                                        {
                                            newNO.GetComponent<NetworkBehaviour>().CopyStateFrom(myCustomNetworkBehaviour);
                                        }
                                    });

                                
                            }
                        }

                        // Puzzle controller
                        if (resNO.TryGetBehaviour<PuzzleController>(out var pzOut))
                        {
                            Debug.Log("Found Object: PuzzleController");
                            if (player.PlayerId >= runner.SessionInfo.MaxPlayers)
                            {
                                runner.Spawn(resNO, inputAuthority: player,
                                    onBeforeSpawned: (runner, newNO) =>
                                    {

                                        // One key aspects of the Host Migration is to have a simple way of restoring the old NetworkObjects state
                                        // If all state of the old NetworkObject is all what is necessary, just call the NetworkObject.CopyStateFrom
                                        newNO.CopyStateFrom(resNO);

                                        // and/or

                                        // If only partial State is necessary, it is possible to copy it only from specific NetworkBehaviours
                                        if (resNO.TryGetBehaviour<NetworkBehaviour>(out var myCustomNetworkBehaviour))
                                        {
                                            newNO.GetComponent<NetworkBehaviour>().CopyStateFrom(myCustomNetworkBehaviour);
                                        }
                                    });
                            }
                        }
                    }
                }
            }
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
            if(runner.CurrentScene > 0) // Game scene
            {
                // Build both on clients and server
                FindObjectOfType<LevelBuilder>().Build(FindObjectOfType<GameManager>().GameSeed);

                if (runner.IsServer)
                {

                    //GameObject.FindObjectOfType<Level.LevelBuilder>().Build(runner);

                    // Create a character for each human player
                    // Load the asset collection
                    List<CharacterAsset> assets = new List<CharacterAsset>(Resources.LoadAll<CharacterAsset>(CharacterAsset.ResourceFolder));
                    // Find all the human players
                    Player[] players = FindObjectsOfType<Player>();
                    // Init spawn data
                    int spIndex = 0;
                    GameObject[] spawnPointList = GameObject.FindGameObjectsWithTag("PlayerSpawnPoint");

                    // Spawn characters
                    foreach (Player player in players)
                    {
                        // Get the character asset 
                        CharacterAsset asset = assets.Find(a => a.CharacterId == player.CharacterId);

                        // Get the next spown point
                        Transform sp = null;
                        sp = spawnPointList[spIndex].transform;
                        spIndex++;
                        
                        // Spawn
                        NetworkObject character = runner.Spawn(asset.CharacterPrefab, sp.position, sp.rotation, player.PlayerRef);
                    }
                }

                
            }
            else // Menu scene
            {
                // Destroy all characters
                PlayerController[] characters = FindObjectsOfType<PlayerController>();
                for(int i=0; i<characters.Length; i++)
                {
                    Destroy(characters[i].gameObject);
                }
            }
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
            if(shutdownReason == ShutdownReason.HostMigration)
            {
                Debug.LogFormat("Runner shutdown for host migration");
                DestroyImmediate(runner);
                runner = null;
            }
            else
            {
                resumingFromHostMigration = false;
                // Destroy all the player objects
                Player[] players = FindObjectsOfType<Player>();
                for (int i = 0; i < players.Length; i++)
                {
                    Destroy(players[i].gameObject);
                }
                // Destroy match manager
                GameManager gm = FindObjectOfType<GameManager>();
                if (gm)
                    Destroy(gm.gameObject);
                // Reset runner
                DestroyImmediate(runner);
                runner = null;
                OnShutdownCallback?.Invoke(runner, shutdownReason);
            }
           
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

        #region spawn methods

        #endregion

        #region private methods
        async void StartSession(StartGameArgs args)
        {
            loading = false;

                            
            // Create the runner
            if (!runner)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }

            
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
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }
            
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
