using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GOA.UI
{
    public abstract class OptionSelectorEx<T> : MonoBehaviour 
    {
        [SerializeField]
        List<Option<T>> options;
        protected List<Option<T>> Options
        {
            get { return options; }
            set { options = value; }
        }

        [SerializeField]
        int defaultOptionId = -1;

        [SerializeField]
        TMP_Text textValue;

        [SerializeField]
        Button buttonPrev;

        [SerializeField]
        Button buttonNext;

        [SerializeField]
        bool serverOnlyInput = false;

        

        int currentOptionId = -1;
        
        

        protected abstract void OptionChanged(int newOptionId);

        protected virtual void Awake()
        {
            buttonNext.onClick.AddListener(OnNext);
            buttonPrev.onClick.AddListener(OnPrev);

            if (defaultOptionId >= 0)
                SetCurrentOptionId(defaultOptionId);


        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            
        }

        void OnNext()
        {
            currentOptionId++;
            textValue.text = options[currentOptionId].Text;
            CheckButtons();
            OptionChanged(currentOptionId);
        }

        void OnPrev()
        {
            currentOptionId--;
            textValue.text = options[currentOptionId].Text;
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
            if (currentOptionId < options.Count - 1)
                buttonNext.interactable = true;
        }

        protected int GetOptionIdByValue(T value)
        {
            return new List<Option<T>>(options).FindIndex(o => o.Value.Equals(value));
        }

        protected Option<T> GetOption(int id)
        {
            return options[id];
        }

        protected void SetCurrentOptionId(int id)
        {
            currentOptionId = id;

            textValue.text = (this.options[currentOptionId] as Option<T>).Text;
            CheckButtons();
            OptionChanged(currentOptionId);
        }
    }

}