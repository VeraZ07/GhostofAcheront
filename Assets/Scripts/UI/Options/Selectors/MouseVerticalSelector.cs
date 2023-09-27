using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class MouseVerticalSelector : OptionSelectorEx<int>
    {
        protected override void Start()
        {
            base.Start();

            //SetCurrentOptionId(GetOptionIdByValue(OptionManager.Instance.CurrentMouseVertical));
        }

        private void OnEnable()
        {
            if (OptionManager.Instance)
                SetCurrentOptionId(GetOptionIdByValue(OptionManager.Instance.CurrentMouseVertical));
                
        }

        protected override void OptionChanged(int newOptionId)
        {
            if(OptionManager.Instance)
                OptionManager.Instance.SetSelectedMouseVertical(GetOption(newOptionId).Value);
        }

        
    }

}
