using Fusion;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class JigSawPuzzleController : PuzzleController
    {

        [System.Serializable]
        public struct NetworkFrameStruct : INetworkStruct
        {
            [UnitySerializeField] [Networked] [Capacity(16)] public NetworkLinkedList<int> TilePositions { get; }

            [UnitySerializeField] [Networked] [Capacity(maxSelectables)] public NetworkLinkedList<int> SelectedTiles { get; }

        }


        [UnitySerializeField] [Networked(OnChanged = nameof(OnNetworkFramesChanged))] [Capacity(5)] public NetworkLinkedList<NetworkFrameStruct> NetworkFrames { get; } = default;


        const int maxSelectables = 2;

        [System.Serializable]
        public class Frame
        {
            List<JigSawTileInteractor> tiles = new List<JigSawTileInteractor>();
            public IList<JigSawTileInteractor> Tiles
            {
                get { return tiles; }
            }

            public Frame(List<JigSawTileInteractor> tiles)
            {
                this.tiles = tiles;
            }
        }

        List<Frame> frames = new List<Frame>();

        #region fusion initialization
        public override void Spawned()
        {
            base.Spawned();

            // Get all the tiles
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            LevelBuilder.JigSawPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as LevelBuilder.JigSawPuzzle;

            for (int i = 0; i < puzzle.FrameIds.Count; i++)
            {
                // Initialization
                GameObject so = builder.CustomObjects[puzzle.FrameIds[i]].SceneObject;

                List<JigSawTileInteractor> tiles = new List<JigSawTileInteractor>(so.GetComponentsInChildren<JigSawTileInteractor>());
                frames.Add(new Frame(tiles));

                // Init tiles
                for (int j = 0; j < tiles.Count; j++)
                {
                    tiles[j].Init(this, i, j);
                    if (NetworkFrames[i].SelectedTiles.Contains(j))
                        frames[i].Tiles[j].Select();
                    else
                        frames[i].Tiles[j].Unselect();
                }

                // If there is more than one tile selected we need to check if they can pair with each other
                if (Runner.IsServer)
                {
                    if (NetworkFrames[i].SelectedTiles.Count == maxSelectables)
                    {
                        int tile1 = NetworkFrames[i].SelectedTiles[0];
                        int tile2 = NetworkFrames[i].SelectedTiles[1];
                        //if (TilesCanPairEachOther(i, tile1, tile2))
                        //{
                        //    var copy = NetworkFrames[i];
                        //    copy.SelectedTiles.Clear();
                        //    copy.SolvedTiles.Add(tile1);
                        //    copy.SolvedTiles.Add(tile2);
                        //    NetworkFrames.Set(i, copy);
                        //}
                    }
                }
            }


        }

        public override void Initialize(int puzzleIndex)
        {
            base.Initialize(puzzleIndex);

            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            LevelBuilder.JigSawPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as LevelBuilder.JigSawPuzzle;
            int tilesCount = puzzle.TilesCount;
            for (int i = 0; i < puzzle.FrameIds.Count; i++)
            {
                NetworkFrameStruct fs = new NetworkFrameStruct();
                NetworkFrames.Add(fs);
            }
        }
        #endregion

        IEnumerator SwitchTiles(int frameId, int tileA, int tileB)
        {
            Vector3 posA = frames[frameId].Tiles[tileA].transform.position;
            Vector3 posB = frames[frameId].Tiles[tileB].transform.position;

            frames[frameId].Tiles[tileA].transform.position = posB;
            frames[frameId].Tiles[tileB].transform.position = posA;

            yield return new WaitForSeconds(1f);

            if (Runner.IsServer)
            {
                NetworkFrameStruct frame = NetworkFrames[frameId];
                frame.SelectedTiles.Clear();
                NetworkFrames.Set(frameId, frame);
            }
                
        }

        public bool TileIsSelectable(int frameId, int tileId)
        {
            return  !Solved && 
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

        public static void OnNetworkFramesChanged(Changed<JigSawPuzzleController> changed)
        {
            Debug.Log("networkframes changed");

            int frameCount = changed.Behaviour.NetworkFrames.Count;
            for (int i = 0; i < frameCount; i++)
            {
                changed.LoadOld();
                var oldFrame = changed.Behaviour.NetworkFrames[i];
                changed.LoadNew();
                var newFrame = changed.Behaviour.NetworkFrames[i];

                if(oldFrame.SelectedTiles.Count != newFrame.SelectedTiles.Count)
                {
                    // Selection changed
                    if(newFrame.SelectedTiles.Count == 1) // Just one tile has been selected
                    {
                        // Select the tile
                        int tileId = newFrame.SelectedTiles[0];
                        changed.Behaviour.frames[i].Tiles[tileId].Select();
                    }
                    else
                    {
                        if (newFrame.SelectedTiles.Count == 2) // Two tiles selected, we need to check move them
                        {
                            changed.Behaviour.StartCoroutine(changed.Behaviour.SwitchTiles(i, newFrame.SelectedTiles[0], newFrame.SelectedTiles[1]));
                        }
                        else
                        {
                            if (newFrame.SelectedTiles.Count == 0) // Unselect tiles
                            {
                                int tileA = oldFrame.SelectedTiles[0];
                                int tileB = oldFrame.SelectedTiles[1];
                                changed.Behaviour.frames[i].Tiles[tileA].Unselect();
                                changed.Behaviour.frames[i].Tiles[tileB].Unselect();
                            }
                        }
                    }
                }
            }

            // Show and hide tiles
            //int frameCount = changed.Behaviour.NetworkFrames.Count;
            //for (int i = 0; i < frameCount; i++)
            //{
            //    changed.LoadOld();
            //    var oldFrame = changed.Behaviour.NetworkFrames[i];
            //    changed.LoadNew();
            //    var newFrame = changed.Behaviour.NetworkFrames[i];

            //    if (oldFrame.SelectedTiles.Count != newFrame.SelectedTiles.Count)
            //    {
            //        // Something changed in the selection
            //        if (newFrame.SelectedTiles.Count == 1)
            //        {
            //            int tileId = newFrame.SelectedTiles[0];
            //            changed.Behaviour.frames[i].Tiles[tileId].Show();
            //        }
            //        else
            //        {
            //            if (newFrame.SelectedTiles.Count == 2)
            //            {
            //                int tile1 = newFrame.SelectedTiles[0];
            //                int tile2 = newFrame.SelectedTiles[1];
            //                changed.Behaviour.frames[i].Tiles[tile2].Show();
            //                //newFrame.SelectedTiles.Clear();
            //                //Debug.LogFormat("Check for tiles as pair - {0}, {1}", tile1, tile2);
            //                //if(changed.Behaviour.TilesCanPairEachOther(i, tile1, tile2))
            //                //{
            //                //    newFrame.SolvedTiles.Add(tile1);
            //                //    newFrame.SolvedTiles.Add(tile2);

            //                //}

            //                //changed.Behaviour.NetworkFrames.Set(i, newFrame);
            //                changed.Behaviour.StartCoroutine(changed.Behaviour.CheckTilesDelayed(i, tile1, tile2));
            //            }
            //            else // Is zero
            //            {
            //                // If the two tiles are in the solved tile list then leave them visible, otherwise hide them
            //                int tile1 = oldFrame.SelectedTiles[0];
            //                int tile2 = oldFrame.SelectedTiles[1];
            //                if (!newFrame.SolvedTiles.Contains(tile1)) // At least one
            //                {

            //                    changed.Behaviour.frames[i].Tiles[tile1].Hide();
            //                    changed.Behaviour.frames[i].Tiles[tile2].Hide();
            //                }
            //                else
            //                {
            //                    // Check if the puzzle has been solved
            //                    changed.Behaviour.CheckForPuzzleSolved();
            //                }
            //            }
            //        }
            //    }



            //}



        }
    }

}
