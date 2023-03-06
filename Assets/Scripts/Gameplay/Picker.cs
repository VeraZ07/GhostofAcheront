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

        bool interactionEnabled = false;
        GameObject sceneObject;

        List<Inventory> inventories = null;

        private void Awake()
        {
            if(itemAsset != null)
            {
                sceneObject = Instantiate(itemAsset.SceneObjectPrefab, root);
                sceneObject.transform.localPosition = Vector3.zero;
                sceneObject.transform.localRotation = Quaternion.identity;
            }
        }

        void Start()
        {
            
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
