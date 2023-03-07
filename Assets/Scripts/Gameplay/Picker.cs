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
            itemAsset = new List<ItemAsset>(Resources.LoadAll<ItemAsset>(ItemAsset.ResourceFolder)).Find(i => i.name.ToLower().Equals(ItemAssetName.ToLower()));

            if (!Empty)
            {
                sceneObject = Instantiate(itemAsset.SceneObjectPrefab, root);
                sceneObject.transform.localPosition = Vector3.zero;
                sceneObject.transform.localRotation = Quaternion.identity;
            }
        }

        

        public void Interact(PlayerController playerController)
        {
            if (!Empty)
                return;
            PickUp(playerController);
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
            SetInteractionEnabled(false);

            // Add to the inventory
            if (inventories == null)
            {
                inventories = new List<Inventory>(GameObject.FindObjectsOfType<Inventory>());
            }

            Inventory inventory = inventories.Find(i => i.PlayerId == playerController.PlayerId);
            inventory.AddItem(itemAsset);

            Empty = true;

            yield return new WaitForSeconds(2);

            DestroyImmediate(sceneObject);
        }

        public static void OnEmptyChanged(Changed<Picker> changed)
        {
            //OnNameChangedCallback?.Invoke(changed.Behaviour);
        }
    }

}
