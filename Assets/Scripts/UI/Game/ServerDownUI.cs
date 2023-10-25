using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class ServerDownUI : MonoBehaviour
    {
        [SerializeField]
        GameObject panel;

        // Start is called before the first frame update
        void Start()
        {
           
            panel.SetActive(false);
        }

        private void Update()
        {
            if (SessionManager.Instance.Runner == null)
                return;
                        

        }

        private void OnEnable()
        {
            SessionManager.OnShutdownCallback += HandleOnShutdownCallback;
        }

        private void OnDisable()
        {
            SessionManager.OnShutdownCallback -= HandleOnShutdownCallback;
        }

        void HandleOnShutdownCallback(ShutdownReason reason)
        {
            if(reason == ShutdownReason.GameClosed)
            {
                panel.SetActive(true);
            }
        }

        
    }

}
