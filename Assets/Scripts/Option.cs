using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    [System.Serializable]
    public class Option<T>
    {
        [SerializeField]
        T value;
        public T Value
        {
            get { return value; }
        }

        [SerializeField]
        string text;
        public string Text
        {
            get { return text; }
        }

        public Option(T value, string text)
        {
            this.value = value;
            this.text = text;
        }

    }

}
