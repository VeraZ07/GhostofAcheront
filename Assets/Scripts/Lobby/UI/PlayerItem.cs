using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GOA.UI
{
    public class PlayerItem : MonoBehaviour
    {
        [SerializeField]
        TMP_Text textName;

        [SerializeField]
        Image imageReady;

        Player player;
        public Player Player
        {
            get { return player; }
        }
        
        public bool IsEmpty
        {
            get { return player == null; }
        }

        private void Awake()
        {
            imageReady.color = Color.red;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetPlayer(Player player)
        {
            this.player = player;
            textName.text = player.Name;
            imageReady.color = player.Ready ? Color.green : Color.red;
        }

        public void Reset()
        {
            player = null;
            textName.text = "Waiting...";
            imageReady.color = Color.red;
        }

        public void SetReady(bool value)
        {
            imageReady.color = value ? Color.green : Color.red;
        }
    }

}
