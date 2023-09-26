using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class MouseSensitivitySelector : OptionSelector
    {
        protected override void Start()
        {
            base.Start();

            Init(GetOptionId(OptionManager.Instance.CurrentMouseSensitivity.ToString()));
        }

        private void OnEnable()
        {
            if (OptionManager.Instance)
                Init(GetOptionId(OptionManager.Instance.CurrentMouseSensitivity.ToString()));
        }

        protected override void OptionChanged(int value)
        {
            OptionManager.Instance.SetSelectedMouseSensitivity(float.Parse(GetOption(value)));
        }
    }

}
