using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GOA.UI
{
    public class MultiplayerPanel : MonoBehaviour
    {
        [SerializeField]
        Button buttonHostMatch;

        [SerializeField]
        Toggle togglePrivate;

        [SerializeField]
        Button buttonBack;

        

        private void Awake()
        {
            buttonBack.onClick.AddListener(()=> { if (SessionManager.Instance) SessionManager.Instance.LeaveLobby(); });
            buttonHostMatch.onClick.AddListener(HostMatch);
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
                SessionManager.Instance.OnSessionListUpdatedCallback += HandleOnSessionListUpdated;
                //SessionManager.Instance.OnLobbyJoint += HandleOnLobbyJoined;
                //SessionManager.Instance.OnLobbyJoinFailed += HandleOnLobbyJoinFailed;
                SessionManager.Instance.OnShutdownCallback += HandleOnShutdown;
                //SessionManager.Instance.JoinDefaultLobby();
            }

            EnableInput(true);
        }

        private void OnDisable()
        {
            SessionManager.Instance.OnPlayerJoinedCallback -= HandleOnPlayerJoined;
            SessionManager.Instance.OnStartSessionFailed -= HandleOnStartSessionFailed;
            SessionManager.Instance.OnSessionListUpdatedCallback -= HandleOnSessionListUpdated;
            //SessionManager.Instance.OnLobbyJoint -= HandleOnLobbyJoined;
            //SessionManager.Instance.OnLobbyJoinFailed -= HandleOnLobbyJoinFailed;
            SessionManager.Instance.OnShutdownCallback -= HandleOnShutdown;
        }

        void HostMatch()
        {
            EnableInput(false);
            SessionManager.Instance.HostSession(togglePrivate.isOn);
        }


        void EnableInput(bool value)
        {
            buttonBack.interactable = value;
            buttonHostMatch.interactable = value;
            togglePrivate.interactable = value;
        }

        #region SessionManager callbacks        
        void HandleOnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            GetComponentInParent<MainMenu>().ActivateLobbyPanel();

        }

        void HandleOnStartSessionFailed(string errorMessage)
        {
            EnableInput(true);
        }

        void HandleOnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessions)
        {

        }

        
        void HandleOnShutdown(NetworkRunner runner, ShutdownReason reason)
        {
            GetComponentInParent<MainMenu>().ActivateMainPanel();
        }
        #endregion

    }

}
