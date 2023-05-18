using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GOA.UI
{
    public class VersionUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text version;

        private void Awake()
        {
            version.text = Application.version;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
