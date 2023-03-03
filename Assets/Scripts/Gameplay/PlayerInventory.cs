using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : NetworkBehaviour
{
    [Networked]
    [UnitySerializeField]
    public int PlayerId { get; private set; }

    [Networked(OnChanged = nameof(OnItemsChanged))]
    [Capacity(10)] public NetworkArray<NetworkString<_16>> Items => default;


    public override void Spawned()
    {
        base.Spawned();
    }

    public void Init(int playerId)
    {
        PlayerId = playerId;
    }

    public static void OnItemsChanged(Changed<PlayerInventory> changed)
    {
        //Debug.LogFormat("OnSolvedChanged:{0}", changed.Behaviour.Solved);
        //OnItemsChangedCallback?.Invoke(changed.Behaviour);
    }

}
