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
using static GOA.Level.LevelBuilder;

namespace GOA
{
    public class SessionManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        // Callbacks
        public static UnityAction<NetworkRunner, PlayerRef> OnPlayerJoinedCallback;
        public static UnityAction<NetworkRunner, PlayerRef> OnPlayerLeftCallback;
        public static UnityAction<String> OnStartSessionFailed;
        public static UnityAction<ShutdownReason> OnShutdownCallback;
        public static UnityAction<NetworkRunner, List<SessionInfo>> OnSessionListUpdatedCallback;
        public static UnityAction<SessionLobby> OnLobbyJoint;
        public static UnityAction<SessionLobby, string> OnLobbyJoinFailed;
        


        [SerializeField]
        NetworkObject playerPrefab;

        [SerializeField]
        NetworkObject inventoryPrefab;

        [SerializeField]
        NetworkObject gameManagerPrefab;

    
    
        public const int MaxPlayers = 2;

        public static SessionManager Instance { get; private set; }


        NetworkRunner runner;

        bool sharedMode = true;

        public NetworkRunner Runner
        {
            get 
            {
                if (!runner)
                    runner = GetComponent<NetworkRunner>();

                return runner;
            }
        }
        
        
        NetworkSceneManagerDefault sceneManager;
        
        List<SessionInfo> sessionList = new List<SessionInfo>();
        public IList<SessionInfo> SessionList
        {
            get { return sessionList.AsReadOnly(); }
        }

        bool loading = false;

#if USE_HOST_MIGRATION
        bool resumingFromHostMigration = false;
#endif
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
            if (runner != null && (runner.IsServer || runner.IsSharedModeMasterClient) && (runner.SessionInfo.IsValid || runner.GameMode == GameMode.Single ) && !loading)
            {
                // Is the room full?
                if (ReadyToPlay())
                    StartCoroutine(StartMatch());
            }
    
        }

        void Update()
        {
                       
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
                // Set the current session closed and not visible
                runner.SessionInfo.IsVisible = false;
                runner.SessionInfo.IsOpen = false;
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
            if (runner.GameMode != GameMode.Single && ( runner.SessionInfo.PlayerCount < 2/*runner.SessionInfo.MaxPlayers*/ || players.Length < 2/*runner.SessionInfo.MaxPlayers*/))
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
            Debug.LogFormat("SessionManager - OnDisconnectedFromServer.");
        }

#if USE_HOST_MIGRATION
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

            // Destroy all networked objects
            NetworkBehaviour[] nbs = FindObjectsOfType<NetworkBehaviour>();
            int count = nbs.Length;
            for (int i = 0; i < count; i++)
                DestroyImmediate(nbs[0].gameObject);

           
        }
#else
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
           
        }

#endif
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
#if USE_HOST_MIGRATION
            if (!resumingFromHostMigration)
            {
#endif
            // The server spawns the new player
            //if (runner.IsServer)

            if (runner.IsServer || runner.IsSharedModeMasterClient)

            {
                    NetworkObject playerObj = runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player,
                        (r,o) => 
                        {
                            o.GetComponent<Player>().Init(player);
                        });

                    // Create the game manager
                    GameManager gm = FindObjectOfType<GameManager>();
                    if (!gm)
                        runner.Spawn(gameManagerPrefab, Vector3.zero, Quaternion.identity, player);
            }

                OnPlayerJoinedCallback?.Invoke(runner, player);
