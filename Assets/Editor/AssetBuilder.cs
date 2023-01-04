using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GOA.Editor
{
    public class AssetBuilder : MonoBehaviour
    {
        static string ResourceFolder = "Assets/Resources";

        [MenuItem("Assets/Create/GOA/Character")]
        public static void CreateCharacterAsset()
        {
            CharacterAsset asset = ScriptableObject.CreateInstance<CharacterAsset>();

            string name = "Character.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, CharacterAsset.ResourceFolder);// "Assets/Resources/AnimationAssets";
                                                                             ////folder = System.IO.Path.Combine(folder, ResourceFolder.Collections);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/GOA/Room")]
        public static void CreateRoomAsset()
        {
            RoomAsset asset = ScriptableObject.CreateInstance<RoomAsset>();

            string name = "Room.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, RoomAsset.ResourceFolder);
                                                                                             
            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/GOA/Corridor")]
        public static void CreateCorridorAsset()
        {
            CorridorAsset asset = ScriptableObject.CreateInstance<CorridorAsset>();

            string name = "Corridor.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, CorridorAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }

}
