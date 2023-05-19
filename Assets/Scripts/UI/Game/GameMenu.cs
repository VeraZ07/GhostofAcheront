using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class GameMenu : MonoBehaviour
    {
        [SerializeField]
        GameObject mainPanel;

        [SerializeField]
        GameObject optionsPanel;

        bool visible = false;

        // Start is called before the first frame update
        void Start()
        {
            Hide();
        }


        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Show();
            }
        }

        #region private methods
        void ActivatePanel(GameObject panel)
        {
            if (!visible)
            {
                Debug.LogWarning("Set the game menu visible first");
                return;
            }
            DeactivateAllPanels();
            panel.SetActive(true);
           
        }

        void DeactivateAllPanels()
        {
            mainPanel.SetActive(false);
            optionsPanel.SetActive(false);
          
        }
        #endregion

        #region public methods
        public void ActivateGamePanel()
        {
            ActivatePanel(mainPanel);
        }

        public void ActivateOptionsPanel()
        {
            ActivatePanel(optionsPanel);
        }

        public void Show()
        {
            if (visible)
                return;
            visible = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ActivateGamePanel();
        }

        public void Hide()
        {
            DeactivateAllPanels();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            visible = false;
        }

        #endregion
    }

}
