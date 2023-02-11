using GOA.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Level
{
    public partial class LevelBuilder : MonoBehaviour
    {
      
        static class PuzzleFactory
        {
            public static Puzzle CreatePuzzle(LevelBuilder builder, PuzzleAsset asset, int sectorIndex)
            {
                Puzzle puzzle = null;
                if(asset.GetType() == typeof(MultiStatePuzzleAsset))
                {
                    puzzle = new MultiStatePuzzle(builder, asset, sectorIndex);
                    
                }

                return puzzle;
            }
        }

        [System.Serializable]
        abstract class Puzzle
        {
            [SerializeField]
            public PuzzleAsset asset;

            //[SerializeField]
            public int sectorId;

            public Puzzle(LevelBuilder builder, PuzzleAsset asset, int sectorId)
            {
                this.asset = asset;
                this.sectorId = sectorId;
            }

            
        }

        class MultiStatePuzzle: Puzzle
        {
        
            public List<int> elementIds = new List<int>();

            public MultiStatePuzzle(LevelBuilder builder, PuzzleAsset asset, int sectorId): base(builder, asset, sectorId)
            {
                
                int elementCount = Random.Range(1, 4); // Should depend by the number of players


                for (int i = 0; i < elementCount; i++)
                {
                    // Create a new custom object to hold the handle ( or whatever it is )
                    CustomObject co = new CustomObject(builder, ((MultiStatePuzzleAsset)asset).ElementAsset);
                    builder.customObjects.Add(co);
                    // Add the new index in the internal list
                    elementIds.Add(builder.customObjects.Count - 1);

                    // Attach to a random tile
                    co.AttachRandomly(sectorId);


                }
            }

            
        }
    }

}
