using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GOA.UI
{
    public class LobbyPanel : MonoBehaviour
    {
        [SerializeField]
        Toggle buttonReady;
        
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
            //buttonReady.onValueChanged.AddListener(ToggleReady);
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
            Player.OnTeamChangedCallback += HandleOnTeamChanged;

            ClearAll();

            // Get session if exists
            SessionManager sessionManager = FindObjectOfType<SessionManager>();
            NetworkRunner runner = FindObjectOfType<NetworkRunner>();
            if (runner && runner.SessionInfo)
            {
                foreach(PlayerRef pRef in runner.ActivePlayers)
                {
                    // Get the player
                    Debug.Log("LoggedPlayer.Count:" + sessionManager.LoggedPlayers.Count);
                    Player player = sessionManager.LoggedPlayers[pRef].GetComponent<Player>();

                    Debug.LogFormat("LobbyPanel - PlayerName:{0}", player.Name);

                    if(player.Team != 0)
                    {
                        HandleOnTeamChanged(player);
                    }
                }

                // Check buttons
                int homeCount = runner.SessionInfo.MaxPlayers / 2;
                int awayCount = homeCount;
                foreach (PlayerRef pRef in runner.ActivePlayers)
                {
                    byte team = sessionManager.LoggedPlayers[pRef].GetComponent<Player>().Team;
                    if (team > 0)
                    {
                        if (team == 1)
                            homeCount--;
                        else
                            awayCount--;
                    }
                }
                buttonJoinHomeTeam.interactable = homeCount == 0 ? false : true;
                buttonJoinAwayTeam.interactable = awayCount == 0 ? false : true;
            }

        }

        private void OnDisable()
        {
            SessionManager.OnShutdownCallback -= HandleOnShutdown;
            Player.OnTeamChangedCallback -= HandleOnTeamChanged;
        }

        void ToggleReady(bool value)
        {
            
        }

       void HandleOnTeamChanged(Player player)
        {
            List<PlayerItem> items = player.Team == 1 ? homePlayerList : awayPlayerList;
            foreach (PlayerItem item in items)
            {
                if (item.IsEmpty)
                {
                    item.SetPlayer(player);
                    break;
                }
            }
        }



        void HandleOnShutdown(NetworkRunner runner, ShutdownReason reason)
        {
            GetComponentInParent<MainMenu>().ActivateMainPanel();
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
            SessionManager sessionManager = FindObjectOfType<SessionManager>();

            SessionInfo info = runner.SessionInfo;
            int count = info.MaxPlayers / 2;
            foreach(PlayerRef pRef in runner.ActivePlayers)
            {
                Player player = sessionManager.LoggedPlayers[pRef].GetComponent<Player>();
                if (player.Team == team)
                    count--;
            }

            if(count > 0) // Not full yet
            {
                // Set the player team
                Player player = sessionManager.LoggedPlayers[runner.LocalPlayer].GetComponent<Player>();
                player.RpcSetTeam(team);
              

                return true;
            }

            return false;
        }

        void TryJoinHomeTeam()
        {
            if (TryJoinTeam(1))
            {
                buttonJoinHomeTeam.interactable = false;
                buttonJoinAwayTeam.interactable = false;
            }
                
        }

        void TryJoinAwayTeam()
        {
            if (TryJoinTeam(2))
            {
                buttonJoinHomeTeam.interactable = false;
                buttonJoinAwayTeam.interactable = false;
            }

        }
    }

}
