using GOA.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace GOA.Settings
{
    public class OptionCollection
    {
        static List<Option<float>> mouseSensitivityOptionList = new List<Option<float>>();
        public static IList<Option<float>> MouseSensitivityOptionList
        {
            get { return mouseSensitivityOptionList.AsReadOnly(); }
        }

        static List<Option<int>> invertedMouseOptionList = new List<Option<int>>();
        public static IList<Option<int>> InvertedMouseOptionList
        {
            get { return invertedMouseOptionList.AsReadOnly(); }
        }

        static List<Option<int>> fullScreenModeOptionList = new List<Option<int>>();
        public static IList<Option<int>> FullScreenModeOptionList
        {
            get { return fullScreenModeOptionList.AsReadOnly(); }
        }

        static List<Option<Vector2>> resolutionOptionList = new List<Option<Vector2>>();
        public static IList<Option<Vector2>> ResolutionOptionList
        {
            get { return resolutionOptionList.AsReadOnly(); }
        }

        static List<Option<int>> depthOfFieldOptionList = new List<Option<int>>();
        public static IList<Option<int>> DepthOfFieldOptionList
        {
            get { return depthOfFieldOptionList.AsReadOnly(); }
        }

        static List<Option<int>> fogOptionList = new List<Option<int>>();
        public static IList<Option<int>> FogOptionList
        {
            get { return fogOptionList.AsReadOnly(); }
        }

        static List<Option<int>> vSyncOptionList = new List<Option<int>>();
        public static IList<Option<int>> VSyncOptionList
        {
            get { return vSyncOptionList.AsReadOnly(); }
        }

        public static void Init()
        {
            //
            // Mouse sensitivity
            for (int i = 1; i <= 10; i++)
            {
                mouseSensitivityOptionList.Add(new Option<float>(i * .2f, i.ToString()));
            }

            //
            // Inverted mouse
            invertedMouseOptionList.Add(new Option<int>(1, "Normal"));
            invertedMouseOptionList.Add(new Option<int>(-1, "Inverted"));

            //
            // Fullscreen mode
            fullScreenModeOptionList.Add(new Option<int>((int)FullScreenMode.ExclusiveFullScreen, "ExclusiveFullScreen"));
            fullScreenModeOptionList.Add(new Option<int>((int)FullScreenMode.FullScreenWindow, "FullScreenWindow"));
            fullScreenModeOptionList.Add(new Option<int>((int)FullScreenMode.MaximizedWindow, "MaximizedWindow"));
            fullScreenModeOptionList.Add(new Option<int>((int)FullScreenMode.Windowed, "Windowed"));

            //
            // Resolution 
            foreach (Resolution resolution in Screen.resolutions)
            {
                string resStr = string.Format(OptionManager.ResolutionFormatString, resolution.width, resolution.height);
                if (!resolutionOptionList.Exists(r=>r.Text.Equals(resStr)))
                    resolutionOptionList.Add(new Option<Vector2>(new Vector2(resolution.width,resolution.height), resStr));
            }

            //
            // Depth of field
            depthOfFieldOptionList.Add(new Option<int>(-1, "Off"));
            depthOfFieldOptionList.Add(new Option<int>((int)ScalableSettingLevelParameter.Level.Low, "Low"));
            depthOfFieldOptionList.Add(new Option<int>((int)ScalableSettingLevelParameter.Level.Medium, "Medium"));
            depthOfFieldOptionList.Add(new Option<int>((int)ScalableSettingLevelParameter.Level.High, "High"));

            //
            // Fog
            //fogOptionList.Add(new Option<int>(-1, "Off"));
            fogOptionList.Add(new Option<int>((int)ScalableSettingLevelParameter.Level.Low, "Low"));
            fogOptionList.Add(new Option<int>((int)ScalableSettingLevelParameter.Level.Medium, "Medium"));
            fogOptionList.Add(new Option<int>((int)ScalableSettingLevelParameter.Level.High, "High"));

            //
            // VSync
            vSyncOptionList.Add(new Option<int>(0, "Off"));
            vSyncOptionList.Add(new Option<int>(1, "On"));
        }

    }

}
