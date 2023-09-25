using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class ResolutionSelector : OptionSelector
    {
        string resolutionFormatString = "{0}x{1}";
        

        protected override void Awake()
        {
            base.Awake();

            // Set options in the parent class
            string[] options = GetOptions();
            int currentId = GetCurrentOptionId(options);
            Init(currentId, options);
        }

        protected override void OptionChanged(int value)
        {
            Debug.Log($"New resolution selected:" + CurrentOption);
            // Retrieve the same resolution with the highest refresh rate
            string[] splits = CurrentOption.Split('x');
            int width = int.Parse(splits[0]);
            int height = int.Parse(splits[1]);
            Screen.SetResolution(width, height, FullScreenMode.Windowed);
        }

        string[] GetOptions()
        {
            List<string> ret = new List<string>();

            

            foreach(Resolution resolution in Screen.resolutions)
            {
                string resStr = string.Format(resolutionFormatString, resolution.width, resolution.height);
                if (!ret.Contains(resStr))
                    ret.Add(resStr);
            }
                        

            return ret.ToArray();
        } 
        
        int GetCurrentOptionId(string[] options)
        {
            Debug.Log("CurrentResolution:" + Screen.currentResolution);
            string cur = string.Format(resolutionFormatString, Screen.currentResolution.width, Screen.currentResolution.height);
            return new List<string>(options).FindIndex(s => s.Equals(cur));
        }

    
    }

}
