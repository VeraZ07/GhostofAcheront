using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GOA.UI
{
    public class GamePanel : MonoBehaviour
    {
        [SerializeField]
        Button buttonResume;
        
        [SerializeField]
        Button buttonQuit;

       

        private void Awake()
        {
            buttonResume.onClick.AddListener(() => GetComponentInParent<GameMenu>().Hide());
            buttonQuit.onClick.AddListener(() => QuitGame());
        }
        
        
        // Update is called once per frame
        void Update()
        {

        }

        #region private methods
   

        void QuitGame()
        {
            // Get the player camera in order to destroy it on session quit
            Camera.main.transform.parent = null;

            SessionManager.Instance.QuitSession();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        #endregion

    
    }

}
