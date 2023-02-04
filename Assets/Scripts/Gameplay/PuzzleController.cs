using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOA
{
    public class PuzzleController : NetworkBehaviour
    {
        public static UnityAction<PuzzleController> OnStateChangedCallback;

        [Networked(OnChanged = nameof(OnStateChanged))] int State { get; set; } = 0;

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

        public static void OnStateChanged(Changed<PuzzleController> changed)
        {
            Debug.LogFormat("OnStateChanged:{0}", changed.Behaviour.State);
            OnStateChangedCallback?.Invoke(changed.Behaviour);
        }
    }

}
