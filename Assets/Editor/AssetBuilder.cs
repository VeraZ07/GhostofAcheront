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
            TileAsset asset = ScriptableObject.CreateInstance<TileAsset>();

            string name = "Room.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, TileAsset.ResourceFolder);
                                                                                             
            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/GOA/Tile")]
        public static void CreateTileAsset()
        {
            TileAsset asset = ScriptableObject.CreateInstance<TileAsset>();

            string name = "Tile.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, TileAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/GOA/CustomObjectAsset")]
        public static void CreateCustomObjectAsset()
        {
            CustomObjectAsset asset = ScriptableObject.CreateInstance<CustomObjectAsset>();

            string name = "CustomObject.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, CustomObjectAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/GOA/Puzzles/MultiStatePuzzleAsset")]
        public static void CreatePuzzleAsset()
        {
            MultiStatePuzzleAsset asset = ScriptableObject.CreateInstance<MultiStatePuzzleAsset>();

            string name = "MultiStatePuzzle.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, PuzzleAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }

}
