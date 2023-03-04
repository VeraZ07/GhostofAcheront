using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GOA.UI
{
    public class SessionItem : MonoBehaviour
    {
        [SerializeField]
        Button buttonJoin;

        [SerializeField]
        TMP_Text textSessionName;

        [SerializeField]
        TMP_Text textPlayerCount;

        SessionInfo info;
        

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init(SessionInfo info, UnityAction<string> JoinSession)
        {
            this.info = info;
            
            textSessionName.text = info.Name.Substring(0, 10);
            textPlayerCount.text = string.Format("{0}/{1}", info.PlayerCount, info.MaxPlayers);
                        

            buttonJoin.onClick.AddListener(() => { JoinSession(info.Name); });
        }

        
    }

}
