using UnityEngine;
using NaughtyAttributes;
using Game.Crops;

namespace Game.Items
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Items/Item")]
    public class Item_scriptable : ScriptableObject
    {
        public enum ItemType
        {
            Consumable,
            Equipment,
            Material,
            Seed,
            Tool,
            Miscellaneous
        }

        [Header("Item Settings")]
        public string ItemName = "New Item";
        public string Description = "Item Description";
        public Sprite Icon;
        public int Price = 1;

        [Header("Item Properties")]
        public ItemType Type = ItemType.Miscellaneous;

        [ShowIf("IsSeed")]
        public Crop_scriptable Crop = null;

        private bool IsConsumable() => Type == ItemType.Consumable;
        private bool IsEquipment() => Type == ItemType.Equipment;
        private bool IsMaterial() => Type == ItemType.Material;
        private bool IsSeed() => Type == ItemType.Seed;
        private bool IsTool() => Type == ItemType.Tool;
    }
}
