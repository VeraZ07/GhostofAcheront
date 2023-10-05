
using Fusion;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class ArrowPuzzleController : PuzzleController
    {

        [System.Serializable]
        public struct NetworkFrameStruct : INetworkStruct
        {
            [UnitySerializeField] [Networked] [Capacity(16)] public NetworkLinkedList<int> SolvedTiles { get; }

            [UnitySerializeField] [Networked] public int SelectedTile { get; set; }

        }


        [UnitySerializeField] [Networked(OnChanged = nameof(OnNetworkFramesChanged))] [Capacity(5)] public NetworkLinkedList<NetworkFrameStruct> NetworkFrames { get; } = default;


        [System.Serializable]
        public class Frame
        {
            [SerializeField]
            List<ArrowTileInteractor> tiles = new List<ArrowTileInteractor>();
            public IList<ArrowTileInteractor> Tiles
            {
                get { return tiles.AsReadOnly(); }
            }

            List<int> directions = new List<int>();
            public IList<int> Directions
            {
                get { return directions.AsReadOnly(); }
            }

            public Frame(List<ArrowTileInteractor> tiles)
            {
                this.tiles = tiles;
            }

            public void AddDirection(int direction)
            {
                directions.Add(direction);
            }
        }

        List<Frame> frames = new List<Frame>();
       
        public override void Spawned()
        {
            base.Spawned();

            // Get all the tiles
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            LevelBuilder.ArrowPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as LevelBuilder.ArrowPuzzle;

            for (int i = 0; i < puzzle.FrameIds.Count; i++)
            {
                GameObject so = builder.CustomObjects[puzzle.FrameIds[i]].SceneObject;

                List<ArrowTileInteractor> tiles = new List<ArrowTileInteractor>(so.GetComponentsInChildren<ArrowTileInteractor>());
                
                frames.Add(new Frame(tiles));


                // Init tiles
                for (int j = 0; j < tiles.Count; j++)
                {
                    tiles[j].Init(this, i, j);
                    frames[i].AddDirection(puzzle.GetDirection(i, j));
                    if (NetworkFrames[i].SolvedTiles.Contains(j)/* || NetworkFrames[i].SelectedTile == j*/)
                        frames[i].Tiles[j].Hide();

                }

                // If there is more than one tile selected we need to check if they can pair with each other
                if (Runner.IsServer || Runner.IsSharedModeMasterClient)
                {
                    // Check for selection
                    

                   // Check if the tile can be set free
                }
            }
        }

        public override void Initialize(int puzzleIndex)
        {
            base.Initialize(puzzleIndex);

            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            LevelBuilder.ArrowPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as LevelBuilder.ArrowPuzzle;
            int tilesCount = puzzle.TileCount;
            for (int i = 0; i < puzzle.FrameIds.Count; i++)
            {
                NetworkFrameStruct fs = new NetworkFrameStruct();
                fs.SelectedTile = -1;
                NetworkFrames.Add(fs);
            }



        }

        public bool TileIsSelectable(int frameId, int tileId)
        {
            if (NetworkFrames[frameId].SelectedTile == -1 && !NetworkFrames[frameId].SolvedTiles.Contains(tileId))
                return true;

            return false;
        }

        public void SelectTile(int frameId, int tileId)
        {
            if (!TileIsSelectable(frameId, tileId))
                return;

            NetworkFrameStruct f = NetworkFrames[frameId];
            f.SelectedTile = tileId;
            NetworkFrames.Set(frameId, f);
        }

        IEnumerator ServerCheckForSetTileFreeDelayed(int frameId, int tileId)
        {
            yield return new WaitForSeconds(.5f);

            int direction = GetTileDirection(frameId, tileId);

            int size = frames[frameId].Tiles.Count;
            int sizeSqrt = (int)Mathf.Sqrt(size);
            bool ret = true;
            switch (direction)
            {
                case 0:
                    for (int i = 0; i < size; i++)
                    {
                        if (tileId > i && tileId % sizeSqrt == i % sizeSqrt && !NetworkFrames[frameId].SolvedTiles.Contains(i))
                            ret = false;
                    }
                    break;
                case 2:
                    for (int i = 0; i < size; i++)
                    {
                        if (tileId < i && tileId % sizeSqrt == i % sizeSqrt && !NetworkFrames[frameId].SolvedTiles.Contains(i))
                            ret = false;
                    }
                    break;
                case 1:
                    for (int i = 0; i < size; i++)
                    {
                        if (tileId < i && tileId / sizeSqrt == i / sizeSqrt && !NetworkFrames[frameId].SolvedTiles.Contains(i))
                            ret = false;
                    }
                    break;
                case 3:

                    for (int i = 0; i < size; i++)
                    {
                        if (tileId > i && tileId / sizeSqrt == i / sizeSqrt && !NetworkFrames[frameId].SolvedTiles.Contains(i))
                            ret = false;
                    }
                    break;
            }

            NetworkFrameStruct f = NetworkFrames[frameId];


            if (ret)
                f.SolvedTiles.Add(tileId);

            f.SelectedTile = -1;
            NetworkFrames.Set(frameId, f);
        }

        void ServerCheckForSetTileFree(int frameId, int tileId)
        {
            if (!Runner.IsServer && !Runner.IsSharedModeMasterClient)
                return;

            StartCoroutine(ServerCheckForSetTileFreeDelayed(frameId, tileId));
        }

        public int GetTileDirection(int frameId, int tileId)
        {
            return frames[frameId].Directions[tileId];
        }

        void ServerCheckForPuzzleSolved()
        {
            if (!Runner.IsServer && !Runner.IsSharedModeMasterClient)
                return;

            for(int i=0; i<NetworkFrames.Count; i++)
            {
                if (NetworkFrames[i].SolvedTiles.Count < frames[i].Directions.Count)
                    return;
            }

            Solved = true;
        }

        public static void OnNetworkFramesChanged(Changed<ArrowPuzzleController> changed)
        {
            
            // Show and hide tiles
            int frameCount = changed.Behaviour.NetworkFrames.Count;
            for (int i = 0; i < frameCount; i++)
            {
                changed.LoadOld();
                if (changed.Behaviour.NetworkFrames.Count == 0)
                    return;
                var oldFrame = changed.Behaviour.NetworkFrames[i];
                changed.LoadNew();
                var newFrame = changed.Behaviour.NetworkFrames[i];
                
                if (newFrame.SelectedTile < 0)
                {
                    if(oldFrame.SelectedTile >= 0)
                    {
                        if(newFrame.SolvedTiles.Contains(oldFrame.SelectedTile))
                            changed.Behaviour.frames[i].Tiles[oldFrame.SelectedTile].Hide();
                        else
                            changed.Behaviour.frames[i].Tiles[oldFrame.SelectedTile].Unselect();
                    }
                }
                else
                {
                    changed.Behaviour.frames[i].Tiles[newFrame.SelectedTile].Select();
                    changed.Behaviour.ServerCheckForSetTileFree(i, newFrame.SelectedTile);
                }

                if(oldFrame.SolvedTiles.Count < newFrame.SolvedTiles.Count)
                {
                    //int tileId = newFrame.SolvedTiles[newFrame.SolvedTiles.Count - 1];
                    changed.Behaviour.ServerCheckForPuzzleSolved();
                }


            }



        }
    }

}

