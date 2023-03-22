using Fusion;
using GOA.Assets;
using GOA.Interfaces;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOA
{
    public class Picker : NetworkBehaviour, IInteractable
    {
        public UnityAction OnItemPicked;

        [SerializeField]
        ItemAsset itemAsset;

        [SerializeField]
        Transform root;

        [SerializeField]
        GameObject vfx;

        [SerializeField]
        new GameObject light;

        [Networked (OnChanged = nameof(OnEmptyChanged))]
        public NetworkBool Empty { get; private set; } = false;

        [Networked]
        public NetworkString<_16> ItemAssetName { get; private set; }

        [Networked]
        public int CustomObjectId { get; private set; }

        bool interactionEnabled = false;
        GameObject sceneObject;

        List<Inventory> inventories = null;

        bool busy = false;

        public override void Spawned()
        {
            base.Spawned();

            SetInteractionEnabled(!Empty);

            // Load the item asset
            itemAsset = new List<ItemAsset>(Resources.LoadAll<ItemAsset>(ItemAsset.ResourceFolder)).Find(i => i.name.ToLower().Equals(ItemAssetName.ToString().ToLower()));

            if (!root)
                root = transform;

            if (!Empty)
            {
                LevelBuilder builder = FindObjectOfType<LevelBuilder>();
                sceneObject = builder.CustomObjects[CustomObjectId].SceneObject;

               
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);

        }

        #region interface implementation

        public void StartInteraction(PlayerController playerController)
        {
            Debug.Log("Interact " + playerController.PlayerId);
            if (interactionEnabled && !busy)
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

        public bool IsBusy()
        {
            return busy;
        }
 
        public void StopInteraction(PlayerController playerController)
        {
            SetInteractionEnabled(false);
            busy = false;
        }

        public bool TryUseItem(string itemName)
        {
            throw new System.NotImplementedException();
        }
        #endregion

        public void Init(int customObjectId, string itemAssetName, bool empty)
        {
            Empty = empty;
            CustomObjectId = customObjectId;
            ItemAssetName = itemAssetName;
        }

        IEnumerator PickUp(PlayerController playerController)
        {
            busy = true;
           
            // Add to the inventory
            if (inventories == null)
            {
                inventories = new List<Inventory>(GameObject.FindObjectsOfType<Inventory>());
            }

            Inventory inventory = inventories.Find(i => i.PlayerId == playerController.PlayerId);
            inventory.AddItem(itemAsset.name);
     
            yield return new WaitForSeconds(.5f);

            Empty = true;

            if (Runner.IsServer)
            {
                yield return new WaitForSeconds(1.0f);
                Runner.Despawn(GetComponent<NetworkObject>());
            }

            StopInteraction(playerController);
        }

        public static void OnEmptyChanged(Changed<Picker> changed)
        {
            if (changed.Behaviour.Empty)
            {
                DestroyImmediate(changed.Behaviour.sceneObject);
                DestroyImmediate(changed.Behaviour.vfx);
                DestroyImmediate(changed.Behaviour.light);

                changed.Behaviour.OnItemPicked?.Invoke();
            }

        }

  
    }

}
