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

        [MenuItem("Assets/Create/GOA/Puzzles/HandlesPuzzleAsset")]
        public static void CreateHandlesPuzzleAsset()
        {
            HandlesPuzzleAsset asset = ScriptableObject.CreateInstance<HandlesPuzzleAsset>();

            string name = "HandlesPuzzle.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, PuzzleAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/GOA/Puzzles/PicturePuzzleAsset")]
        public static void CreatePicturePuzzleAsset()
        {
            PicturePuzzleAsset asset = ScriptableObject.CreateInstance<PicturePuzzleAsset>();

            string name = "PicturePuzzleAsset.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, PuzzleAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/GOA/Puzzles/MemoryPuzzleAsset")]
        public static void CreateMemoryPuzzleAsset()
        {
            MemoryPuzzleAsset asset = ScriptableObject.CreateInstance<MemoryPuzzleAsset>();

            string name = "MemoryPuzzleAsset.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, PuzzleAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/GOA/Puzzles/JigSawPuzzleAsset")]
        public static void CreateJigSawPuzzleAsset()
        {
            JigSawPuzzleAsset asset = ScriptableObject.CreateInstance<JigSawPuzzleAsset>();

            string name = "JigSawPuzzleAsset.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, PuzzleAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/GOA/Puzzles/FifteenPuzzleAsset")]
        public static void CreateFifteenPuzzleAsset()
        {
            FifteenPuzzleAsset asset = ScriptableObject.CreateInstance<FifteenPuzzleAsset>();

            string name = "FifteenPuzzleAsset.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, PuzzleAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/GOA/Puzzles/ArrowPuzzleAsset")]
        public static void CreateArrowPuzzleAsset()
        {
            ArrowPuzzleAsset asset = ScriptableObject.CreateInstance<ArrowPuzzleAsset>();

            string name = "ArrowPuzzleAsset.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, PuzzleAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/GOA/Puzzles/GlobeCoopPuzzleAsset")]
        public static void CreateGlobeCoopPuzzleAsset()
        {
            GlobeCoopPuzzleAsset asset = ScriptableObject.CreateInstance<GlobeCoopPuzzleAsset>();

            string name = "GlobeCoopPuzzleAsset.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, PuzzleAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/GOA/ItemAsset")]
        public static void CreateItemAsset()
        {
            ItemAsset asset = ScriptableObject.CreateInstance<ItemAsset>();

            string name = "Item.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, ItemAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/GOA/MonsterAsset")]
        public static void CreateMonsterAsset()
        {
            MonsterAsset asset = ScriptableObject.CreateInstance<MonsterAsset>();

            string name = "Monster.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, MonsterAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/GOA/ObjectGroupAsset")]
        public static void CreateObjectGroupAsset()
        {
            ObjectGroupAsset asset = ScriptableObject.CreateInstance<ObjectGroupAsset>();

            string name = "ObjectGroupAsset.asset";

            string folder = System.IO.Path.Combine(ResourceFolder, ObjectGroupAsset.ResourceFolder);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }

}
