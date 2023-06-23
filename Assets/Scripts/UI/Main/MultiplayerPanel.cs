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
        Button buttonReloadSessions;

        [SerializeField]
        Button buttonBack;



        [SerializeField]
        GameObject sessionItemPrefab;

        [SerializeField]
        GameObject sessionItemContainer;

        List<SessionItem> sessionItemList = new List<SessionItem>();

        private void Awake()
        {
            buttonBack.onClick.AddListener(()=> { if (SessionManager.Instance) SessionManager.Instance.LeaveLobby(); });
            buttonHostMatch.onClick.AddListener(HostMatch);
            buttonReloadSessions.onClick.AddListener(LoadOnlineSessions);
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
            
            SessionManager.OnPlayerJoinedCallback += HandleOnPlayerJoined;
            SessionManager.OnStartSessionFailed += HandleOnStartSessionFailed;
            SessionManager.OnSessionListUpdatedCallback += HandleOnSessionListUpdated;
            SessionManager.OnShutdownCallback += HandleOnShutdown;

            EnableInput(true);
        }

        private void OnDisable()
        {
            SessionManager.OnPlayerJoinedCallback -= HandleOnPlayerJoined;
            SessionManager.OnStartSessionFailed -= HandleOnStartSessionFailed;
            SessionManager.OnSessionListUpdatedCallback -= HandleOnSessionListUpdated;
            SessionManager.OnShutdownCallback -= HandleOnShutdown;
            
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
            //togglePrivate.interactable = value;
        }

        void JoinSession(string sessionName)
        {

            SessionManager.Instance.JoinSession(sessionName);
        }

        void LoadOnlineSessions()
        {
            Debug.Log("LoadOnlineSessions");

            // Clear all session items
            int count = sessionItemContainer.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                DestroyImmediate(sessionItemContainer.transform.GetChild(0).gameObject);
            }

            // Get all the sessions            
            ICollection<SessionInfo> sessions = SessionManager.Instance.SessionList;

            if (sessions.Count == 0)
                return;

            // Fill the list
            foreach(SessionInfo session in sessions)
            {
                if (!session.IsValid || session.PlayerCount >= session.MaxPlayers || !session.IsVisible || !session.IsOpen)
                    continue;
                GameObject sessionItem = Instantiate(sessionItemPrefab, sessionItemContainer.transform);
                sessionItem.GetComponent<SessionItem>().Init(session, JoinSession);
            }
        }



        IEnumerator ActivateLobbyPanel()
        {
            yield return new WaitForSeconds(1f);
            GetComponentInParent<MainMenu>().ActivateLobbyPanel();
        }
      
        #region SessionManager callbacks        
        void HandleOnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            // We need to wait for lobby players to spawn
            StartCoroutine(ActivateLobbyPanel());
        }

        void HandleOnStartSessionFailed(string errorMessage)
        {
            EnableInput(true);
        }

        void HandleOnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessions)
        {
            // Get online sessions 
            LoadOnlineSessions();
        }

        
        void HandleOnShutdown(ShutdownReason reason)
        {
            
            GetComponentInParent<MainMenu>().ActivateMainPanel();
        }
        #endregion

    }

}
