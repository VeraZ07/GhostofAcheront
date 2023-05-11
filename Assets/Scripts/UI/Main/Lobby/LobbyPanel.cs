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
        Sprite readySprite;

        [SerializeField]
        Sprite notReadySprite;

        private void Awake()
        {
            buttonQuitMatch.onClick.AddListener(()=>SessionManager.Instance.QuitSession());
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
            SessionManager.OnPlayerJoinedCallback += HandleOnPlayerJoined;
            Player.OnDespawned += HandleOnPlayerDespawned;
            Player.OnReadyChangedCallback += HandleOnReadyChanged;
            Player.OnNameChangedCallback += HandleOnNameChanged;

            ClearAll();

            // Get session if exists
            
            NetworkRunner runner = FindObjectOfType<NetworkRunner>();
            if ( runner && ( runner.SessionInfo || runner.GameMode == GameMode.Single ) )
            {

                List<Player> players = new List<Player>(FindObjectsOfType<Player>());
                foreach(PlayerRef pRef in runner.ActivePlayers)
                {
                    // Get the player
                   
                    Player player = players.Find(p => p.PlayerRef == pRef);

                    // Set player item
                    PlayerItem playerItem = new List<PlayerItem>(GetComponentsInChildren<PlayerItem>()).Find(p => p.Player == null);
                    playerItem.SetPlayer(player);

                    HandleOnReadyChanged(player);
                    //SetReadyButton(false);
                }

            }

        }

        private void OnDisable()
        {
            SessionManager.OnShutdownCallback -= HandleOnShutdown;
            Player.OnDespawned -= HandleOnPlayerDespawned;
            SessionManager.OnPlayerJoinedCallback -= HandleOnPlayerJoined;
            Player.OnReadyChangedCallback -= HandleOnReadyChanged;
            Player.OnNameChangedCallback -= HandleOnNameChanged;
        }

        void ToggleReady()
        {
            NetworkRunner runner = FindObjectOfType<NetworkRunner>();
            Player player = new List<Player>(FindObjectsOfType<Player>()).Find(p => p.PlayerRef == runner.LocalPlayer);
            player.RpcSetReady(!player.Ready);
        }
       
        void HandleOnPlayerDespawned(Player player)
        {
            
            PlayerItem item = new List<PlayerItem>(GetComponentsInChildren<PlayerItem>()).Find(p => p.Player == player);
            if (item)
            {
                item.Reset();
            }

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
                    //buttonReady.GetComponentInChildren<TMP_Text>().text = "Not Ready";
                    buttonReady.GetComponent<Image>().sprite = readySprite;
                }
                else
                {
                    //buttonReady.GetComponentInChildren<TMP_Text>().text = "Ready";
                    buttonReady.GetComponent<Image>().sprite = notReadySprite;
                }

            }
                       
            // For all
            PlayerItem item = new List<PlayerItem>(GetComponentsInChildren<PlayerItem>()).Find(p => p.Player == player);
            item.SetReady(player.Ready);
            
        }

        void HandleOnNameChanged(Player player)
        {
            PlayerItem pItem = new List<PlayerItem>(GetComponentsInChildren<PlayerItem>()).Find(p => p.Player == player);
            pItem.SetPlayer(player);
        }

        void HandleOnShutdown(NetworkRunner runner, ShutdownReason reason)
        {
            GetComponentInParent<MainMenu>().ActivateMainPanel();
        }

        void HandleOnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
        {
            Player player = new List<Player>(FindObjectsOfType<Player>()).Find(p => p.PlayerRef == playerRef);

            Debug.LogFormat("LobbyPanel - PlayerName:{0}", player.Name);

            // Set player item
            PlayerItem playerItem = new List<PlayerItem>(GetComponentsInChildren<PlayerItem>()).Find(p => p.Player == null);
            playerItem.SetPlayer(player);

            HandleOnReadyChanged(player);
        }

        void ClearAll()
        {
            PlayerItem[] playerItemList = GetComponentsInChildren<PlayerItem>();
            foreach (PlayerItem player in playerItemList)
                player.Reset();

        }

       
    }

}
