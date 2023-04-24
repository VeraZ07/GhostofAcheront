using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Interfaces
{
    public interface IHandleController
    {
        void Init(int initialState, int stateCount);

        void SetState(int state);
    }

}
