using GOA.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GOA.UI
{
    public class OptionPanel : MonoBehaviour
    {
        [SerializeField]
        Button buttonBack;

        [SerializeField]
        Button buttonApply;

        private void OnEnable()
        {
            buttonApply.onClick.AddListener(HandleOnApply);
            buttonBack.onClick.AddListener(HandleOnBack);
            buttonApply.interactable = false;
            
            OptionManager.OnSelectedChanged += HandleOnOptionsChanged;
        }

        private void OnDisable()
        {
            buttonApply.onClick.RemoveAllListeners();
            buttonBack.onClick.RemoveAllListeners();
            OptionManager.OnSelectedChanged -= HandleOnOptionsChanged;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void HandleOnApply()
        {
            OptionManager.Instance.ApplyChanges();
            buttonApply.interactable = false;
        }

        void HandleOnBack()
        {
            OptionManager.Instance.DiscardChanges();
            if(SceneManager.GetActiveScene().buildIndex == GameConfig.MainSceneId)
                GetComponentInParent<MainMenu>().ActivateMainPanel();
            else
                GetComponentInParent<GameMenu>().ActivateGamePanel();
        }

        void HandleOnOptionsChanged(bool changed)
        {
            if (changed)
                buttonApply.interactable = true;
            else
                buttonApply.interactable = false;
        }
    }

}
