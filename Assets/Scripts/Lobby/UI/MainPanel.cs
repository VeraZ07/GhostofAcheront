using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GOA.UI
{
    public class MainPanel : MonoBehaviour
    {
        [SerializeField]
        Button buttonSingleplayerMatch;

        [SerializeField]
        Button buttonMultiplayerMatch;

        [SerializeField]
        Button buttonOptions;

        [SerializeField]
        Button buttonExit;
                

        private void Awake()
        {
            
            // Set buttons
            buttonExit.onClick.AddListener(QuitGame);
            buttonSingleplayerMatch.onClick.AddListener(CreateSingleplayerMatch);
            buttonMultiplayerMatch.onClick.AddListener(CreateMultiplayerMatch);
            buttonOptions.onClick.AddListener(OpenOptionsPanel);
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
            if (SessionManager.Instance)
            {
                SessionManager.Instance.OnPlayerJoinedCallback += HandleOnPlayerJoined;
                SessionManager.Instance.OnStartSessionFailed += HandleOnStartSessionFailed;
                SessionManager.Instance.OnLobbyJoint += HandleOnLobbyJoint;
                SessionManager.Instance.OnLobbyJoinFailed += HandleOnLobbyJoinFailed;
            }

            EnableInput(true);
        }

        private void OnDisable()
        {
            SessionManager.Instance.OnPlayerJoinedCallback -= HandleOnPlayerJoined;
            SessionManager.Instance.OnStartSessionFailed -= HandleOnStartSessionFailed;
            SessionManager.Instance.OnLobbyJoint -= HandleOnLobbyJoint;
            SessionManager.Instance.OnLobbyJoinFailed -= HandleOnLobbyJoinFailed;
        }

        void CreateSingleplayerMatch()
        {
            EnableInput(false);

            // Create singleplayer match
            SessionManager.Instance.PlaySolo();
        }

        void CreateMultiplayerMatch()
        {
            EnableInput(false);

            SessionManager.Instance.JoinDefaultLobby();
        }

        void OpenOptionsPanel()
        {

        }

        void QuitGame()
        {
            Application.Quit();
        }

        void EnableInput(bool value)
        {
            buttonSingleplayerMatch.interactable = value;
            buttonMultiplayerMatch.interactable = value;
            buttonOptions.interactable = value;
            buttonExit.interactable = value;
            
        }


        /// <summary>
        /// Called when an offline match started ( solo )
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="player"></param>
        void HandleOnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            GetComponentInParent<MainMenu>().ActivateLobbyPanel();
        }

        void HandleOnStartSessionFailed(string errorMessage)
        {
            EnableInput(true);
        }


        void HandleOnLobbyJoint(SessionLobby sessionLobby)
        {
            GetComponentInParent<MainMenu>().ActivateMultiplayerPanel();
        }

        void HandleOnLobbyJoinFailed(SessionLobby sessionLobby, string errorMessage)
        {
            EnableInput(true);
        }

    }

}
