using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GOA
{
    public class OptionManager : MonoBehaviour
    {
        public UnityAction<bool> OnSelectedChanged;
        public UnityAction OnApply;

        public const string ResolutionFormatString = "{0}x{1}";
        public const float MouseSensitivityDefault = 5;
        
        public static OptionManager Instance { get; private set; }


        string currentResolution, selectedResolution;
        public string CurrentResolution
        {
            get { return currentResolution; }
        }


        int currentFullScreenMode, selectedFullScreenMode;
        public int CurrentFullScreenMode
        {
            get { return currentFullScreenMode; }
        }

        float currentMouseSensitivity, selectedMouseSensitivity;
        public float CurrentMouseSensitivity
        {
            get { return currentMouseSensitivity; }
        }

        int currentMouseVertical, selectedMouseVertical;
        public int CurrentMouseVertical
        {
            get { return currentMouseVertical; }
        }

        string resolutionParamName = "Resolution";
        string fullScreenModeParamName = "FullScreen";
        string mouseSensitivityParamName = "MouseSensitivity";
        string mouseVerticalParamName = "MouseVertical";

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                LoadOptions();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(Screen.currentResolution);
        }

        /// <summary>
        /// Load options stored in the player prefs
        /// </summary>
        void LoadOptions()
        {
            // Fullscreen mode
            if (PlayerPrefs.HasKey(fullScreenModeParamName))
            {
                currentFullScreenMode = PlayerPrefs.GetInt(fullScreenModeParamName);
                selectedFullScreenMode = currentFullScreenMode;
            }
            else
            {
                currentFullScreenMode = (int)Screen.fullScreenMode;
                selectedFullScreenMode = currentFullScreenMode;
            }

            // Resolution
            if (PlayerPrefs.HasKey(resolutionParamName))
            {
                currentResolution = PlayerPrefs.GetString(resolutionParamName);
                selectedResolution = currentResolution;
                string[] splits = currentResolution.Split('x');
                Screen.SetResolution(int.Parse(splits[0]), int.Parse(splits[1]), (FullScreenMode)currentFullScreenMode);
            }
            else
            {
                
                currentResolution = string.Format(ResolutionFormatString, Screen.width, Screen.height);
                selectedResolution = currentResolution;
            }

            // Mouse sensitivity
            currentMouseSensitivity = PlayerPrefs.GetFloat(mouseSensitivityParamName, MouseSensitivityDefault);
            selectedMouseSensitivity = currentMouseSensitivity;

            // Mouse vertical input
            currentMouseVertical = PlayerPrefs.GetInt(mouseVerticalParamName, 1);
            selectedMouseVertical = currentMouseVertical;

            Debug.Log("ActualResolution:" + currentResolution);
            Debug.Log("ActualFullScreenMode:" + currentFullScreenMode);
        }

        bool CheckOptionsChanged()
        {
            if (selectedResolution != currentResolution)
                return true;
            if (selectedFullScreenMode != currentFullScreenMode)
                return true;
            if (selectedMouseSensitivity != currentMouseSensitivity)
                return true;
            if (currentMouseVertical != selectedMouseVertical)
                return true;
            return false;
        }

        public void SetSelectedResolution(string value)
        {
            selectedResolution = value;
            OnSelectedChanged?.Invoke(CheckOptionsChanged());
            
        }

        public void SetSelectedFullScreenMode(int value)
        {
            selectedFullScreenMode = value;
            OnSelectedChanged?.Invoke(CheckOptionsChanged());
        }

        public void SetSelectedMouseSensitivity(float value)
        {
            selectedMouseSensitivity = value;
            OnSelectedChanged?.Invoke(CheckOptionsChanged());
        }

        public void SetSelectedMouseVertical(int value)
        {
            selectedMouseVertical = value;
            OnSelectedChanged?.Invoke(CheckOptionsChanged());
        }

        public void ApplyChanges()
        {
            // Resolution
            if(currentFullScreenMode != selectedFullScreenMode || currentResolution != selectedResolution)
            {
                currentResolution = selectedResolution;
                currentFullScreenMode = selectedFullScreenMode;
                string[] splits = currentResolution.Split("x");
                Screen.SetResolution(int.Parse(splits[0]), int.Parse(splits[1]), (FullScreenMode)currentFullScreenMode);
            }

            currentMouseSensitivity = selectedMouseSensitivity;
            currentMouseVertical = selectedMouseVertical;

            OnApply?.Invoke();
        }

        public void DiscardChanges()
        {
            selectedFullScreenMode = currentFullScreenMode;
            selectedResolution = currentResolution;
            selectedMouseSensitivity = currentMouseSensitivity;
            selectedMouseVertical = currentMouseVertical;
        }
    }

}
