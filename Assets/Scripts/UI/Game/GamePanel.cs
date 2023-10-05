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

            if ((SessionManager.Instance.Runner.IsServer || SessionManager.Instance.Runner.IsSharedModeMasterClient) && !SessionManager.Instance.Runner.IsSinglePlayer)
            {
#if USE_HOST_MIGRATION
                StartCoroutine(
#endif
                    DoQuitGame()
#if USE_HOST_MIGRATION
                    )
#endif
                    ;
            }
            else
            {
                SessionManager.Instance.QuitSession();
                //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }

        }

#if USE_HOST_MIGRATION
        IEnumerator DoQuitGame()
#else
        void DoQuitGame()
#endif
        {
#if USE_HOST_MIGRATION
            yield return SessionManager.Instance.Runner.PushHostMigrationSnapshot();
#endif
            SessionManager.Instance.QuitSession();
            //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            
        }



#endregion


    }

}
