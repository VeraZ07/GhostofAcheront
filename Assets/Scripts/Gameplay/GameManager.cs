using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    
    public class GameManager : NetworkBehaviour
    {
        
        public static GameManager Instance { get; private set; }

        [Networked] public int GameSeed { get; private set; } = 0;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
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

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            Instance = null;
        }



        public void CreateNewSeed()
        {

            GameSeed = (int)System.DateTime.UtcNow.Ticks;
        }
    }

}
