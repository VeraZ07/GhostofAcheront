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
            if (SessionManager.Instance.Runner.IsClient)
                return;

            // Is it a player?
            if (!other.tag.Equals(Tags.Player))
                return;

            PlayerController playerController = other.GetComponent<PlayerController>();
            if (!playerControllers.Contains(playerController))
                playerControllers.Add(playerController);

            FindObjectOfType<GameManager>().PlayerEnteredTheEscapeTrigger(playerController);
        }

        //private void OnTriggerStay(Collider other)
        //{
        //    return;
        //    // Server side only 
        //    if (SessionManager.Instance.Runner.IsClient)
        //        return;

        //    // Is it a player?
        //    if (!other.tag.Equals(Tags.Player))
        //        return;

            

        //    // Check if there is at least a player alive outside this trigger
        //    PlayerController outside = new List<PlayerController>(FindObjectsOfType<PlayerController>()).Find(p => !playerControllers.Contains(p) && p.State == (int)PlayerState.Alive);
        //    if (outside)
        //        return;

        //    // Report the monster
        //    FindObjectOfType<MonsterController>()?.SetPlayerEscapedState();
        //    // All the alive players escaped 
        //    List<PlayerController> aliveAll = new List<PlayerController>(FindObjectsOfType<PlayerController>()).FindAll(p => p.State == (int)PlayerState.Alive);
        //    foreach (PlayerController player in aliveAll)
        //        player.SetEscapedState();

        //    // Sacrifice all the players already in the dead state: dying players will be sacrificed when moving to the dead state too. 
        //    List<PlayerController> deadAll = new List<PlayerController>(FindObjectsOfType<PlayerController>()).FindAll(p => p.State == (int)PlayerState.Dead);
        //    foreach (PlayerController player in deadAll)
        //        player.SetSacrificedState();

        //}

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

        public bool IsPlayerInside(PlayerController playerController)
        {
            return playerControllers.Contains(playerController);
        }
    }

}

