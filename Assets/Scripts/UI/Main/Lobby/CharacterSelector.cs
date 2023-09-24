using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GOA.UI
{
    public class CharacterSelector : MonoBehaviour
    {
        [SerializeField]
        Button buttonPrev;

        [SerializeField]
        Button buttonNext;

        int selectedId = 0;
        int characterCount;

        private void Awake()
        {
            // Init the character asset list

            characterCount = new List<CharacterAsset>(Resources.LoadAll<CharacterAsset>(CharacterAsset.ResourceFolder)).Count;
            

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            buttonPrev.onClick.AddListener(HandleOnPrevCharacter);
            buttonNext.onClick.AddListener(HandleOnNextCharacter);
            // Check buttons
            CheckButtonsInteraction();
        }

        private void OnDisable()
        {
            buttonPrev.onClick.RemoveAllListeners();
            buttonNext.onClick.RemoveAllListeners();
        }

        void HandleOnPrevCharacter()
        {
            selectedId--;
            CheckButtonsInteraction();
            Debug.Log("LocalPlayer:" + Player.Local);
            Debug.Log("LocalPlayer.HasInputAuthority:" + Player.Local.HasInputAuthority);
            Player.Local.RpcSetCharacterId((byte)selectedId);
        }

        void HandleOnNextCharacter()
        {
            selectedId++;
            CheckButtonsInteraction();
            Debug.Log("LocalPlayer:" + Player.Local);
            Debug.Log("LocalPlayer.HasInputAuthority:" + Player.Local.HasInputAuthority);
            Player.Local.RpcSetCharacterId((byte)selectedId);
        }

        void CheckButtonsInteraction()
        {
            buttonNext.interactable = true;
            buttonPrev.interactable = true;
            if (selectedId == 0)
                buttonPrev.interactable = false;
            if (selectedId == characterCount - 1)
                buttonNext.interactable = false;
        }

    }

}
