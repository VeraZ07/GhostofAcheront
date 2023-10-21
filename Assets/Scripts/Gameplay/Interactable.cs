using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public abstract class Interactable : MonoBehaviour, IInteractable
    {
       
        protected virtual void Awake(){}

        public virtual void StopInteraction(PlayerController playerController) { }

        public virtual bool IsInteractionEnabled() { return false; }

        public virtual void StartInteraction(PlayerController playerController) { }

        public virtual bool KeepPressed() { return false; }
    }

}
