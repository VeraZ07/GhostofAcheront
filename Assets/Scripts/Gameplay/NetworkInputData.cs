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
        public NetworkBool run;
    }

}
