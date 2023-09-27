using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
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
    }

}
