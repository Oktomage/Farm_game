using Game.Items;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Crops
{
    [CreateAssetMenu(fileName = "New Crop", menuName = "Crops/Crop")]
    public class Crop_scriptable : ScriptableObject
    {
        public enum CropType
        {
            Vegetable,
            Fruit,
            Grain,
            Flower,
            Herb
        }

        [Header("Crop Settings")]
        public string CropName = "New Crop";
        public string Description = "Crop Description";
        public List<Sprite> Crop_stages = new List<Sprite>();

        [Space]
        public CropType Type = CropType.Vegetable;

        [Space]
        public Item_scriptable Crop_item;
    }
}