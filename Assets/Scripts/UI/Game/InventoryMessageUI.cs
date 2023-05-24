using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class InventoryMessageUI : MonoBehaviour
    {
        [SerializeField]
        GameObject panel;

        [SerializeField]
        GameObject inMessage;

        [SerializeField]
        GameObject outMessage;

        [SerializeField]
        GameObject emptyMessage;

        Inventory inventory;

        float hideDelay = 1f;

        private void Awake()
        {
            panel.SetActive(false);
        }

        // Start is called before the first frame update
        

        // Update is called once per frame
        void Update()
        {
            if (!inventory)
            {
                //inventory = new List<Inventory>(FindObjectsOfType<Inventory>()).Find(i => i.PlayerId == Player.Local.PlayerRef.PlayerId);
                inventory = new List<Inventory>(FindObjectsOfType<Inventory>()).Find(i => i.Object.InputAuthority == Player.Local.PlayerRef);
                if (inventory)
                {
                    inventory.OnItemAdded += HandleOnItemAdded;
                    inventory.OnItemRemoved += HandleOnItemRemoved;
                    inventory.OnEmptyInventory += HandleOnEmptyInventory;
                }
            }

            if(hideDelay > 0)
            {
                hideDelay -= Time.deltaTime;

                if(hideDelay <= 0)
                {
                    panel.SetActive(false);
                }
            }
        }

        void HandleOnItemAdded(string itemName)
        {
            inMessage.SetActive(true);
            outMessage.SetActive(false);
            emptyMessage.SetActive(false);
            panel.SetActive(true);
            ResetHideDelay();
        }

        void HandleOnItemRemoved(string itemName)
        {
            inMessage.SetActive(false);
            outMessage.SetActive(true);
            emptyMessage.SetActive(false);
            panel.SetActive(true);
            ResetHideDelay();
        }
              
        void HandleOnEmptyInventory()
        {
            inMessage.SetActive(false);
            outMessage.SetActive(false);
            emptyMessage.SetActive(true);
            panel.SetActive(true);
            ResetHideDelay();
        }

        void ResetHideDelay()
        {
            hideDelay = 1f;
        }
    }

}
