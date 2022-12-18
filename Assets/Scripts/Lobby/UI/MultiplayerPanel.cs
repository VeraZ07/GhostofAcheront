using Fusion;
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
            buttonBack.onClick.AddListener(GetComponentInParent<MainMenu>().ActivateMainPanel);
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
                SessionManager.Instance.OnSessionCreated += HandleOnSessionCreated;
                SessionManager.Instance.OnSessionCreationFailed += HandleOnSessionCreationFailed;
            }
            
        }

        private void OnDisable()
        {
            SessionManager.Instance.OnSessionCreated -= HandleOnSessionCreated;
            SessionManager.Instance.OnSessionCreationFailed -= HandleOnSessionCreationFailed;
        }

        void HostMatch()
        {
            DisableInput(true);
            SessionManager.Instance.HostSession(togglePrivate.isOn);
        }

        void HandleOnSessionCreated(SessionInfo sessionInfo)
        {
            GetComponentInParent<MainMenu>().ActivateLobbyPanel();
        }

        void HandleOnSessionCreationFailed(string errorMessage)
        {
            
        }

        void DisableInput(bool value)
        {
            buttonBack.interactable = value;
            buttonHostMatch.interactable = value;
            togglePrivate.interactable = value;
        }
    }

}
