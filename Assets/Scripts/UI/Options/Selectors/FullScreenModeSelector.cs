using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOA.Settings;

namespace GOA.UI
{
    public class FullScreenModeSelector : OptionSelectorEx<int>
    {
        //protected override void Start()
        //{
        //    base.Start();

        //    Init((int)Screen.fullScreenMode);
        //}


        protected override void OptionChanged(int newOptionId)
        {
            OptionManager.Instance.SetFullScreenModeSelectedId(newOptionId);
        }

        protected override ICollection<Option<int>> GetOptionList()
        {
            return OptionCollection.FullScreenModeOptionList;
        }

        protected override int GetCurrentOptionId()
        {
            return OptionManager.Instance.FullScreenModeCurrentId;
        }
    }

}
