using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            if(OptionManager.Instance)
                OptionManager.Instance.OnSelectedChanged += HandleOnOptionsChanged;
        }

        private void OnDisable()
        {
            buttonApply.onClick.RemoveAllListeners();
            buttonBack.onClick.RemoveAllListeners();
            if (OptionManager.Instance)
                OptionManager.Instance.OnSelectedChanged -= HandleOnOptionsChanged;
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
            //buttonApply.interactable = false;
            GetComponentInParent<MainMenu>().ActivateMainPanel();
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
