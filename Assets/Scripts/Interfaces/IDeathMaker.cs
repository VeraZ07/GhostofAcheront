
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOA.Interfaces
{
    public interface IDeathMaker
    {
        
        void Kill(PlayerController victim, int attackType);
        
    }

}
