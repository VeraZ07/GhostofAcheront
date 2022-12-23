using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GOA.UI
{
    public class LobbyPanel : MonoBehaviour
    {
        [SerializeField]
        Button buttonReady;
        
        [SerializeField]
        Button buttonQuitMatch;

        [SerializeField]
        Button buttonJoinHomeTeam;

        [SerializeField]
        Button buttonJoinAwayTeam;

        [SerializeField]
        List<PlayerItem> homePlayerList;

        [SerializeField]
        List<PlayerItem> awayPlayerList;

        //[Networked] NetworkBool Ready { get; set; }



        private void Awake()
        {
            buttonQuitMatch.onClick.AddListener(()=>SessionManager.Instance.QuitSession());
            buttonJoinHomeTeam.onClick.AddListener(TryJoinHomeTeam);
            buttonJoinAwayTeam.onClick.AddListener(TryJoinAwayTeam);
            buttonReady.onClick.AddListener(ToggleReady);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            SessionManager.OnShutdownCallback += HandleOnShutdown;
            Player.OnTeamChangedCallback += HandleOnTeamJoined;
            Player.OnDespawned += HandleOnPlayerDespawned;
            Player.OnReadyChangedCallback += HandleOnReadyChanged;

            ClearAll();

            // Get session if exists
            
            NetworkRunner runner = FindObjectOfType<NetworkRunner>();
            if (runner && runner.SessionInfo)
            {
                List<Player> players = new List<Player>(FindObjectsOfType<Player>());
                foreach(PlayerRef pRef in runner.ActivePlayers)
                {
                    // Get the player
                    Debug.Log("LoggedPlayer.Count:" + players.Count);
                    Player player = players.Find(p => p.PlayerRef == pRef);

                    Debug.LogFormat("LobbyPanel - PlayerName:{0}", player.Name);

                    if(player.HasTeam)
                    {
                        HandleOnTeamJoined(player);
                        //buttonReady.interactable = true;
                    }
                    else
                    {
                        CheckJoinTeamButtons();
                    //    buttonReady.GetComponentInChildren<TMP_Text>().text = "Not Ready";
                    //    buttonReady.interactable = false;
                    }

                    HandleOnReadyChanged(player);
                    //SetReadyButton(false);
                }

            }

        }

        private void OnDisable()
        {
            SessionManager.OnShutdownCallback -= HandleOnShutdown;
            Player.OnTeamChangedCallback -= HandleOnTeamJoined;
            Player.OnDespawned -= HandleOnPlayerDespawned;
        }

        void ToggleReady()
        {
            NetworkRunner runner = FindObjectOfType<NetworkRunner>();
            Player player = new List<Player>(FindObjectsOfType<Player>()).Find(p => p.PlayerRef == runner.LocalPlayer);
            if (!player.HasTeam)
                return;

            player.RpcSetReady(!player.Ready);
        }
         

       void HandleOnTeamJoined(Player player)
        {
            // Join the team
            List<PlayerItem> items = player.TeamId == 1 ? homePlayerList : awayPlayerList;
            foreach (PlayerItem item in items)
            {
                if (item.IsEmpty)
                {
                    item.SetPlayer(player);
                    break;
                }
            }

            // Check buttons
            CheckJoinTeamButtons();

            buttonReady.interactable = true;
        }



        void HandleOnPlayerDespawned(Player player)
        {
            if (!player.HasTeam)
                return;

            List<PlayerItem> playerItemList = player.TeamId == TeamManager.HomeTeamId ? homePlayerList : awayPlayerList;
            PlayerItem item = playerItemList.Find(p => p.Player == player);
            if (item)
            {
                item.Reset();
            }

            // Check buttons
            CheckJoinTeamButtons();
        }

        void HandleOnReadyChanged(Player player)
        {
            NetworkRunner runner = FindObjectOfType<NetworkRunner>();
            
            // Is local player?
            if(player.PlayerRef == runner.LocalPlayer)
            {
                // Check the ready button
                if (!player.Ready)
                {
                    buttonReady.GetComponentInChildren<TMP_Text>().text = "Not Ready";
                }
                else
                {
                    buttonReady.GetComponentInChildren<TMP_Text>().text = "Ready";
                }

                buttonReady.interactable = player.HasTeam ? true : false;

            }
                       
            // For all
            if (player.HasTeam)
            {
                List<PlayerItem> items = player.TeamId == TeamManager.HomeTeamId ? homePlayerList : awayPlayerList;
                PlayerItem item = items.Find(p => p.Player == player);
                item.SetReady(player.Ready);
            }
            
        }

        void HandleOnShutdown(NetworkRunner runner, ShutdownReason reason)
        {
            GetComponentInParent<MainMenu>().ActivateMainPanel();
        }

      

        void CheckJoinTeamButtons()
        {
            NetworkRunner runner = FindObjectOfType<NetworkRunner>();
            
            // Is the local player already logged?
            
            Player localPlayer = new List<Player>(FindObjectsOfType<Player>()).Find(p => p.PlayerRef == runner.LocalPlayer);
            if(localPlayer.HasTeam)
            {
                buttonJoinHomeTeam.interactable = false;
                buttonJoinAwayTeam.interactable = false;
                return;
            }

            // Home team
            if(TeamManager.HomeTeamIsFull())
            {
                buttonJoinHomeTeam.interactable = false;
            }
            else
            {
                buttonJoinHomeTeam.interactable = true;
            }

            if (TeamManager.AwayTeamIsFull())
            {
                buttonJoinAwayTeam.interactable = false;
            }
            else
            {
                buttonJoinAwayTeam.interactable = true;
            }

        }

        void ClearAll()
        {
            foreach (PlayerItem player in homePlayerList)
                player.Reset();

            foreach (PlayerItem player in awayPlayerList)
                player.Reset();

            
        }

        bool TryJoinTeam(byte team)
        {
            
            // Check if the team is full
            NetworkRunner runner = FindObjectOfType<NetworkRunner>();
      
            // The button should be disabled at this point... who knows
            if (TeamManager.TeamIsFull(team) && runner.GameMode != GameMode.Single)
                return false;

            Player player = new List<Player>(FindObjectsOfType<Player>()).Find(p => p.PlayerRef == runner.LocalPlayer);
            player.RpcSetTeam(team);

            return true;
        }

        void TryJoinHomeTeam()
        {
            if (!TryJoinTeam(TeamManager.HomeTeamId))
            {
                Debug.LogWarning("Can't join home team.");
            }
            

            CheckJoinTeamButtons();
        }

        void TryJoinAwayTeam()
        {
            if (!TryJoinTeam(TeamManager.AwayTeamId))
            {
                Debug.LogWarning("Can't join away team.");
            }
            
            CheckJoinTeamButtons();
        }
    }

}
