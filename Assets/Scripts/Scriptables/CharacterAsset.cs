using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    public class CharacterAsset : ScriptableObject
    {
        public const string ResourceFolder = "Characters";

        [SerializeField]
        int characterId;
        public int CharacterId
        {
            get { return characterId; }
        }

        [SerializeField]
        Sprite avatar;
        public Sprite Avatar
        {
            get { return avatar; }
        }

        [SerializeField]
        NetworkObject characterPrefab;
        public NetworkObject CharacterPrefab
        {
            get { return characterPrefab; }
        }
    }
}

