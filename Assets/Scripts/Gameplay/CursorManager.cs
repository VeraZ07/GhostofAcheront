using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GOA
{
    public class CursorManager : MonoBehaviour
    {
        public static CursorManager Instance { get; private set; }

        [SerializeField]
        Image gameCursorImage;

        [SerializeField]
        Sprite noEffectSprite;
        
        [SerializeField]
        Sprite effectSprite;

        float noEffectSize = .025f;
        float effectSize = .035f;
        float noEffectAlpha = 0.5f;
        float effectAlpha = 0.8f;

        Vector3 gameCursorScale;

        Tweener gameCursorTweener;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                gameCursorScale = gameCursorImage.transform.localScale;
                SceneManager.sceneLoaded += HandleOnSceneLoaded;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
            
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                StartGameCursorEffect();
            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                StopGameCursorEffect();
            }
        }

        void HandleOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(scene.buildIndex > 0) // Game scene
            {
                //HideMenuCursor();
                ShowGameCursor();
            }
            else // Main scene
            {
                ShowMenuCursor();
                //HideGameCursor();
            }
        }

        void HideGameCursor()
        {
            gameCursorImage.enabled = false;
        }

        void HideMenuCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void ShowMenuCursor()
        {
            HideGameCursor();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void ShowGameCursor()
        {
            HideMenuCursor();
            gameCursorImage.enabled = true;
            gameCursorImage.transform.localScale = Vector3.one * noEffectSize;
            gameCursorImage.sprite = noEffectSprite;
            gameCursorImage.color = new Color(1f, 1f, 1f, noEffectAlpha);
        }

      

        public void StartGameCursorEffect()
        {
            if (gameCursorTweener == null)
                gameCursorTweener = gameCursorImage.transform.DOShakeScale(1, .5f).SetLoops(-1);
            else if (!gameCursorTweener.IsPlaying())
            {
                gameCursorImage.sprite = effectSprite;
                gameCursorImage.transform.localScale = Vector3.one * noEffectSize;
                gameCursorImage.color = new Color(1f, 1f, 1f, effectAlpha);
                gameCursorTweener.Restart();
            }
                

            
            //gameCursorImage.transform.localScale = Vector3.one * 2f;
            

        }

        public void StopGameCursorEffect()
        {
            if (gameCursorTweener != null && gameCursorTweener.IsPlaying())
            {
                gameCursorImage.sprite = noEffectSprite;
                gameCursorImage.transform.localScale = Vector3.one * effectSize;
                gameCursorImage.color = new Color(1f, 1f, 1f, noEffectAlpha);
                gameCursorTweener?.Rewind();
            }
            
        }
    }

}

