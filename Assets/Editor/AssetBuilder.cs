using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GOA.Editor
{
    public class AssetBuilder : MonoBehaviour
    {
        [MenuItem("Assets/Create/GOA/Character")]
        public static void CreateAnimationAsset()
        {
            CharacterAsset asset = ScriptableObject.CreateInstance<CharacterAsset>();

            string name = "Character.asset";

            string folder = System.IO.Path.Combine("Assets/Resources", CharacterAsset.ResourceFolder);// "Assets/Resources/AnimationAssets";
                                                                             ////folder = System.IO.Path.Combine(folder, ResourceFolder.Collections);

            if (!System.IO.Directory.Exists(folder))
                System.IO.Directory.CreateDirectory(folder);

            AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(folder, name));

            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }
    }

}
