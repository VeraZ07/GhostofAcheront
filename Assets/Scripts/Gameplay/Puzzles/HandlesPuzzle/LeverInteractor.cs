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

        [SerializeField]
        AudioSource audioSource;

        [SerializeField]
        AudioClip interactionClip;
       
       
        public override IEnumerator DoMoveImpl(int oldState, int newState)
        {

            if (audioSource && interactionClip)
            {
                audioSource.clip = interactionClip;
                audioSource.Play();
            }
            yield return handleObject.transform.DOLocalRotate(Vector3.forward * newState * angle, 1f).WaitForCompletion();
        }

        public override void Init(int state)
        {
            //handleObject.transform.localRotation = Quaternion.AngleAxis(state * angle, handleObject.transform.forward);
            handleObject.transform.DOLocalRotate(Vector3.forward * state * angle, 1f);
        }
    }

}

