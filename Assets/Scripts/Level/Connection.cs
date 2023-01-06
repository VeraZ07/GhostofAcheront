using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{
    public class Connection
    {
        // A blocked connection needs the player to resolve some puzzle in order to walk through it ( the puzzle can also be something
        // as simple as getting the key ); connections linking different sectors are blocked
        bool isBlocked = false;

        // List of connections of other rooms this connection is linked to ( the connection is logic and doesn't take into account
        // corridors between rooms ).
        // A connection can also simply lead to a dead-end street in which case the room list must be empy.
        // A connection can also lead to another connection of the same room ( so you can run in circle ).
        List<Connection> targets = new List<Connection>();

        // Puzzle type and eventually puzzle objects references
    }

}
