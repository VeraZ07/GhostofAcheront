using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace GOA.Settings
{
    public class VolumeOptionManager : MonoBehaviour
    {
        Volume volume;

        DepthOfField depthOfField;
        Fog fog;

        VolumeProfile currentProfile;

        private void Awake()
        {
            //volume = GetComponent<Volume>();
            InitData(GetComponent<Volume>());
        }

        // Start is called before the first frame update
        void Start()
        {
            OptionManager.OnApply += HandleOnOptionsApplied;
            VolumeUtility.OnProfileChanged += HandleOnProfileChanged;
            UpdateQuality();
        }

        private void LateUpdate()
        {
            if(volume.profile != currentProfile)
            {
                currentProfile = volume.profile;
                UpdateQuality();
            }
        }

        private void OnDestroy()
        {
            OptionManager.OnApply -= HandleOnOptionsApplied;
            VolumeUtility.OnProfileChanged -= HandleOnProfileChanged;
        }

        void InitData(Volume volume)
        {
            this.volume = volume;
            volume.profile.TryGet(out depthOfField);
            volume.profile.TryGet(out fog);
            currentProfile = volume.profile;
        }

        void HandleOnOptionsApplied()
        {
            UpdateQuality();
        }

        void HandleOnProfileChanged(Volume volume)
        {
            InitData(volume);
            UpdateQuality();
        }

        void UpdateQuality()
        {
            UpdateDepthOfField();
            UpdateFog();
        }

        void UpdateDepthOfField()
        {
            UpdateVolumeComponentQuality(depthOfField, OptionCollection.DepthOfFieldOptionList[OptionManager.Instance.DepthOfFieldCurrentId].Value);    
        }

        void UpdateFog()
        {
            UpdateVolumeComponentQuality(fog, OptionCollection.FogOptionList[OptionManager.Instance.FogCurrentId].Value);
        }

        void UpdateVolumeComponentQuality(VolumeComponentWithQuality component, int quality)
        {
            VolumeParameter<int> vp = new VolumeParameter<int>();
            vp.value = quality;
            if (vp.value < 0)
                component.active = false;
            else
            {
                //depthOfField.quality =                    
                component.quality.SetValue(vp);
                if (!component.active)
                    component.active = true;
            }
        }

        public void SetProfile(VolumeProfile profile)
        {
            volume.profile = profile;
            UpdateQuality();
        }
    }

}
