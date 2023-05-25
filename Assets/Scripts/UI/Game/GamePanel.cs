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

            if (SessionManager.Instance.Runner.IsServer && !SessionManager.Instance.Runner.IsSinglePlayer)
            {
                StartCoroutine(DoQuitGame());
            }
            else
            {
                SessionManager.Instance.QuitSession();
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
            
        }

        IEnumerator DoQuitGame()
        {
            SessionManager.Instance.Runner.PushHostMigrationSnapshot().ContinueWith((t)=> {
                if (t.IsCompleted)
                {
                    if (t.IsFaulted)
                    {
                        Debug.Log("PushSnapshot failed");
                    }
                    else
                    {
                        Debug.Log("PushSnapshot succeeded");
                    }
                }
            });
            yield return new WaitForSeconds(3.2f);
            SessionManager.Instance.QuitSession();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            
        }

        #endregion

    
    }

}
