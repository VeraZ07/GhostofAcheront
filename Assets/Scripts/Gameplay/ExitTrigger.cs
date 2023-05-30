using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOA
{
    public class ExitTrigger : MonoBehaviour
    {

        List<PlayerController> playerControllers = new List<PlayerController>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            // Server side only 
            if (SessionManager.Instance.Runner.IsClient)
                return;

            // Is it a player?
            if (!other.tag.Equals(Tags.Player))
                return;

            PlayerController playerController = other.GetComponent<PlayerController>();
            if (!playerControllers.Contains(playerController))
                playerControllers.Add(playerController);
        }

        private void OnTriggerStay(Collider other)
        {
            // Server side only 
            if (SessionManager.Instance.Runner.IsClient)
                return;

            // Is it a player?
            if (!other.tag.Equals(Tags.Player))
                return;

            // Check if there is at least a player alive outside this trigger
            PlayerController outside = new List<PlayerController>(FindObjectsOfType<PlayerController>()).Find(p => !playerControllers.Contains(p) && p.State == (int)PlayerState.Alive);
            if (outside)
                return;

            
            FindObjectOfType<GameManager>().RpcGameWin();
        }

        private void OnTriggerExit(Collider other)
        {
            // Server side only 
            if (SessionManager.Instance.Runner.IsClient)
                return;

            // Is it a player?
            if (!other.tag.Equals(Tags.Player))
                return;

            playerControllers.Remove(other.GetComponent<PlayerController>());
        }
    }

}

