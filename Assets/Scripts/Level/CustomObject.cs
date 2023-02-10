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

            //public string GetCode()
            //{
            //    return string.Format("{0}_{1}", codePrefix, style);
            //}
        }

        class Gate : CustomObject
        {
            public Puzzle puzzle;

            public Gate(LevelBuilder builder, CustomObjectAsset asset) : base(builder, asset)
            {
                //codePrefix = "gate";
            }
        }

       
    }
}

