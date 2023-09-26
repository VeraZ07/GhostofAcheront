using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class FullScreenModeSelector : OptionSelector
    {
        protected override void Start()
        {
            base.Start();

            Init((int)Screen.fullScreenMode);
        }

        protected override void OptionChanged(int value)
        {
            OptionManager.Instance.SetSelectedFullScreenMode(value);
        }

    }

}
