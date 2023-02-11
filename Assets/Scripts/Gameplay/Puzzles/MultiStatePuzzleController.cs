using Fusion;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    /// <summary>
    /// Manage a combination of objects ( for example levers ).
    /// </summary>
    public class MultiStatePuzzleController : PuzzleController
    {

        [SerializeField]
        GameObject stateControllerPrefab; // Create OnOffController script

        /// <summary>
        /// The key is the index of the corresponding object in the builder list; the value is the state of that object.
        /// </summary>
        [Networked]
        //[Capacity(6)]
        [UnitySerializeField]
        NetworkDictionary<int, int> StateControllerDictionary => default;

        //[Networked]
        [UnitySerializeField]
        int StateControllerCount { get; set; } = 0;



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

            if (!Runner.IsHostMigrationEnabled)
            {
                // Create state controllers from scratch
                int count = Random.Range(1, 5);
                LevelBuilder builder = FindObjectOfType<LevelBuilder>();


            }
            else
            {
                // Resume state controllers from the builder
            }
        }
    }

}
