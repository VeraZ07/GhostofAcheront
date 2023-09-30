using Fusion;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GOA.Level.LevelBuilder;

namespace GOA
{
    public class JigSawPuzzleController : PuzzleController
    {

        [System.Serializable]
        public struct NetworkFrameStruct : INetworkStruct
        {
            [UnitySerializeField] [Networked] [Capacity(16)] public NetworkLinkedList<int> TilesOrder { get; }

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

        bool busy = false;

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
                for(int j=0; j<puzzle.StartingOrder[i].Length; j++)
                {
                    fs.TilesOrder.Add(puzzle.StartingOrder[i][j]);
                }
                NetworkFrames.Add(fs);
            }
        }
        #endregion

        IEnumerator SwitchTiles(int frameId, int tileA, int tileB)
        {
            busy = true;
            yield return new WaitForSeconds(.5f);

            Vector3 posA = frames[frameId].Tiles[tileA].transform.localPosition;
            Vector3 posB = frames[frameId].Tiles[tileB].transform.localPosition;

            //frames[frameId].Tiles[tileA].transform.position = posB;
            //frames[frameId].Tiles[tileB].transform.position = posA;

            frames[frameId].Tiles[tileA].SwitchPosition(posB);
            frames[frameId].Tiles[tileB].SwitchPosition(posA);

            yield return new WaitForSeconds(.5f);
            busy = false;
            if (Runner.IsServer)
            {
                NetworkFrameStruct frame = NetworkFrames[frameId];
                frame.SelectedTiles.Clear();
                int orderTileA = frame.TilesOrder.IndexOf(tileA);
                int orderTileB = frame.TilesOrder.IndexOf(tileB);
                frame.TilesOrder.Set(orderTileA, tileB);
                frame.TilesOrder.Set(orderTileB, tileA);

                NetworkFrames.Set(frameId, frame);
            }
                
        }

        void CheckTilesOrder()
        {
            Debug.Log("Checking tiles");
            //foreach(NetworkFrameStruct netFrame in NetworkFrames)
            for (int i = 0; i < NetworkFrames.Count; i++)
            {
                //for(int i=0; i<netFrame.TilesOrder.Count; i++)
                //{
                //    if (netFrame.TilesOrder[i] != i)
                //        return;

                //}
                if (!IsFrameSolved(i))
                    return;
            }
            Debug.Log("Checking tiles: solved");
            Solved = true;
        }

        bool IsFrameSolved(int frameId)
        {

            for (int i = 0; i < NetworkFrames[frameId].TilesOrder.Count; i++)
            {
                if (NetworkFrames[frameId].TilesOrder[i] != i)
                    return false;

            }

            return true;
        }

        public bool TileIsSelectable(int frameId, int tileId)
        {
            return  !Solved && !busy &&
                    !IsFrameSolved(frameId) &&
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
                if (changed.Behaviour.NetworkFrames.Count == 0)
                    return;
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
                            changed.Behaviour.frames[i].Tiles[newFrame.SelectedTiles[1]].Select();
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

                                changed.Behaviour.CheckTilesOrder();
                            }
                        }
                    }
                }
            }

        }
    }

}
