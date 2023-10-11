using GOA.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class FogSelector : OptionSelectorEx<int>
    {
        protected override void OptionChanged(int newOptionId)
        {
            OptionManager.Instance.SetFogSelectedId(newOptionId);
        }

        protected override ICollection<Option<int>> GetOptionList()
        {
            return OptionCollection.FogOptionList;
        }

        protected override int GetCurrentOptionId()
        {
            return OptionManager.Instance.FogCurrentId;
        }
    }

}