#if USE_HOST_MIGRATION
            }
            else
            {
                
                if (runner.IsServer)
                {

                    Debug.Log("ResumeSnapshot.Count:" + new List<NetworkObject>(runner.GetResumeSnapshotNetworkObjects()).Count);

                    Inventory oldHostInventory = null;
                    LevelBuilder builder = FindObjectOfType<LevelBuilder>();

                    //new List<NetworkObject>(runner.GetResumeSnapshotNetworkObjects()).FindAll(o=>o.GetBehaviour<)
                    foreach (var resNO in runner.GetResumeSnapshotNetworkObjects())
                    {
                        // The old player-0 becomes the new host, so the old player-1 becomes the new player-0 and so on.
                        // It works only for two players at the moment
                        int oldPlayerId = 0; 
                        //if (player.PlayerId < runner.SessionInfo.MaxPlayers)
                        //    oldPlayerId = player.PlayerId + 1;
                        //else
                        //    oldPlayerId = runner.SessionInfo.MaxPlayers - 1;
                        
                        // Player
                        if (resNO.TryGetBehaviour<Player>(out var ppOut))
                        {
                            Debug.Log("Found player to resume -> playerId:" + resNO.InputAuthority.PlayerId);
                            if (resNO.InputAuthority.PlayerId == oldPlayerId)
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

                            Debug.Log("Found character controller to resume -> playerId:" + resNO.InputAuthority.PlayerId);
                            if (resNO.InputAuthority.PlayerId == oldPlayerId)
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

                                            newNO.GetComponent<PlayerController>().Init(player.PlayerId);

                                        });

                                
                            }
                        }

                        

                        // Game manager
                        if (resNO.TryGetBehaviour<GameManager>(out var gmOut))
                        {
                            Debug.Log("Found game manager to resume");
                            //if (player.PlayerId >= runner.SessionInfo.MaxPlayers)
                            // We only create the game manager when the local player ( which is the server in this case ) joins 
                            // the match.
                            if(player == runner.LocalPlayer) 
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
                            Debug.Log("Found PuzzleController to resume");
                            //if (player.PlayerId >= runner.SessionInfo.MaxPlayers)
                            if (player == runner.LocalPlayer)
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

                        // Inventory
                        if (resNO.TryGetBehaviour<Inventory>(out var piOut))
                        {
                            
                            Debug.Log("Found Inventory to resume");
                            //if ((player.PlayerId >= runner.SessionInfo.MaxPlayers && piOut.PlayerId == runner.SessionInfo.MaxPlayers-1) || player.PlayerId == piOut.PlayerId - 1)
                            //if ((player == runner.LocalPlayer && piOut.PlayerId == 0) || player.PlayerId == piOut.PlayerId - 1)
                            //if ((player == runner.LocalPlayer && piOut.Object.InputAuthority.PlayerId == 0) || player.PlayerId == piOut.Object.InputAuthority.PlayerId - 1)
                            if (resNO.InputAuthority.PlayerId == oldPlayerId)
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

                                        //newNO.GetComponent<Inventory>().Init(player.PlayerId);
                                    });
                            }
                            else
                            {
                                oldHostInventory = piOut;
                            }
                            // Manage the items of the previous host here
                            // ...
                            // ...
                            // ...
                            // ...


                        }

                        if (resNO.TryGetBehaviour<Picker>(out var pickOut))
                        {
                            Debug.Log("Found Picker to resume");
                            if (player == runner.LocalPlayer)
                            {
                                int customObjectId = pickOut.CustomObjectId;
                                CustomObject co = builder.CustomObjects[pickOut.CustomObjectId];
                                Tile tile = builder.GetTile(co.TileId);
                                Vector3 pos = tile.GetPosition();

                                runner.Spawn(resNO, inputAuthority: player, position: pos, rotation: Quaternion.identity,
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

                        // Monster
                        // Game manager
                        if (resNO.TryGetBehaviour<MonsterController>(out var mcOut))
                        {
                            Debug.Log("Found monster controller to resume");
                            //if (player.PlayerId >= runner.SessionInfo.MaxPlayers)
                            // We only create the game manager when the local player ( which is the server in this case ) joins 
                            // the match.
                            NetworkTransform nt = mcOut.GetComponent<NetworkTransform>();
                            if (player == runner.LocalPlayer)
                            {
                                runner.Spawn(resNO, inputAuthority: null, position: nt.ReadPosition(), rotation: nt.ReadRotation(),
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

                                        newNO.GetComponent<MonsterController>().Init();
                                    });


                            }
                        }
                    }

                    // At this point we need to check for the inventry to resume
                    StartCoroutine(RespawnOldHostItems(oldHostInventory));
                }
            }
#endif
        }

#if USE_HOST_MIGRATION
        IEnumerator RespawnOldHostItems(Inventory inventory)
        {
            yield return new WaitForEndOfFrame();
            inventory.RespawnAllItems();
        }
