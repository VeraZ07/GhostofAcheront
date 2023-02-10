using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{
    public partial class LevelBuilder : MonoBehaviour
    {
        [System.Serializable]
        class CustomObject
        {

            LevelBuilder builder;

            public GameObject sceneObject;


            public Vector3 direction;

            public int tileId;

            protected string codePrefix;

            //public string style = "000";
            public CustomObjectAsset asset;

            public CustomObject(LevelBuilder builder, CustomObjectAsset asset)
            {
                this.builder = builder;
                this.asset = asset;
            }

           
        }

        class Gate : CustomObject
        {
            public int puzzleIndex;

            public Gate(LevelBuilder builder, CustomObjectAsset asset) : base(builder, asset)
            {
                
            }
        }

       
    }
}

