using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class OptionManager : MonoBehaviour
    {
        public const string DefaultResolution = "1024x768";

        public static OptionManager Instance { get; private set; }

        

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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(Screen.currentResolution);
        }

        /// <summary>
        /// Load options stored in the player prefs
        /// </summary>
        void LoadOptions()
        {
            
        }

        
    }

}
