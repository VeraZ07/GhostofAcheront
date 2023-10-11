using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOA.Settings;

namespace GOA.UI
{
    public class DepthOfFieldSelector : OptionSelectorEx<int>
    {
        protected override void OptionChanged(int newOptionId)
        {
            OptionManager.Instance.SetDepthOfFieldSelectedId(newOptionId);
        }

        protected override ICollection<Option<int>> GetOptionList()
        {
            return OptionCollection.DepthOfFieldOptionList;
        }

        protected override int GetCurrentOptionId()
        {
            return OptionManager.Instance.DepthOfFieldCurrentId;
        }
    }

}
