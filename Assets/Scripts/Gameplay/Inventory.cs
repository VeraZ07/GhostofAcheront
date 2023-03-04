using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : NetworkBehaviour
{
    [Networked]
    [UnitySerializeField]
    public int PlayerId { get; private set; } // The player id this inventory belongs to

    [Networked(OnChanged = nameof(OnItemsChanged))]
    [Capacity(10)] public NetworkLinkedList<NetworkString<_16>> Items => default;

    public void Update()
    {
        if (Runner.IsServer)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                string[] itemNames = new string[] { "pippo", "pluto", "paperino", "minnie", "topolino" };
                if (PlayerId == 3 || PlayerId == 9)
                {
                    Items.Add(itemNames[Items.Count]);
                }
                
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                string[] itemNames = new string[] { "pippo", "pluto", "paperino", "minnie", "topolino" };
                if (PlayerId == 0)
                {
                    Items.Add(itemNames[Items.Count]);
                }

            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                string[] itemNames = new string[] { "pippo", "pluto", "paperino", "minnie", "topolino" };
                if (PlayerId == 1)
                {
                    Items.Add(itemNames[Items.Count]);
                }

            }
        }
        
        

    }

    public override void Spawned()
    {
        base.Spawned();
    }

    public void Init(int playerId)
    {
        PlayerId = playerId;
    }

    public static void OnItemsChanged(Changed<Inventory> changed)
    {
        //Debug.LogFormat("OnSolvedChanged:{0}", changed.Behaviour.Solved);
        //OnItemsChangedCallback?.Invoke(changed.Behaviour);
    }

}
