using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace GOA
{
    public class VolumeUtility
    {
        public static UnityAction<Volume> OnProfileChanged;

        public static void SetProfile(VolumeProfile profile)
        {
            SetProfile(GameObject.FindObjectOfType<UnityEngine.Rendering.Volume>(), profile);
            
        }

        public static void SetProfile(Volume volume, VolumeProfile profile)
        {
            volume.profile = profile;
            OnProfileChanged?.Invoke(volume);
        }
    }

}
