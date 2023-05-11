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

        [SerializeField]
        Sprite spriteReady;

        [SerializeField]
        Sprite spriteNotReady;

        [SerializeField]
        Sprite spriteLocked;

        Color unlockedColor = Color.white;
        Color lockedColor = Color.gray;

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
           
            SetReady(false);
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
            textName.color = unlockedColor;
            SetReady(player.Ready ? true : false);
         
        }

        public void Reset()
        {
            player = null;
            textName.text = "Waiting...";
            textName.color = unlockedColor;
            SetReady(false);
        }

        public void SetLocked()
        {
            Reset();
            textName.text = "Not available";
            textName.color = lockedColor;
            imageReady.sprite = spriteLocked;
        }

        public void SetReady(bool value)
        {
            imageReady.sprite = value ? spriteReady : spriteNotReady;
        }
    }

}
