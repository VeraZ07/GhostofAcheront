using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
  
    public class HandlesPuzzleAsset: PuzzleAsset
    {
        [System.Serializable]
        public class Handle
        {
            [SerializeField]
            CustomObjectAsset asset;

            public CustomObjectAsset Asset
            {
                get { return asset; }
            }
            
            [SerializeField]
            int initialState = -1; // Negative for random state
            public int InitialState
            {
                get { return initialState; }
            }

            [SerializeField]
            int finalState = -1; // Negative for random state
            public int FinalState
            {
                get { return finalState; }
            }

            [SerializeField]
            int stateCount = 2;
            public int StateCount
            {
                get { return stateCount; }
            }
        }

        [SerializeField]
        List<Handle> handles;

        public IList<Handle> Handles
        {
            get { return handles.AsReadOnly(); }
        }

        [SerializeField]
        bool stopHandleOnFinalState = false;
        public bool StopHandleOnFinalState
        {
            get { return stopHandleOnFinalState; }
        }

        [SerializeField]
        List<int> clue; // The state 
        public List<int> Clue
        {
            get { return clue; }
        }

        [SerializeField]
        CustomObjectAsset clueAsset;

        //[SerializeField]
        //bool circular = false;
        //public bool Circular
        //{
        //    get { return circular; }
        //}
    }

}
