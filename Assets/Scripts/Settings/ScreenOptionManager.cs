using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Settings
{
    public class ScreenOptionManager : MonoBehaviour
    {
        static ScreenOptionManager Instance { get; set; }

        int vSyncId = -1;
        int resolutionId = -1;
        int fullScreenModeId = -1;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                OptionManager.OnApply += () => { UpdateSettings(false); };
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateSettings(true);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void UpdateSettings(bool forced)
        {
            UpdateResolution(forced);
            
            UpdateVSync(forced);
        }

        void UpdateVSync(bool forced)
        {
            if (!forced && OptionManager.Instance.VSyncCurrentId == vSyncId)
                return;

            vSyncId = OptionManager.Instance.VSyncCurrentId;
            Application.targetFrameRate = vSyncId == 0 ? -1 : Screen.currentResolution.refreshRate;
        }

        void UpdateResolution(bool forced)
        {
            if (!forced && OptionManager.Instance.ResolutionCurrentId == resolutionId && OptionManager.Instance.FullScreenModeCurrentId == fullScreenModeId)
                return;

            resolutionId = OptionManager.Instance.ResolutionCurrentId;
            fullScreenModeId = OptionManager.Instance.FullScreenModeCurrentId;
            Option<Vector2> opt = OptionCollection.ResolutionOptionList[resolutionId];
            Screen.SetResolution((int)opt.Value.x, (int)opt.Value.y, (FullScreenMode)fullScreenModeId);
        }

        

    }

}
