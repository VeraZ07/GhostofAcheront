using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector2 move;
        public float yaw;
#if SYNCH_PITCH
        public float pitch;
#endif
        public NetworkBool run;
        public bool leftAction;
        public bool rightAction;
    }

}
