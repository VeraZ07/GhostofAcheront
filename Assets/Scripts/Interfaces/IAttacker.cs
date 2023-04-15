
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOA.Interfaces
{
    public interface IAttacker
    {
        public const string ParamAttackTrigger = "Attack";
        public const string ParamAttackId = "AttackId";

        void Kill(PlayerController victim);

        int GetAttackId();
        
    }

}
