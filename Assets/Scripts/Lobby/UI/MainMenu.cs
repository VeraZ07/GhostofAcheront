using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        GameObject mainPanel;

        [SerializeField]
        GameObject multiplayerPanel;

        [SerializeField]
        GameObject lobbyPanel;

        [SerializeField]
        GameObject optionsPanel;

        GameObject currentActivePanel;

        // Start is called before the first frame update
        void Start()
        {
            ActivateMainPanel();
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region private methods
        void ActivatePanel(GameObject panel)
        {
            DeactivateAllPanels();
            panel.SetActive(true);
        }

        void DeactivateAllPanels()
        {
            mainPanel.SetActive(false);
            lobbyPanel.SetActive(false);
            optionsPanel.SetActive(false);
            multiplayerPanel.SetActive(false);
        }
        #endregion

        #region public methods
        public void ActivateMainPanel()
        {
            ActivatePanel(mainPanel);
        }

        public void ActivateOptionsPanel()
        {
            ActivatePanel(optionsPanel);
        }

        public void ActivateLobbyPanel()
        {
            ActivatePanel(lobbyPanel);
        }

        public void ActivateMultiplayerPanel()
        {
            ActivatePanel(multiplayerPanel);
        }
        #endregion

    }
}

