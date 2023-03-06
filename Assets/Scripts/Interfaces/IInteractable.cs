using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Interfaces
{
    public interface IInteractable
    {
        void Interact(PlayerController playerController);

        bool IsInteractionEnabled();

        void SetInteractionEnabled(bool value); 
    }

}
