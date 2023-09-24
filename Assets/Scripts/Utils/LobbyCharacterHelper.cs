using Fusion;
using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GOA
{
    public class LobbyCharacterHelper : MonoBehaviour
    {
        public static LobbyCharacterHelper Instance { get; private set; }

        [SerializeField]
        GameObject statueObject;

        [SerializeField]
        GameObject pillarObject;

        [SerializeField]
        Transform characterPivot;

        [SerializeField]
        TMP_Text textCharacterName;

        List<CharacterAsset> characterAssets;
        GameObject characterObject;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            HideCharacter();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            Player.OnSpawned += HandleOnPlayerSpawned;
            Player.OnDespawned += HandleOnPlayerDespawned;
            Player.OnCharacterIdChangedCallback += HandleOnCharacterIdChanged;
        }

        private void OnDisable()
        {
            Player.OnCharacterIdChangedCallback -= HandleOnCharacterIdChanged;
            Player.OnSpawned -= HandleOnPlayerSpawned;
            Player.OnDespawned -= HandleOnPlayerDespawned;
        }

        public void HandleOnPlayerSpawned(Player player)
        {
            if (player.HasInputAuthority)
                ShowCharacter();
        }

        public void HandleOnPlayerDespawned(Player player)
        {
            if(player.HasInputAuthority)
                HideCharacter();
        }

        void HandleOnCharacterIdChanged(Player player) 
        {
            Debug.Log("Character changed:" + player.CharacterId);

            if (player.HasInputAuthority)
                ShowCharacter();
                
        }

        void ShowCharacter()
        {
            // Destroy the old character if any
            HideCharacter();
            // Load character assets
            characterAssets = new List<CharacterAsset>(Resources.LoadAll<CharacterAsset>(CharacterAsset.ResourceFolder));
            CharacterAsset characterAsset = characterAssets.Find(c => c.CharacterId == Player.Local.CharacterId);
            characterObject = characterAsset.LobbyPlaceholderPrefab;
            // Instantiate the new character
            characterObject = Instantiate(characterObject, characterPivot.position, characterPivot.rotation);
            characterObject.SetActive(true);
            pillarObject.SetActive(true);
            // Hide statue
            statueObject.SetActive(false);

            // Set name
            textCharacterName.text = characterAsset.CharacterName;
        }

        void HideCharacter()
        {
            pillarObject.SetActive(false);
            if (characterObject)
                Destroy(characterObject);
            statueObject.SetActive(true);
        }
    }

}
