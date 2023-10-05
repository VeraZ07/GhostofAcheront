using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class InteractableManager: MonoBehaviour
    {
        public static InteractableManager Instance { get; private set; }

        List<IInteractable> interactables;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public int GetInteractableId(IInteractable interactable)
        {
            return interactables.IndexOf(interactable);
        }

        public IInteractable GetInteractable(int interactableId)
        {
            return interactables[interactableId];
        }

        public void Init()
        {
            interactables = new List<IInteractable>(FindObjectsOfType<Interactable>());
            Debug.Log("Interactables:" + interactables.Count);
        }
    }

}
