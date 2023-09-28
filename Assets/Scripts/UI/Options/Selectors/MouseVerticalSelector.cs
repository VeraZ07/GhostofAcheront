using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class MouseVerticalSelector : OptionSelectorEx<int>
    {
    
        protected override void OptionChanged(int newOptionId)
        {
            OptionManager.Instance.SetVerticalMouseSelectedId(newOptionId);
        }

        protected override ICollection<Option<int>> GetOptionList()
        {
            return OptionCollection.InvertedMouseOptionList;
        }

        protected override int GetCurrentOptionId()
        {
            return OptionManager.Instance.VerticalMouseCurrentId;
        }
    }

}
