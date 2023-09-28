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
        public const int MouseSensitivityDefaultId = 4;
        public const int VerticalMouseDefaultId = 0;

        public static OptionManager Instance { get; private set; }


        int resolutionCurrentId, resolutionSelectedId;
        public int ResolutionCurrentId
        {
            get { return resolutionCurrentId; }
        }


        int fullScreenModeCurrentId, fullScreenModeSelectedId;
        public int FullScreenModeCurrentId
        {
            get { return fullScreenModeCurrentId; }
        }

        int mouseSensitivityCurrentId, mouseSensitivitySelectedId;
        public int MouseSensitivityCurrentId
        {
            get { return mouseSensitivityCurrentId; }
        }
        public float MouseSensitivity
        {
            get { return OptionCollection.MouseSensitivityOptionList[mouseSensitivityCurrentId].Value; }
        }


        int verticalMouseCurrentId, verticalMouseSelectedId;
        public int VerticalMouseCurrentId
        {
            get { return verticalMouseCurrentId; }
        }
        public int VerticalMouse
        {
            get { return OptionCollection.InvertedMouseOptionList[verticalMouseCurrentId].Value; }
        }

        string resolutionIdParamName = "Resolution";
        string fullScreenModeIdParamName = "FullScreen";
        string mouseSensitivityIdParamName = "MouseSensitivity";
        string verticalMouseIdParamName = "VerticalMouse";

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                OptionCollection.Init();
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
            if (PlayerPrefs.HasKey(fullScreenModeIdParamName))
            {
                fullScreenModeCurrentId = PlayerPrefs.GetInt(fullScreenModeIdParamName);
                fullScreenModeSelectedId = fullScreenModeCurrentId;
            }
            else
            {
                fullScreenModeCurrentId = (int)Screen.fullScreenMode;
                fullScreenModeSelectedId = fullScreenModeCurrentId;
            }

            // Resolution
            if (PlayerPrefs.HasKey(resolutionIdParamName))
            {
                resolutionCurrentId = PlayerPrefs.GetInt(resolutionIdParamName);
                resolutionSelectedId = resolutionCurrentId;
                //string[] splits = resolutionCurrentId.Split('x');
                Option<Vector2> opt = OptionCollection.ResolutionOptionList[resolutionCurrentId];
                Screen.SetResolution((int)opt.Value.x, (int)opt.Value.y, (FullScreenMode)fullScreenModeCurrentId);
            }
            else
            {
                List<Option<Vector2>> resList = new List<Option<Vector2>>(OptionCollection.ResolutionOptionList);
                resolutionCurrentId = resList.FindIndex(r=>r.Value.x == Screen.width && r.Value.y == Screen.height);
                resolutionSelectedId = resolutionCurrentId;
            }

            // Mouse sensitivity
            mouseSensitivityCurrentId = PlayerPrefs.GetInt(mouseSensitivityIdParamName, MouseSensitivityDefaultId);
            mouseSensitivitySelectedId = mouseSensitivityCurrentId;

            // Mouse vertical input
            verticalMouseCurrentId = PlayerPrefs.GetInt(verticalMouseIdParamName, VerticalMouseDefaultId);
            verticalMouseSelectedId = verticalMouseCurrentId;

            Debug.Log("ActualResolution:" + resolutionCurrentId);
            Debug.Log("ActualFullScreenMode:" + fullScreenModeCurrentId);
        }

        bool CheckOptionsChanged()
        {
            if (resolutionSelectedId != resolutionCurrentId)
                return true;
            if (fullScreenModeSelectedId != fullScreenModeCurrentId)
                return true;
            if (mouseSensitivitySelectedId != mouseSensitivityCurrentId)
                return true;
            if (verticalMouseCurrentId != verticalMouseSelectedId)
                return true;
            return false;
        }

        public void SetResolutionSelectedId(int id)
        {
            resolutionSelectedId = id;
            OnSelectedChanged?.Invoke(CheckOptionsChanged());
            
        }

        public void SetFullScreenModeSelectedId(int id)
        {
            fullScreenModeSelectedId = id;
            OnSelectedChanged?.Invoke(CheckOptionsChanged());
        }

        public void SetMouseSensitivitySelectedId(int id)
        {
            mouseSensitivitySelectedId = id;
            OnSelectedChanged?.Invoke(CheckOptionsChanged());
        }

        public void SetVerticalMouseSelectedId(int value)
        {
            verticalMouseSelectedId = value;
            OnSelectedChanged?.Invoke(CheckOptionsChanged());
        }

        public void ApplyChanges()
        {
            // Resolution
            if(fullScreenModeCurrentId != fullScreenModeSelectedId || resolutionCurrentId != resolutionSelectedId)
            {
                resolutionCurrentId = resolutionSelectedId;
                fullScreenModeCurrentId = fullScreenModeSelectedId;
                Option<Vector2> opt = OptionCollection.ResolutionOptionList[resolutionCurrentId];
                Screen.SetResolution((int)opt.Value.x, (int)opt.Value.y, (FullScreenMode)fullScreenModeCurrentId);
                PlayerPrefs.SetInt(resolutionIdParamName, resolutionCurrentId);
                PlayerPrefs.SetInt(fullScreenModeIdParamName, fullScreenModeCurrentId);
            }

            if(mouseSensitivityCurrentId != mouseSensitivitySelectedId)
            {
                mouseSensitivityCurrentId = mouseSensitivitySelectedId;
                PlayerPrefs.SetInt(mouseSensitivityIdParamName, mouseSensitivityCurrentId);
            }
            
            if(verticalMouseCurrentId != verticalMouseSelectedId)
            {
                verticalMouseCurrentId = verticalMouseSelectedId;
                PlayerPrefs.SetInt(verticalMouseIdParamName, verticalMouseCurrentId);
            }
            

            PlayerPrefs.Save();

            OnApply?.Invoke();
        }

        public void DiscardChanges()
        {
            fullScreenModeSelectedId = fullScreenModeCurrentId;
            resolutionSelectedId = resolutionCurrentId;
            mouseSensitivitySelectedId = mouseSensitivityCurrentId;
            verticalMouseSelectedId = verticalMouseCurrentId;
        }
    }

}
