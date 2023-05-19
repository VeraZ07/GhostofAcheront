using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GOA.UI
{
    public class SocialButton : MonoBehaviour
    {
        [SerializeField]
        string url;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => { Application.OpenURL(url); });
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
