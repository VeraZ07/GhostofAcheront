using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Interfaces
{
    public interface IHandle
    {

        //void Init(int initialState, int finalState, int stateCount);
        void Init(PuzzleController puzzleController, int handleId, int initialState, int finalState, int stateCount, bool stopOnFinalState);

        void Move(int oldState, int newState);
    }

}
