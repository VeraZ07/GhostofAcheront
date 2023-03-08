using DG.Tweening;
using Fusion;
using GOA.Assets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GOA.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField]
        GameObject mainPanel;

        [SerializeField]
        List<GameObject> elements;
        

        float height;

        bool open = false;

        private void Awake()
        {
            height = 740f;//(mainPanel.transform as RectTransform).anchoredPosition.magnitude.;
        }

        // Start is called before the first frame update
        void Start()
        {
            
            open = false;
            (mainPanel.transform as RectTransform).anchoredPosition = new Vector2(0f, height);
            mainPanel.SetActive(open);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!open)
                    Open();
                else
                    Close();
            }
                
        }

        void Open()
        {
            if (open)
                return;

            open = true;
            mainPanel.SetActive(open);

            Player player = GetLocalPlayer();

            CharacterAsset character = new List<CharacterAsset>(Resources.LoadAll<CharacterAsset>(CharacterAsset.ResourceFolder)).Find(c => c.CharacterId == player.CharacterId);

            SetAvatar(elements[0], character.Avatar);

            Inventory inv = GetPlayerInventory(player);
            for(int i=0; i<inv.Items.Count; i++)
                SetItem(elements[0], i, inv.Items[i].ToString());

            
            (mainPanel.transform as RectTransform).DOAnchorPos(Vector3.zero, .5f, true);

        }

        void Close()
        {
            if (!open)
                return;

            (mainPanel.transform as RectTransform).DOAnchorPos(new Vector2(0f, height), .5f, true).OnComplete(()=> { open = false; mainPanel.SetActive(open); });

        }

        Player GetLocalPlayer()
        {
            PlayerRef pRef = SessionManager.Instance.Runner.LocalPlayer;
            Player p = new List<Player>(FindObjectsOfType<Player>()).Find(p => p.PlayerRef == pRef);
            return p;
        }

        void SetAvatar(GameObject element, Sprite avatar)
        {
            // Get the avatar image
            Image avatarImg = new List<Image>(element.GetComponentsInChildren<Image>()).Find(i => i.gameObject.name.ToLower().Equals("avatar"));
            avatarImg.sprite = avatar;
        }

        void SetItem(GameObject element, int index, string itemName)
        {
            ItemAsset itemAsset = new List<ItemAsset>(Resources.LoadAll<ItemAsset>(ItemAsset.ResourceFolder)).Find(c => c.name.ToLower().Equals(itemName.ToLower()));
            Image itemImg = new List<Image>(element.GetComponentsInChildren<Image>()).Find(i => i.gameObject.name.ToLower().Equals(string.Format("item_{0}", index)));

            itemImg.sprite = itemAsset.Icon;
        }

        Inventory GetPlayerInventory(Player player)
        {
            return new List<Inventory>(FindObjectsOfType<Inventory>()).Find(i => i.PlayerId == player.PlayerRef.PlayerId);
        }
    }

}
