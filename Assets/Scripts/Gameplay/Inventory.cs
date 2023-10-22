using Fusion;
using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOA
{
    public class Inventory : NetworkBehaviour
    {
        public UnityAction<string> OnItemAdded;
        public UnityAction<string> OnItemRemoved;
        public UnityAction OnEmptyInventory;

        //[Networked]
        //[UnitySerializeField]
        //public int PlayerId { get; private set; } // The player id this inventory belongs to

        [Networked(OnChanged = nameof(OnItemsChanged))]
        [Capacity(10)] public NetworkLinkedList<NetworkString<_16>> Items => default;

        

        public override void Spawned()
        {
            base.Spawned();
        }

        //public void Init(int playerId)
        //{
        //    PlayerId = playerId;
        //}

        public void AddItem(string itemName)
        {
            Items.Add(itemName);
            // This is only called on local inventory
            //OnItemAdded?.Invoke(itemName);
        }

        public void RemoveItem(string itemName)
        {
            Items.Remove(itemName);
            // This is only called on local inventory
            //OnItemRemoved?.Invoke(itemName);
        }

        public void RespawnAllItems()
        {
            if (Runner.IsClient)
                return;
            //Inventory inv = new List<Inventory>(FindObjectsOfType<Inventory>()).Find(i => i.Object.HasInputAuthority);
            //Debug.Log("Found local inventory:" + inv);
            while (Items.Count > 0)
            {
                Level.LevelBuilder builder = FindObjectOfType<Level.LevelBuilder>();

                Picker picker = new List<Picker>(FindObjectsOfType<Picker>()).Find(p => p.ItemAssetName == Items[0].ToString() && p.Empty && builder.GetCurrentPlayingSector() == builder.GetSector(builder.GetTile(builder.CustomObjects[p.CustomObjectId].TileId).sectorIndex));
                Debug.Log("Found empty picker:" + picker.name);
                RemoveItem(Items[0].ToString());
                picker.Reactivate();
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
        public void RpcReportEmpty()
        {
            OnEmptyInventory?.Invoke();
        }

        public static void OnItemsChanged(Changed<Inventory> changed)
        {
            //Debug.LogFormat("OnSolvedChanged:{0}", changed.Behaviour.Solved);
            //OnItemsChangedCallback?.Invoke(changed.Behaviour);
            changed.LoadOld();
            int oldCount = changed.Behaviour.Items.Count;
            changed.LoadNew();
            int newCount = changed.Behaviour.Items.Count;
            if (oldCount < newCount)
                changed.Behaviour.OnItemAdded?.Invoke("");
            else
                changed.Behaviour.OnItemRemoved?.Invoke("");
        }

    }

}
