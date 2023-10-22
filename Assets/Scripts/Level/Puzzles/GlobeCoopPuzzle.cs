using Fusion;
using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{
    partial class LevelBuilder
    {
        public class GlobeCoopPuzzle : Puzzle
        {
            public class Globe
            {
                int customObjectId;
                public int CustomObjectId
                {
                    get { return customObjectId; }
                }

                public Globe(int customObjectId)
                {
                    this.customObjectId = customObjectId;
                }
            }


            List<Globe> globes = new List<Globe>();
            public IList<Globe> Globes
            {
                get { return globes.AsReadOnly(); }
            }

           
            public GlobeCoopPuzzle(LevelBuilder builder, PuzzleAsset asset, int sectorId) : base(builder, asset, sectorId)
            {
                // Get the number of players 
                // In single player mode the actual playerCount value is zero, but we only play this puzzle in coop
                int playerCount = SessionManager.Instance.Runner.SessionInfo.PlayerCount;
                playerCount = 2;
               
                // We need to spawn a globe for each player
                for(int i=0; i<playerCount; i++)
                {
                    // Create the custom object
                    CustomObject co = new CustomObject(builder, (asset as GlobeCoopPuzzleAsset).GlobeAsset);
                    builder.customObjects.Add(co);
                    co.AttachRandomly(sectorId, true);

                    // Create the globe that will be referenced by the puzzle controller
                    globes.Add(new Globe(builder.CustomObjects.Count - 1));
                }
            }

            public override void CreateSceneObjects()
            {
               
                // Loop through all the ids
                for (int i = 0; i < globes.Count; i++)
                {
                    builder.customObjects[globes[i].CustomObjectId].CreateSceneObject();
                }
            }


        }
    }

   

}

