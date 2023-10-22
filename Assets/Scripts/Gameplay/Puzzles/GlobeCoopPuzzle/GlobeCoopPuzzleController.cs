using Fusion;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class GlobeCoopPuzzleController : PuzzleController
    {

        [UnitySerializeField]
        [Networked(OnChanged = nameof(OnActiveGlobesChanged))] [Capacity(SessionManager.MaxPlayers)] public NetworkLinkedList<NetworkBool> ActiveGlobes { get; }

        List<GlobeInteractor> globes = new List<GlobeInteractor>();


        public override void Spawned()
        {
            base.Spawned();

            // Get the builder
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            // Get the puzzle
            LevelBuilder.GlobeCoopPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as LevelBuilder.GlobeCoopPuzzle;
            // Loop through all the globes and create the corresponding interactor
            for(int i=0; i<puzzle.Globes.Count; i++)
            {
                // Get the globe custom object id
                int coId = puzzle.Globes[i].CustomObjectId;
                // Get the corresponding scene object
                GameObject co = builder.CustomObjects[coId].SceneObject;
                // Get the interactor component
                GlobeInteractor comp = co.GetComponentInChildren<GlobeInteractor>();
                // Init the interactor
                comp.Init(this);
                // Add the component to the globe list
                globes.Add(comp);

                if(Runner.IsServer || Runner.IsSharedModeMasterClient)
                {
                    // Add a new entry in the synch list
                    ActiveGlobes.Add(false);
                }
                
            }
        }

        public bool GlobeIsInteractable(GlobeInteractor globe)
        {
            if (Solved || ActiveGlobes[GetGlobeIndex(globe)])
                return false;

            return true;
        }

        public void ActivateGlobe(GlobeInteractor globe)
        {
            if (!GlobeIsInteractable(globe))
                return;

            // Set the corresponding item in the synch list
            ActiveGlobes.Set(GetGlobeIndex(globe), true);
        }

        public void DeactivateGlobe(GlobeInteractor globe)
        {
            if (GlobeIsInteractable(globe) || Solved)
                return;
            // Deactivate the globe
            ActiveGlobes.Set(GetGlobeIndex(globe), false);
        }

        public int GetGlobeIndex(GlobeInteractor globe)
        {
            return globes.IndexOf(globe);
        }

        public int GetActiveGlobeCount()
        {
            int count = 0;
            foreach(NetworkBool globe in ActiveGlobes)
            {
                if (globe)
                    count++;
            }
            return count;
        }

        void CheckSolution()
        {
            for(int i=0; i<ActiveGlobes.Count; i++)
            {
                if (!ActiveGlobes[i])
                    return;
            }

            Solved = true;
        }

        public static void OnActiveGlobesChanged(Changed<GlobeCoopPuzzleController> changed)
        {
            // Get the old values
            changed.LoadOld();
            if (changed.Behaviour.ActiveGlobes.Count == 0) // Synch list just initialized
                return;
            // Store the old list
            var oldList = changed.Behaviour.ActiveGlobes;
            // Switch back to the new list
            changed.LoadNew();
            // Store the new list
            var newList = changed.Behaviour.ActiveGlobes;
            // Loop
            for(int i=0; i<newList.Count; i++)
            {
                if (newList[i] == oldList[i]) // Not changed
                    continue;

                if(newList[i] && !oldList[i]) // Globe activated
                {
                    changed.Behaviour.globes[i].LightUp();
                }
                else // Globe deactivated
                {
                    if(!changed.Behaviour.Solved)
                        changed.Behaviour.globes[i].DimLight();
                }

            }
            // Check puzzle solution
            changed.Behaviour.CheckSolution();

        }
    }

}
