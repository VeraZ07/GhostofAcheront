using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GOA.UI
{
    public class NotAvailable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        GameObject textUI;

        [SerializeField]
        bool clientOnly = false;

        float time = 0;
        bool loop = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (loop)
            {
                time += Time.deltaTime;

                if (time > .25f)
                    textUI.SetActive(true);

            }
        }

        private void OnEnable()
        {
            textUI.SetActive(false);

            Debug.Log("OnEnabled:" + gameObject);

            if (!SessionManager.Instance || !SessionManager.Instance.Runner)
                return;

            Debug.Log("OnEnabled passed instance:" + gameObject);

            if (!clientOnly || SessionManager.Instance.Runner.IsClient)
            {
                GetComponent<Image>().raycastTarget = true;
            }
            else
            {
                GetComponent<Image>().raycastTarget = false;
            }
        }

        private void OnDisable()
        {
            textUI.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
           
                //do stuff
                loop = true;
                time = 0;
            
          
        }

        public void OnPointerExit(PointerEventData eventData)
        {
                //do stuff
                loop = false;
                time = 0;
                textUI.SetActive(false);
                
         
        }
    }

}
