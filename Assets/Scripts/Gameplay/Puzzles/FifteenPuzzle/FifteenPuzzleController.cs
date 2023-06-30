using Fusion;
using GOA.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA
{
    public class FifteenPuzzleController : PuzzleController
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
            List<FifteenTileInteractor> tiles = new List<FifteenTileInteractor>();
            public IList<FifteenTileInteractor> Tiles
            {
                get { return tiles; }
            }

            public Frame(List<FifteenTileInteractor> tiles)
            {
                this.tiles = tiles;
            }
        }

        List<Frame> frames = new List<Frame>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void Spawned()
        {
            base.Spawned();

            // Get all the tiles
            LevelBuilder builder = FindObjectOfType<LevelBuilder>();
            LevelBuilder.FifteenPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as LevelBuilder.FifteenPuzzle;

            for (int i = 0; i < puzzle.FrameIds.Count; i++)
            {
                // Initialization
                GameObject so = builder.CustomObjects[puzzle.FrameIds[i]].SceneObject;

                List<FifteenTileInteractor> tiles = new List<FifteenTileInteractor>(so.GetComponentsInChildren<FifteenTileInteractor>());
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
            LevelBuilder.FifteenPuzzle puzzle = builder.GetPuzzle(PuzzleIndex) as LevelBuilder.FifteenPuzzle;
            //int tileCount = puzzle.TileCount;
            for (int i = 0; i < puzzle.FrameIds.Count; i++)
            {
                NetworkFrameStruct fs = new NetworkFrameStruct();
                for (int j = 0; j < puzzle.StartingOrder[i].Length; j++)
                {
                    fs.TilesOrder.Add(puzzle.StartingOrder[i][j]);
                }
                NetworkFrames.Add(fs);
            }
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
            return !Solved &&
                    !IsFrameSolved(frameId) &&
                    NetworkFrames[frameId].SelectedTiles.Count < maxSelectables &&
                    !NetworkFrames[frameId].SelectedTiles.Contains(tileId);
        }

        public static void OnNetworkFramesChanged(Changed<FifteenPuzzleController> changed)
        {

        }
    }

}
