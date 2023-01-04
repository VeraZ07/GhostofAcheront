using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{
    public class Corridor
    {
        // Each item connect to another corridor or a room
        List<Connection> connections = new List<Connection>();

        // The size of the corridor in tiles
        Vector2 size = Vector2.one;
    }

}
