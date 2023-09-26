using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class ResolutionSelector : OptionSelector
    {
        //string resolutionFormatString = "{0}x{1}";
        

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            // Set options in the parent class
            string[] options = GetResolutions();
            int currentId = GetCurrentOptionId(options);
            Init(currentId, options);
        }

        protected override void OptionChanged(int value)
        {
            // Set the selected resolution
            OptionManager.Instance.SetSelectedResolution(GetOption(value));
        }

        string[] GetResolutions()
        {
            List<string> ret = new List<string>();

            

            foreach(Resolution resolution in Screen.resolutions)
            {
                string resStr = string.Format(OptionManager.ResolutionFormatString, resolution.width, resolution.height);
                if (!ret.Contains(resStr))
                    ret.Add(resStr);
            }
                        

            return ret.ToArray();
        } 
        
        int GetCurrentOptionId(string[] options)
        {
            Debug.Log("CurrentResolution:" + Screen.currentResolution);
            string cur = OptionManager.Instance.CurrentResolution;
            return new List<string>(options).FindIndex(s => s.Equals(cur));
        }

    
    }

}
