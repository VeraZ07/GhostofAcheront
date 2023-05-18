using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class LevelBuildingUI : MonoBehaviour
    {
        [SerializeField]
        GameObject panel;

        // Start is called before the first frame update
        void Start()
        {
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            builder.OnLevelBuilt += () => { panel.SetActive(false); };
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
