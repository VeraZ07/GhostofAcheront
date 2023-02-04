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

            public string style = "000";

            public CustomObject(LevelBuilder builder)
            {
                this.builder = builder;
            }

            public string GetCode()
            {
                return string.Format("{0}_{1}", codePrefix, style);
            }
        }

        class Gate : CustomObject
        {
            public Puzzle puzzle;

            public Gate(LevelBuilder builder) : base(builder)
            {
                codePrefix = "gate";
            }
        }

       
    }
}

