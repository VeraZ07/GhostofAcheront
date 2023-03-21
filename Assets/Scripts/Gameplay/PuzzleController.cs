using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOA
{
    public abstract class PuzzleController : NetworkBehaviour
    {
        public static UnityAction<PuzzleController> OnSolvedChangedCallback;

        [Networked(OnChanged = nameof(OnSolvedChanged))] public NetworkBool Solved { get; private set; } = false;

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
        }

        public static void OnSolvedChanged(Changed<PuzzleController> changed)
        {
            Debug.LogFormat("OnSolvedChanged:{0}", changed.Behaviour.Solved);
            OnSolvedChangedCallback?.Invoke(changed.Behaviour);
        }

        public virtual void Initialize(int puzzleIndex)
        {
            PuzzleIndex = puzzleIndex;
        }
       
        
    }

}
