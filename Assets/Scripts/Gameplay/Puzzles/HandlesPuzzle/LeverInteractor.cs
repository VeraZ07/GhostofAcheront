using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class LeverInteractor : HandleInteractor
    {
        [SerializeField]
        GameObject handleObject;

        [SerializeField]
        float angle = 90f;

       
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override IEnumerator DoMoveImpl(int oldState, int newState)
        {
            yield return handleObject.transform.DORotateQuaternion(Quaternion.AngleAxis(CurrentState * angle, handleObject.transform.forward), 1f).WaitForCompletion();
        }

        public override void Init(int state)
        {
            handleObject.transform.rotation = Quaternion.AngleAxis(state * angle, handleObject.transform.forward);
        }
    }

}

