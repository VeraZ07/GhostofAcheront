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
        Button buttonQuitMatch;

        

        private void Awake()
        {
            buttonQuitMatch.onClick.AddListener(()=>SessionManager.Instance.QuitSession());
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
            if(SessionManager.Instance)
            {
                SessionManager.Instance.OnShutdownCallback += HandleOnShutdown;
            }
            
        }

        private void OnDisable()
        {
            SessionManager.Instance.OnShutdownCallback -= HandleOnShutdown;
        }

       
        void HandleOnShutdown(NetworkRunner runner, ShutdownReason reason)
        {
            GetComponentInParent<MainMenu>().ActivateMainPanel();
        }
    }

}
