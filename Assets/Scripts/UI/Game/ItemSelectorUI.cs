using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GOA.UI
{
    public class ItemSelectorUI : MonoBehaviour
    {
        [SerializeField]
        GameObject panel;

        [SerializeField]
        Button buttonUse;

        [SerializeField]
        Button buttonView;

        [SerializeField]
        Button buttonQuit;

        [SerializeField]
        ToggleGroup toggleGroup;

        [SerializeField]
        List<Toggle> toggles;
        
        List<ItemAsset> items = new List<ItemAsset>();

        bool open = false;

        private Inventory inventory;

       

        // Start is called before the first frame update
        void Start()
        {
            toggles = new List<Toggle>(toggleGroup.GetComponentsInChildren<Toggle>());
            Close();
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (open)
                    Close();
                else
                    Open();
            }
#endif
        }


        void Open()
        {
       
            open = true;

            panel.SetActive(true);

            // Get the local player inventory
            if (!inventory)
            {
                inventory = new List<Inventory>(GameObject.FindObjectsOfType<Inventory>()).Find(i => i.PlayerId == SessionManager.Instance.Runner.LocalPlayer.PlayerId);
            }

            // Cast all the names to string
            List<string> names = new List<string>();
            //foreach(var itemName in inventory.Items)
            //{
            //    names.Add(itemName.ToString().ToLower());
            //}
            names.Add("pic1_0");
            names.Add("pic1_1");
            names.Add("pic1_3");

            // Load the items
            List<ItemAsset> assets = new List<ItemAsset>(Resources.LoadAll<ItemAsset>(ItemAsset.ResourceFolder));
            foreach(ItemAsset asset in assets)
            {
                if (names.Contains(asset.name.ToLower()))
                    items.Add(asset);
            }

            // Reset all toggles
            foreach(Toggle toggle in toggles)
            {
                toggle.targetGraphic.GetComponent<Image>().sprite = null;
                toggle.isOn = false;
                toggle.interactable = false;
            }

            // Show the icons
            for(int i=0; i<items.Count; i++)
            {
                toggles[i].targetGraphic.GetComponent<Image>().sprite = items[i].Icon;
                if (i == 0)
                    toggles[i].isOn = true;
                toggles[i].group.RegisterToggle(toggles[i]);
                toggles[i].interactable = true;
            }

            
        }

        void Close()
        {
            
            open = false;

            // Unregister toggles
            foreach (Toggle toggle in toggles)
            {
                toggle.group.UnregisterToggle(toggle);
            }

            items.Clear();

            panel.SetActive(false);
        }

    }

}
