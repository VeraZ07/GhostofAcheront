using Fusion;
using GOA.Assets;
using GOA.Interfaces;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static GOA.Level.LevelBuilder;

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

        [SerializeField]
        AudioSource audioSource;

        [Networked (OnChanged = nameof(OnEmptyChanged))]
        public NetworkBool Empty { get; private set; } = false;

        [Networked]
        public NetworkString<_16> ItemAssetName { get; private set; }

        [Networked]
        public int CustomObjectId { get; private set; }
        GameObject sceneObject;

        List<Inventory> inventories = null;

        float rotationSpeed = 20f;
        Transform objectToRotate;

        void Update()
        {
            if(objectToRotate)
                objectToRotate.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        }

        public override void Spawned()
        {
            base.Spawned();

            //SetInteractionEnabled(!Empty);

            // Load the item asset
            itemAsset = new List<ItemAsset>(Resources.LoadAll<ItemAsset>(ItemAsset.ResourceFolder)).Find(i => i.name.ToLower().Equals(ItemAssetName.ToString().ToLower()));

            if (!root)
                root = transform;

            Debug.Log("Spawning picker:" + name);
            //if (!Empty)
            {
                LevelBuilder builder = FindObjectOfType<LevelBuilder>();
                CustomObject co = builder.CustomObjects[CustomObjectId];
                sceneObject = co.SceneObject;
                Debug.Log("Spawning picker - found scene object:" + sceneObject.name);
                objectToRotate = sceneObject.transform.GetChild(0).GetChild(0);
               
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
            if (IsInteractionEnabled())
                StartCoroutine(PickUp(playerController));
                
        }

        public bool IsInteractionEnabled()
        {
            return !Empty;
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
           
            Empty = true;

            // Add to the inventory
            if (inventories == null)
            {
                inventories = new List<Inventory>(GameObject.FindObjectsOfType<Inventory>());
            }

            //Inventory inventory = inventories.Find(i => i.PlayerId == playerController.PlayerId);
            Inventory inventory = inventories.Find(i => i.Object.InputAuthority == playerController.Object.InputAuthority);
            inventory.AddItem(itemAsset.name);

            

            yield return new WaitForSeconds(.5f);

           

            if (Runner.IsServer)
            {
                SessionManager.Instance.PushSnapshot();
                yield return new WaitForSeconds(1.0f);
                //Runner.Despawn(GetComponent<NetworkObject>());
            }


        }

        

        public void Reactivate()
        {
            Empty = false;
        }



        public static void OnEmptyChanged(Changed<Picker> changed)
        {
            if (changed.Behaviour.Empty)
            {
                Debug.Log("BBBBBBBBBBBBBBBBBBBBBBBBB");
                //DestroyImmediate(changed.Behaviour.sceneObject);
                //DestroyImmediate(changed.Behaviour.vfx);
                //DestroyImmediate(changed.Behaviour.light);

                changed.Behaviour.sceneObject.SetActive(false);
                
                changed.Behaviour.audioSource.Play();

                changed.Behaviour.OnItemPicked?.Invoke();
            }
            else
            {
                Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                changed.Behaviour.sceneObject.SetActive(true);
            }
        }

  
    }

}
