using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Interfaces
{
    public interface IHandleManager
    {
        void SetHandleBusy(int handleId, bool value);

        bool IsHandleBusy(int handleId);

        int GetHandleState(int handleId);

        void SetHandleState(int handleId, int value);
    }

}
