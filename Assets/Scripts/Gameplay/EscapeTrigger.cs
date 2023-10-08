using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOA
{
    public class EscapeTrigger : MonoBehaviour
    {
        public static EscapeTrigger Instance { get; private set; }


        List<PlayerController> playerControllers = new List<PlayerController>();

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
            if (SessionManager.Instance.Runner.IsClient && !SessionManager.Instance.Runner.IsSharedModeMasterClient)
                return;

            // Is it a player?
            if (!other.tag.Equals(Tags.Player))
                return;

            PlayerController playerController = other.GetComponent<PlayerController>();
            if (!playerControllers.Contains(playerController))
                playerControllers.Add(playerController);

            FindObjectOfType<GameManager>().PlayerEnteredTheEscapeTrigger(playerController);
        }

    

        private void OnTriggerExit(Collider other)
        {
            // Server side only 
            if (SessionManager.Instance.Runner.IsClient && !SessionManager.Instance.Runner.IsSharedModeMasterClient)
                return;

            // Is it a player?
            if (!other.tag.Equals(Tags.Player))
                return;

            playerControllers.Remove(other.GetComponent<PlayerController>());
        }

        public bool IsPlayerInside(PlayerController playerController)
        {
            return playerControllers.Contains(playerController);
        }
    }

}

