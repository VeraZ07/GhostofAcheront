using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
        public abstract bool IsInteractionEnabled();

        public abstract void StartInteraction(PlayerController playerController);

        protected virtual void Awake()
        {
            //InteractableManager.Instac.Register(this);
        }
    }

}
