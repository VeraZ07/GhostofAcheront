using Fusion;
using GOA.Interfaces;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GOA
{
    public class MemoryPuzzleController : PuzzleController
    {
        [System.Serializable]
        public struct NetworkFrameStruct : INetworkStruct
        {
            [UnitySerializeField] [Networked] [Capacity(16)] public NetworkLinkedList<int> SolvedTiles { get; }

            [UnitySerializeField] [Networked] [Capacity(maxSelectables)] public NetworkLinkedList<int> SelectedTiles { get; }

        }


        [UnitySerializeField][Networked(OnChanged = nameof(OnNetworkFramesChanged))] [Capacity(5)] public NetworkLinkedList<NetworkFrameStruct> NetworkFrames { get; } = default;


        [System.Serializable]
        public class Frame
        {
            [SerializeField]
            List<MemoryTileInteractor> tiles = new List<MemoryTileInteractor>();
            public IList<MemoryTileInteractor> Tiles
            {
                get { return tiles.AsReadOnly(); }
            }

       

            public Frame(List<MemoryTileInteractor> tiles)
            {
                this.tiles = tiles;
            
            }

        }

        [SerializeField]
        List<Frame> frames = new List<Frame>();

        const int maxSelectables = 2;

            

        #region fusion initialization
        public override void Spawned()
        {
            base.Spawned();

            // Get all the tiles
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            LevelBuilder.MemoryPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as LevelBuilder.MemoryPuzzle;

            for(int i=0; i<puzzle.FrameIds.Count; i++)
            {
                GameObject so = builder.CustomObjects[puzzle.FrameIds[i]].SceneObject;

                List<MemoryTileInteractor> tiles = new List<MemoryTileInteractor>(so.GetComponentsInChildren<MemoryTileInteractor>());
                frames.Add(new Frame(tiles));

                // Init tiles
                for(int j=0; j<tiles.Count; j++)
                {
                    tiles[j].Init(this, i, j);
                    if (NetworkFrames[i].SolvedTiles.Contains(j) || NetworkFrames[i].SelectedTiles.Contains(j))
                        frames[i].Tiles[j].Select();
                    else
                        frames[i].Tiles[j].Unselect();
                }

                // If there is more than one tile selected we need to check if they can pair with each other
                if (Runner.IsServer || Runner.IsSharedModeMasterClient)
                {
                    if(NetworkFrames[i].SelectedTiles.Count == maxSelectables)
                    {
                        int tile1 = NetworkFrames[i].SelectedTiles[0];
                        int tile2 = NetworkFrames[i].SelectedTiles[1];
                        if (TilesCanPairEachOther(i, tile1, tile2))
                        {
                            var copy = NetworkFrames[i];
                            copy.SelectedTiles.Clear();
                            copy.SolvedTiles.Add(tile1);
                            copy.SolvedTiles.Add(tile2);
                            NetworkFrames.Set(i, copy);
                        }
                    }
                }
            }

      
        }

        public override void Initialize(int puzzleIndex)
        {
            base.Initialize(puzzleIndex);

            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            LevelBuilder.MemoryPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as LevelBuilder.MemoryPuzzle;
            int tilesCount = puzzle.TilesCount;
            for (int i = 0; i < puzzle.FrameIds.Count; i++)
            {
                NetworkFrameStruct fs = new NetworkFrameStruct();
                NetworkFrames.Add(fs);
            }

            
            
        }
        #endregion

        #region private methods
        IEnumerator CheckTilesDelayed(int frameId, int tile1Id, int tile2Id)
        {
            yield return new WaitForSeconds(.5f);

            var newFrame = NetworkFrames[frameId];
            newFrame.SelectedTiles.Clear();
        
            if (TilesCanPairEachOther(frameId, tile1Id, tile2Id))
            {
                newFrame.SolvedTiles.Add(tile1Id);
                newFrame.SolvedTiles.Add(tile2Id);

            }

            NetworkFrames.Set(frameId, newFrame);

            
        }

        void ServerCheckForPuzzleSolved()
        {
            if (!Runner.IsServer && !Runner.IsSharedModeMasterClient)
                return;

            // Check if the puzzle has been solved
            for (int i = 0; i < frames.Count; i++)
            {
                if (NetworkFrames[i].SolvedTiles.Count < frames[i].Tiles.Count)
                {
                    return;
                }
            }

            
            Solved = true;
        }

        bool TilesCanPairEachOther(int frameId, int tile1, int tile2)
        {
            GameObject tile1Obj = frames[frameId].Tiles[tile1].gameObject;
            GameObject tile2Obj = frames[frameId].Tiles[tile2].gameObject;

            if (string.Equals(tile1Obj.name.ToLower(), tile2Obj.name.ToLower()))
                return true;
            else
                return false;
        }
        #endregion

        public bool TileIsSelectable(int frameId, int tileId)
        {
            return !NetworkFrames[frameId].SolvedTiles.Contains(tileId) &&
                    NetworkFrames[frameId].SelectedTiles.Count < maxSelectables && 
                    !NetworkFrames[frameId].SelectedTiles.Contains(tileId);
        }

        public void SelectTile(int frameId, int tileId)
        {
            if (!TileIsSelectable(frameId, tileId))
                return;

            var copy = NetworkFrames[frameId];
            copy.SelectedTiles.Add(tileId);
            NetworkFrames.Set(frameId, copy);

         
        }

       

        public void UnselectTile(int frameId, int tileId)
        {
            if (!NetworkFrames[frameId].SelectedTiles.Contains(tileId))
                return;

        
            var copy = NetworkFrames[frameId];
            copy.SelectedTiles.Remove(tileId);
            NetworkFrames.Set(frameId, copy);
        }



        


        #region fusion callbacks

       

        public static void OnNetworkFramesChanged(Changed<MemoryPuzzleController> changed)
        {
     

            // Show and hide tiles
            int frameCount = changed.Behaviour.NetworkFrames.Count;
            for (int i=0; i<frameCount; i++)
            {
                changed.LoadOld();
                if (changed.Behaviour.NetworkFrames.Count == 0)
                    return;
                var oldFrame = changed.Behaviour.NetworkFrames[i];
                changed.LoadNew();
                var newFrame = changed.Behaviour.NetworkFrames[i];

                if (oldFrame.SelectedTiles.Count != newFrame.SelectedTiles.Count)
                {
                    // Something changed in the selection
                    if(newFrame.SelectedTiles.Count == 1)
                    {
                        int tileId = newFrame.SelectedTiles[0];
                        changed.Behaviour.frames[i].Tiles[tileId].Select();
                    }
                    else
                    {
                        if (newFrame.SelectedTiles.Count == 2)
                        {
                            int tile1 = newFrame.SelectedTiles[0];
                            int tile2 = newFrame.SelectedTiles[1];
                            changed.Behaviour.frames[i].Tiles[tile2].Select();
                           
                            changed.Behaviour.StartCoroutine(changed.Behaviour.CheckTilesDelayed(i, tile1, tile2));
                        }
                        else // Is zero
                        {
                            // If the two tiles are in the solved tile list then leave them visible, otherwise hide them
                            int tile1 = oldFrame.SelectedTiles[0];
                            int tile2 = oldFrame.SelectedTiles[1];
                            if(!newFrame.SolvedTiles.Contains(tile1)) // At least one
                            {
                                changed.Behaviour.frames[i].Tiles[tile1].Unselect();
                                changed.Behaviour.frames[i].Tiles[tile2].Unselect();
                            }
                            else
                            {
                                changed.Behaviour.frames[i].Tiles[tile1].Show();
                                changed.Behaviour.frames[i].Tiles[tile2].Show();

                                // Check if the puzzle has been solved
                                //if(changed.Behaviour.Runner.IsServer)
                                changed.Behaviour.ServerCheckForPuzzleSolved();
                            }
                        }
                    }
                }

               

            }
            
            

        }
        #endregion
    }
}


