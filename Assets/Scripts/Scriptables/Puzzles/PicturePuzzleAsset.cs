using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOA.Assets
{
    public class PicturePuzzleAsset : PuzzleAsset
    {
        
        [SerializeField]
        CustomObjectAsset picture;
        public CustomObjectAsset Picture
        {
            get { return picture; }
        }

        [SerializeField]
        List<CustomObjectAsset> pieces;
        public IList<CustomObjectAsset> Pieces
        {
            get { return pieces.AsReadOnly(); }
        }

        [SerializeField]
        List<ItemAsset> items;
        public IList<ItemAsset> Items
        {
            get { return items.AsReadOnly(); }
        }

        [SerializeField]
        NetworkObject pickerPrefab;
        public NetworkObject PickerPrefab
        {
            get { return pickerPrefab; }
        }
    }

}
