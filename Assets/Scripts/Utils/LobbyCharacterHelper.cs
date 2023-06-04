using Fusion;
using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
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
            SessionManager.OnPlayerJoinedCallback += HandleOnPlayerJoined;
            SessionManager.OnShutdownCallback += HandleOnShutdown;
            Player.OnCharacterIdChangedCallback += HandleOnCharacterIdChanged;
        }

        private void OnDisable()
        {
            SessionManager.OnPlayerJoinedCallback -= HandleOnPlayerJoined;
            SessionManager.OnShutdownCallback -= HandleOnShutdown;
            Player.OnCharacterIdChangedCallback -= HandleOnCharacterIdChanged;
        }

        public void HandleOnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.LocalPlayer == player)
                ShowCharacter(0);
        }

        public void HandleOnShutdown(NetworkRunner runner, ShutdownReason reason)
        {
            HideCharacter();
        }

        void HandleOnCharacterIdChanged(Player player) 
        {
            if (player.HasInputAuthority)
                ShowCharacter(player.CharacterId);
        }

        void ShowCharacter(int characterId)
        {
            // Destroy the old character if any
            HideCharacter();
            // Load character assets
            characterAssets = new List<CharacterAsset>(Resources.LoadAll<CharacterAsset>(CharacterAsset.ResourceFolder));
            characterObject = characterAssets.Find(c => c.CharacterId == Player.Local.CharacterId).LobbyPlaceholderPrefab;
            // Instantiate the new character
            characterObject = Instantiate(characterObject, characterPivot.position, characterPivot.rotation);
            characterObject.SetActive(true);
            pillarObject.SetActive(true);
            // Hide statue
            statueObject.SetActive(false);
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
