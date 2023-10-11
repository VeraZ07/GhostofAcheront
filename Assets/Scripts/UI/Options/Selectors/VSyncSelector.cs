using GOA.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class VSyncSelector : OptionSelectorEx<int>
    {
        protected override void OptionChanged(int newOptionId)
        {
            OptionManager.Instance.SetVSyncSelectedId(newOptionId);
        }

        protected override ICollection<Option<int>> GetOptionList()
        {
            return OptionCollection.VSyncOptionList;
        }

        protected override int GetCurrentOptionId()
        {
            return OptionManager.Instance.VSyncCurrentId;
        }
    }

}
