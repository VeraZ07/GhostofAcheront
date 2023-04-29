using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class YouWinUI : MonoBehaviour
    {
        [SerializeField]
        GameObject panel;

        private void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            panel.SetActive(false);
            FindObjectOfType<GameManager>().OnGameWin += HandleOnGameWin;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void HandleOnGameWin()
        {
            panel.SetActive(true);
        }

     
    }

}
