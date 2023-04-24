using GOA.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class HandleController : MonoBehaviour, IHandleController
    {
        #region fields
        [SerializeField]
        GameObject handleObject;

        [SerializeField]
        float maxAngle = 90f;

        [SerializeField]
        bool circular = false;

        float angleStep = 0f;

        int stateCount, currentState;
        
        #endregion

       
        #region native
        #endregion

        public void Init(int initialState, int stateCount)
        {
            this.stateCount = stateCount;
            currentState = initialState;

            // Compute the maximum movement for each step
            angleStep = maxAngle / stateCount;
            // Set the initial position
            handleObject.transform.rotation = Quaternion.AngleAxis(currentState * angleStep, handleObject.transform.forward);
        }

        public void SetState(int state)
        {
            int oldState = currentState;
            currentState = state;
            if (circular)
            {
                if (currentState >= stateCount)
                    currentState = 0;
                else if (currentState < 0)
                    currentState = stateCount - 1;
            }
            else
            {
                if (currentState >= stateCount)
                    currentState = stateCount-1;
                else if (currentState < 0)
                    currentState = 0;
            }

            // Move
            handleObject.transform.rotation = Quaternion.AngleAxis(currentState * angleStep, handleObject.transform.forward);
        }
    }

}
