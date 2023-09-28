using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GOA.UI
{
    public abstract class OptionSelector : MonoBehaviour
    {
        [SerializeField]
        string[] options;
       
        
        [SerializeField]
        int defaultOptionId;

        [SerializeField]
        TMP_Text textValue;

        [SerializeField]
        Button buttonPrev;

        [SerializeField]
        Button buttonNext;

        [SerializeField]
        bool serverOnlyInput = false;

        int currentOptionId;
       

        protected abstract void OptionChanged(int value);
               

        protected virtual void Awake()
        {
            buttonNext.onClick.AddListener(OnNext);
            buttonPrev.onClick.AddListener(OnPrev);
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            //OptionChanged(currentOptionId);
        }

        protected void Init(int optionId, string[] options = null, int defaultOptionId = -1)
        {
            if (options != null)
                this.options = options;

            if (defaultOptionId >= 0)
                this.defaultOptionId = defaultOptionId;

            currentOptionId = optionId;
            textValue.text = this.options[currentOptionId];

            CheckButtons();
        }

        //protected void SetOptions(string[] options)
        //{
        //    this.options = options;
        //}

        void OnNext()
        {
            currentOptionId++;
            textValue.text = options[currentOptionId];
            CheckButtons();
            OptionChanged(currentOptionId);
        }

        void OnPrev()
        {
            currentOptionId--;
            textValue.text = options[currentOptionId];
            CheckButtons();
            OptionChanged(currentOptionId);
        }

        void CheckButtons()
        {
            buttonNext.interactable = false;
            buttonPrev.interactable = false;

            if (serverOnlyInput && SessionManager.Instance.Runner.IsClient)
                return;

            if (currentOptionId > 0)
                buttonPrev.interactable = true;
            if (currentOptionId < options.Length - 1)
                buttonNext.interactable = true;
        }
          
        protected string GetOption(int id)
        {
            return options[id];
        }

        protected int GetOptionId(string option)
        {
            return new List<string>(options).FindIndex(o => o == option);
        }
    }

}