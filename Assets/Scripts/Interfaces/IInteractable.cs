using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Interfaces
{
    public interface IInteractable
    {
        void StartInteraction(PlayerController playerController);

        void StopInteraction(PlayerController playerController);

        bool IsInteractionEnabled();

        //bool IsBusy();

        //void SetInteractionEnabled(bool value);

        //bool TryUseItem(string itemName);
    }

}
