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
            volume = GetComponent<Volume>();
            volume.profile.TryGet(out depthOfField);
            volume.profile.TryGet(out fog);
            currentProfile = volume.profile;
        }

        // Start is called before the first frame update
        void Start()
        {
            OptionManager.OnApply += HandleOnOptionsApplied;
            Debug.Log("DOF value:" + depthOfField.quality.GetValue<int>());
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
        }

        void HandleOnOptionsApplied()
        {
            UpdateQuality();
        }

        void UpdateQuality()
        {
            UpdateDepthOfField();
            UpdateFog();
        }

        void UpdateDepthOfField()
        {
            //VolumeParameter<int> vp = new VolumeParameter<int>();
            //vp.value = OptionCollection.DepthOfFieldOptionList[OptionManager.Instance.DepthOfFieldCurrentId].Value;
            //if (vp.value < 0)
            //    depthOfField.active = false;
            //else
            //{
            //    //depthOfField.quality =                    
            //    depthOfField.quality.SetValue(vp);
            //    if (!depthOfField.active)
            //        depthOfField.active = true;
            //}
            UpdateVolumeComponentQuality(depthOfField, OptionCollection.DepthOfFieldOptionList[OptionManager.Instance.DepthOfFieldCurrentId].Value);    
        }

        void UpdateFog()
        {
            //VolumeParameter<int> vp = new VolumeParameter<int>();
            //vp.value = OptionCollection.FogOptionList[OptionManager.Instance.FogCurrentId].Value;
            //if (vp.value < 0)
            //    fog.active = false;
            //else
            //{
            //    //depthOfField.quality =                    
            //    fog.quality.SetValue(vp);
            //    if (!fog.active)
            //        fog.active = true;
            //}
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
    }

}
