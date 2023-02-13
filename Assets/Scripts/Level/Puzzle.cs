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
        public abstract class Puzzle
        {
            [SerializeField]
            PuzzleAsset asset;
            public PuzzleAsset Asset
            {
                get { return asset; }
            }

            //[SerializeField]
            int sectorId;

            //public List<GameObject> sceneObjects = new List<GameObject>();

            //public PuzzleController puzzleController;

            protected LevelBuilder builder;

            public Puzzle(LevelBuilder builder, PuzzleAsset asset, int sectorId)
            {
                this.asset = asset;
                this.sectorId = sectorId;
                this.builder = builder;
            }

            public abstract void CreateSceneObjects();

            
        }

        public class MultiStatePuzzle: Puzzle
        {
        
            List<int> elementIds = new List<int>();
            public ICollection<int> ElementsIds
            {
                get { return elementIds.AsReadOnly(); }
            }

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
                    co.AttachRandomly(sectorId, false);


                }
            }

            public override void CreateSceneObjects()
            {
                // Loop through all the ids
                for(int i=0; i<elementIds.Count; i++)
                {
                    builder.customObjects[elementIds[i]].CreateSceneObject();
                }
            }
        }
    }

}
