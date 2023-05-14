using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.UI
{
    public class LevelSizeSelector : OptionSelector
    {

        GameManager gameManager;

        

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            
            if (GetGameManager())
            {
                Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                Init(GetGameManager().LevelSize);
            }
        }

        protected override void OptionChanged(int value)
        {
            if(GetGameManager())
                GetGameManager().LevelSize = value;
        }

        GameManager GetGameManager()
        {
            if (!gameManager)
                gameManager = FindObjectOfType<GameManager>();
            return gameManager;
        }
    }

}
