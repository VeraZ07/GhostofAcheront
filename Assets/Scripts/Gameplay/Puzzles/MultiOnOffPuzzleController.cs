using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    /// <summary>
    /// Manage a combination of objects ( for example levers ).
    /// </summary>
    public class MultiOnOffPuzzleController : PuzzleController
    {
        [SerializeField]
        GameObject onOffControllerPrefab; // Create OnOffController script

        [SerializeField]
        int onOffControllerCount = 1;

        //[Networked]
        //[Capacity(10)]
        //NetworkArray<NetworkBool> OnOffArray { get; } = MakeInitializer(new NetworkBool[10]);

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Spawned()
        {
            base.Spawned();

            if (Runner.IsServer)
            {
                // Spawn OnOffController objects
                for(int i=0; i<onOffControllerCount; i++)
                {
                    //Runner.Spawn(onOffControllerPrefab, )
                }
            }
        }
    }

}
