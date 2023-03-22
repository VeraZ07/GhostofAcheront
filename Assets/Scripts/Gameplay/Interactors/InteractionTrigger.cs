using Fusion;
using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class InteractionTrigger : MonoBehaviour
    {
        
        [SerializeField]
        GameObject interactableObject;

        IInteractable interactable;

        PlayerController playerController;

        NetworkRunner runner;

        private void Awake()
        {
            if (interactableObject)
                interactable = interactableObject.GetComponent<IInteractable>();
            else
                interactable = GetComponent<IInteractable>();

        }

        // Start is called before the first frame update
        void Start()
        {
            interactable = GetComponentInChildren<IInteractable>();        
                
        }

        // Update is called once per frame
        void Update()
        {
            if (!playerController)
                return;

            if (playerController.HasInputAuthority)
            {
                // Manage the UI ( you may want to show some icon to let the player know about the interaction )
                //if (interactable.IsInteractionEnabled())
                //    ShowInteraction(true);
                //else
                //    ShowInteraction(false);
            }


            if (SessionManager.Instance.Runner.IsServer)
            {
                
                // Do some logic here
                if (interactable.IsInteractionEnabled() && !interactable.IsBusy())
                    interactable.StartInteraction(playerController);
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (playerController)
                return;

            if(other.tag == "Player")
            {
                playerController = other.GetComponent<PlayerController>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!playerController)
                return;

            if(other.tag == "Player")
            {
                if (other.GetComponent<PlayerController>() == playerController)
                    playerController = null;
            }
        }


    }

}
