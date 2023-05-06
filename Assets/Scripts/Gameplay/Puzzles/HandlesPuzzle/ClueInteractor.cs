using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace GOA
{
    public class ClueInteractor : HandleInteractor
    {
        [SerializeField]
        VisualEffect vfx;

        [SerializeField]
        List<Color> colors;

        [SerializeField]
        float intensity = 10f;


        public override IEnumerator DoMoveImpl(int oldState, int newState)
        {
            vfx.SetVector4("Color", ((Vector4) colors[newState]) * intensity);
            yield break;
        }

        public override void Init(int state)
        {
            vfx.SetVector4("Color", ((Vector4)colors[state])* intensity);
        }
    }

}
