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
                Init(GetGameManager().LevelSize);
                GetGameManager().OnLeveSizeChanged += OnLevelSizeChanged;
            }
        }

        public void OnDisable()
        {
            if (GetGameManager())
            {
                GetGameManager().OnLeveSizeChanged -= OnLevelSizeChanged;
            }
        }

        void OnLevelSizeChanged(int levelSize)
        {
            Init(levelSize);
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
