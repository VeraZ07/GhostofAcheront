using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOA
{
    public class ExitTrigger : MonoBehaviour
    {

        

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
            if (!other.tag.Equals(Tags.Player))
                return;

            
            GameManager gameManager = FindObjectOfType<GameManager>();
            gameManager.PlayerExit(other.GetComponent<PlayerController>());
            
        }
    }

}

