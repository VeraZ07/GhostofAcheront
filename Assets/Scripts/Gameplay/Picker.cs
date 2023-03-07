using Fusion;
using GOA.Assets;
using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class Picker : NetworkBehaviour, IInteractable
    {
        [SerializeField]
        ItemAsset itemAsset;

        [SerializeField]
        Transform root;

        [Networked (OnChanged = nameof(OnEmptyChanged))]
        public NetworkBool Empty { get; private set; } = false;

        [Networked]
        public NetworkString<_16> ItemAssetName { get; private set; }

        bool interactionEnabled = false;
        GameObject sceneObject;

        List<Inventory> inventories = null;



        private void Awake()
        {
            
        }

        void Start()
        {
            
        }

        public override void Spawned()
        {
            base.Spawned();

            Debug.Log("Empty:" + Empty);

            // Load item asset resource
            Debug.Log("ItemAssetName:" + ItemAssetName);
            itemAsset = new List<ItemAsset>(Resources.LoadAll<ItemAsset>(ItemAsset.ResourceFolder)).Find(i => i.name.ToLower().Equals(ItemAssetName.ToString().ToLower())); 
            Debug.Log("Found:" + Resources.LoadAll<ItemAsset>(ItemAsset.ResourceFolder)[0].name);
            Debug.Log("ItemAsset:" + itemAsset);

            SetInteractionEnabled(!Empty);

            if (!Empty)
            {
                sceneObject = Instantiate(itemAsset.SceneObjectPrefab, root);
                sceneObject.transform.localPosition = Vector3.zero;
                sceneObject.transform.localRotation = Quaternion.identity;
            }
        }

        

        public void Interact(PlayerController playerController)
        {
            Debug.Log("Interact " + playerController.PlayerId);
            if (interactionEnabled)
                StartCoroutine(PickUp(playerController));
        }

        public bool IsInteractionEnabled()
        {
            return interactionEnabled;
        }

        public void SetInteractionEnabled(bool value)
        {
            interactionEnabled = value;
        }

        public void Init(string itemAssetName, bool empty)
        {
            Empty = empty;
            ItemAssetName = itemAssetName;
        }

        IEnumerator PickUp(PlayerController playerController)
        {
            Debug.Log("PickUp:" + playerController.PlayerId);
            SetInteractionEnabled(false);
            

            // Add to the inventory
            if (inventories == null)
            {
                inventories = new List<Inventory>(GameObject.FindObjectsOfType<Inventory>());
            }

            Inventory inventory = inventories.Find(i => i.PlayerId == playerController.PlayerId);
            inventory.AddItem(itemAsset);

            yield return new WaitForSeconds(.5f);

            Empty = true;

            

            
        }

        public static void OnEmptyChanged(Changed<Picker> changed)
        {
            if(changed.Behaviour.Empty)
                DestroyImmediate(changed.Behaviour.sceneObject);
        }
    }

}
