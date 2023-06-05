using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GOA.UI
{
    public class MapName : MonoBehaviour
    {
        [SerializeField]
        TMP_Text textField;

        
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
            if (!SessionManager.Instance || !SessionManager.Instance.Runner)
                return;

            if (SessionManager.Instance.Runner.IsSinglePlayer)
            {
                textField.text = "SinglePlayer";
            }
            else
            {
                textField.text = SessionManager.Instance.Runner.SessionInfo.Name;
            }
        }

        private void OnDisable()
        {
            textField.text = "";
        }
    }

}
