using Fusion;
using GOA.Interfaces;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static GOA.Level.LevelBuilder;

namespace GOA
{
    public abstract class PuzzleController : NetworkBehaviour
    {
        public static UnityAction<PuzzleController> OnSolvedChangedCallback;
        public static UnityAction<PuzzleController> OnPuzzleControllerSpawned;

        
        [Networked(OnChanged = nameof(OnSolvedChanged))] public NetworkBool Solved { get; protected set; } = false;

        [Networked] public int PuzzleIndex { get; private set; } = 0;

                
        //public abstract void Initialize(int puzzleIndex);

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

            OnPuzzleControllerSpawned?.Invoke(this);

        }

        public static void OnSolvedChanged(Changed<PuzzleController> changed)
        {
            Debug.LogFormat("OnSolvedChanged:{0}", changed.Behaviour.Solved);
            OnSolvedChangedCallback?.Invoke(changed.Behaviour);

            // Try to save the current snapshot
#if USE_HOST_MIGRATION
            SessionManager.Instance.PushSnapshot();
#endif
        }

        public virtual void Initialize(int puzzleIndex)
        {
            PuzzleIndex = puzzleIndex;
        }
       
  
    }

}
