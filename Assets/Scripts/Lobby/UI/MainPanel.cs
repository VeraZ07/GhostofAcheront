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

        void CreateSingleplayerMatch()
        {
            // Create singleplayer match
            SessionManager.Instance.PlaySolo();
        }

        void CreateMultiplayerMatch()
        {
            //SessionManager.Instance.HostMatch(false);
            GetComponentInParent<MainMenu>().ActivateMultiplayerPanel();
        }

        void OpenOptionsPanel()
        {

        }

        void QuitGame()
        {
            Application.Quit();
        }
    }

}
