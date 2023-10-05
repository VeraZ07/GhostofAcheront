using DG.Tweening;
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

            // Just one, the other one is the black tile
            [UnitySerializeField] [Networked] public int SelectedTile { get; set; }

        }


        [UnitySerializeField] [Networked(OnChanged = nameof(OnNetworkFramesChanged))] [Capacity(5)] public NetworkLinkedList<NetworkFrameStruct> NetworkFrames { get; } = default;

        const int maxSelectables = 2;

        const int blackTileId = 4;

        const int size = 3;

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

        bool busy = false;

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
                fs.SelectedTile = -1;
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
            
            if (Solved || IsFrameSolved(frameId) || NetworkFrames[frameId].SelectedTile != -1 || busy)
                return false;

            //if(NetworkFrames[frameId].SelectedTile == -1)
            //{
                int blackOrderId = NetworkFrames[frameId].TilesOrder.IndexOf(blackTileId); // Black
                int whiteOrderId = NetworkFrames[frameId].TilesOrder.IndexOf(tileId); // White
    
                if (!(((whiteOrderId == blackOrderId - 1 || whiteOrderId == blackOrderId + 1) && whiteOrderId / size == blackOrderId / size) ||
                     ((whiteOrderId == blackOrderId - 3 || whiteOrderId == blackOrderId + 3) && whiteOrderId % size == blackOrderId % size)))
                    return false;
            //}

            

            return true;
        }

        public void SelectTile(int frameId, int tileId)
        {
            if (!TileIsSelectable(frameId, tileId))
                return;

            var copy = NetworkFrames[frameId];
            copy.SelectedTile = tileId;
            NetworkFrames.Set(frameId, copy);


        }

        /// <summary>
        /// Server only
        /// </summary>
        void ServerCheckPuzzle()
        {
            if (!Runner.IsServer && !Runner.IsSharedModeMasterClient)
                return;

            for(int i=0; i<frames.Count; i++)
            {
                if (!IsFrameSolved(i))
                    return;
            }

            Solved = true;
        }

        void SwitchTiles(int frameId, int whiteTileId)
        {
            busy = true;
            Frame frame = frames[frameId];

            float moveDist = 0.1f;
            float moveTime = .25f;

            Transform bT = frame.Tiles[blackTileId].transform;
            Transform wT = frame.Tiles[whiteTileId].transform;

            // Switch tiles
            Vector3 wPos = frame.Tiles[whiteTileId].transform.localPosition;
            Vector3 bPos = frame.Tiles[blackTileId].transform.localPosition;
            float bZ = bT.localPosition.z + moveDist;

            Sequence seq = DOTween.Sequence();
            seq.Append(bT.DOLocalMoveZ(bZ, moveTime, false));
            seq.Append(bT.DOLocalMove(new Vector3(wPos.x, wPos.y, bZ), moveTime));
            seq.Join(wT.DOLocalMove(new Vector3(bPos.x, bPos.y, wPos.z), moveTime));
            seq.Append(bT.DOLocalMoveZ(bT.localPosition.z, moveTime, false));
            
            seq.onComplete += () => 
            {
                busy = false;
                if(Runner.IsServer || Runner.IsSharedModeMasterClient)
                {
                    var nFrame = NetworkFrames[frameId];
                    nFrame.SelectedTile = -1;
                    int blackOrder = nFrame.TilesOrder.IndexOf(blackTileId);
                    int whiteOrder = nFrame.TilesOrder.IndexOf(whiteTileId);
                    nFrame.TilesOrder.Set(blackOrder, whiteTileId);
                    nFrame.TilesOrder.Set(whiteOrder, blackTileId);
                    NetworkFrames.Set(frameId, nFrame);
                }
                
            };

            seq.Play();

        }

        public static void OnNetworkFramesChanged(Changed<FifteenPuzzleController> changed)
        {
            int frameCount = changed.Behaviour.NetworkFrames.Count;
            for (int i = 0; i < frameCount; i++)
            {
                changed.LoadOld();
                if (changed.Behaviour.NetworkFrames.Count == 0)
                    return;
                var oldFrame = changed.Behaviour.NetworkFrames[i];
                changed.LoadNew();
                var newFrame = changed.Behaviour.NetworkFrames[i];

                if(oldFrame.SelectedTile != newFrame.SelectedTile)
                {
                    if(newFrame.SelectedTile < 0) // Switch completed, lets check the puzzle
                    {
                        changed.Behaviour.ServerCheckPuzzle();
                    }
                    else // Switch tiles
                    {
                        changed.Behaviour.SwitchTiles(i, newFrame.SelectedTile);
                    }
                }

              
            }
        }
    }

}
