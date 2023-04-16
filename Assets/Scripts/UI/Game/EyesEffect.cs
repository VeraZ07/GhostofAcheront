using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GOA.UI
{
    public class EyesEffect : MonoBehaviour
    {
        public static EyesEffect Instance { get; private set; }

        [SerializeField]
        GameObject panel;

        Image image;

        float duration = 0.25f;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                image = panel.GetComponent<Image>();
                Color c = image.color;
                c.a = 0;
                image.color = c;
            }
            else
            {
                Destroy(gameObject);
            }

            
        }

        // Start is called before the first frame update
        void Start()
        {
            

        }

        // Update is called once per frame
        void Update()
        {
           

        }

        public IEnumerator CloseEyes()
        {
            Color c = image.color;
            c.a = 1f;
            yield return DOTween.To(() => image.color, x => image.color = x, c, duration).WaitForCompletion();

        }

        public IEnumerator OpenEyes()
        {
            Color c = image.color;
            c.a = 0f;
            yield return DOTween.To(() => image.color, x => image.color = x, c, duration).WaitForCompletion();
        }
    }

}