#endif
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
        {
            Debug.LogFormat("SessionManager - OnPlayerLeft: {0}", playerRef);

            // The server despawns the player
            if (runner.IsServer || runner.IsSharedModeMasterClient)
            {
                // Desapwn controller
                PlayerController playerController = new List<PlayerController>(FindObjectsOfType<PlayerController>()).Find(p => p.Object.InputAuthority == playerRef);
                if (playerController)
                    runner.Despawn(playerController.GetComponent<NetworkObject>());

                // Despawn player
                Player player = new List<Player>(FindObjectsOfType<Player>()).Find(p=>p.PlayerRef == playerRef);
                if (player)
                    runner.Despawn(player.GetComponent<NetworkObject>());

                // Manage the items of the disconnected player here
                // ...
                Inventory inv = new List<Inventory>(FindObjectsOfType<Inventory>()).Find(i => i.Object.InputAuthority == playerRef);
                if (inv)
                {
                    inv.RespawnAllItems();
                    runner.Despawn(inv.GetComponent<NetworkObject>());
                }

#if USE_HOST_MIGRATION
                SessionManager.Instance.PushSnapshot();
#endif          
                List<PlayerController> players = new List<PlayerController>(FindObjectsOfType<PlayerController>());
                PlayerController local = players.Find(p => p.HasInputAuthority);
                if(local.State != (int)PlayerState.Sacrificed && local.State != (int)PlayerState.Escaped)
                {
                    // Player quit the game, so we do the same
                    QuitSession(ShutdownReason.GameClosed);
                }
                else
                {
                    // I'm the last one
                    if (players.Count == 1)
                        QuitSession();
                }
                

                //StartCoroutine(CheckDeadOrAliveDelayed());
            }
            else // Client side
            {
                List<PlayerController> players = new List<PlayerController>(FindObjectsOfType<PlayerController>());
                PlayerController local = players.Find(p => p.HasInputAuthority);
                if (local.State != (int)PlayerState.Sacrificed && local.State != (int)PlayerState.Escaped)
                {
                    // Player quit the game, so we do the same
                    QuitSession(ShutdownReason.GameClosed);
                }
            }


            OnPlayerLeftCallback?.Invoke(runner, playerRef);
            
        }

        IEnumerator CheckDeadOrAliveDelayed()
        {
            yield return new WaitForEndOfFrame();

            if (SessionManager.Instance.Runner.IsServer || SessionManager.Instance.Runner.IsSharedModeMasterClient)
                FindObjectOfType<GameManager>().CheckForEscaped();
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
                InteractableManager.Instance.Init();

                if (runner.IsServer || runner.IsSharedModeMasterClient)
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
                        NetworkObject character = runner.Spawn(asset.CharacterPrefab, sp.position, sp.rotation, player.PlayerRef, 
                            (r,o) => {
                                o.GetComponent<PlayerController>().Init(player.PlayerRef.PlayerId);
                            });

                        // Each character has an inventory attached to it. Server has authority on all inventories.
                        runner.Spawn(inventoryPrefab, Vector3.zero, Quaternion.identity, player.PlayerRef, 
                            (r, o) => {
                                //o.GetComponent<Inventory>().Init(player.PlayerRef.PlayerId);
                            });


                    
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
                // Destroy all inventories
                Inventory[] inventories = FindObjectsOfType<Inventory>();
                for(int i=0; i<inventories.Length; i++)
                {
                    Destroy(inventories[i].gameObject);
                }
            }
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            this.sessionList = sessionList;
            OnSessionListUpdatedCallback?.Invoke(runner, sessionList);
        }
        
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("Shutting down session:" + shutdownReason);
            OnShutdownCallback?.Invoke(shutdownReason);
            
#if USE_HOST_MIGRATION
            if(shutdownReason == ShutdownReason.HostMigration)
            {
                DestroyImmediate(runner);
                runner = null;
            }
            else
            {

            resumingFromHostMigration = false;    
#endif

            NetworkBehaviour[] nbs = FindObjectsOfType<NetworkBehaviour>();
                for (int i = 0; i < nbs.Length; i++)
                    Destroy(nbs[i].gameObject);
        
            
            DestroyImmediate(runner);
            runner = null;
            
            //if(UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(0) != UnityEngine.SceneManagement.SceneManager.GetActiveScene())
            //    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
#if USE_HOST_MIGRATION
            }
#endif

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

                GameMode = sharedMode ? GameMode.Shared : GameMode.Host,
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

                GameMode = sharedMode ? GameMode.Shared : GameMode.Client,
                SessionName = sessionName,
                //MatchmakingMode = Fusion.Photon.Realtime.MatchmakingMode.FillRoom,
                //PlayerCount = 1,
                //SceneManager = sceneManager,
                DisableNATPunchthrough = true 
            };

            StartSession(args);
        }

        public void QuitSession(ShutdownReason reason = ShutdownReason.Ok)
        {
            
            Task t = runner.Shutdown(false, reason, true).ContinueWith((t) =>
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

#if USE_HOST_MIGRATION
        public void PushSnapshot()
        {
            if (!Runner.IsServer || Runner.IsSinglePlayer)
                return;

            Runner.PushHostMigrationSnapshot().ContinueWith((t)=>
            {
                if (t.IsCompleted)
                {
                    if (t.IsFaulted)
                    {
                        Debug.LogErrorFormat("PushSnapshot failed:{0}", t.Exception.Message.ToString());
                    }
                    else
                    {
                        Debug.Log("PushSnapshot succeeded");
                    }
                }
            });
        }
#endif
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
