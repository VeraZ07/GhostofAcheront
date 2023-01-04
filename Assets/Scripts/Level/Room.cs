using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{

    public class Room
    {
        // Each item gets a connection to a corridor ( a room can not be connected to another room )
        List<Connection> connections = new List<Connection>();

        // The size of the room in tiles
        Vector2 size = Vector2.one;

        
    }

}
