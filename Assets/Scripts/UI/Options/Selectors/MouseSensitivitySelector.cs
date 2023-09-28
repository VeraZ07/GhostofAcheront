using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    
    public class MouseSensitivitySelector : OptionSelectorEx<float>
    {
      
        protected override void OptionChanged(int newOptionId)
        {
            OptionManager.Instance.SetMouseSensitivitySelectedId(newOptionId);
        }

        protected override ICollection<Option<float>> GetOptionList()
        {
            return OptionCollection.MouseSensitivityOptionList;
        }

        protected override int GetCurrentOptionId()
        {
            return OptionManager.Instance.MouseSensitivityCurrentId;
        }
    }

}
