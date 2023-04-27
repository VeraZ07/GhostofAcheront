using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class YouLoseUI : MonoBehaviour
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
            FindObjectOfType<GameManager>().OnGameLose += HandleOnGameLose;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void HandleOnGameLose()
        {
            StartCoroutine(DoHandleOnGameLose());
        }

        IEnumerator DoHandleOnGameLose()
        {
            panel.SetActive(true);
            yield return new WaitForSeconds(5f);

            // Get the player camera in order to destroy it on session quit
            Camera.main.transform.parent = null;

            SessionManager.Instance.QuitSession();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

}
