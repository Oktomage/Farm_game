using UnityEngine;
using NaughtyAttributes;
using Game.Crops;
using System.Collections.Generic;
using Game.Items.Tools;

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

        [Space]
        public bool Can_trade = false;
        public bool Can_smelt = false;

        [ShowIf("Can_smelt")]
        public Item_scriptable Smelt_result;
        [ShowIf("Can_smelt")]
        [Range(0f, 10f)]
        public float Smelt_time;

        [Header("Item Properties")]
        public ItemType Type = ItemType.Miscellaneous;

        [ShowIf("IsTool")]
        public Tool_behaviour.ToolType ToolType = Tool_behaviour.ToolType.Axe;

        [ShowIf("IsSeed")]
        public Crop_scriptable Crop = null;

        [ShowIf("IsBow")]
        public Sprite Arrow_sprite;

        [ShowIf("IsStaff")]
        public Sprite Magic_sprite;

        private bool IsConsumable() => Type == ItemType.Consumable;
        private bool IsEquipment() => Type == ItemType.Equipment;
        private bool IsMaterial() => Type == ItemType.Material;
        private bool IsSeed() => Type == ItemType.Seed;
        private bool IsTool() => Type == ItemType.Tool;
        private bool IsBow() => Type == ItemType.Tool && ToolType == Tool_behaviour.ToolType.Bow;
        private bool IsStaff() => Type == ItemType.Tool && ToolType == Tool_behaviour.ToolType.Staff;
    }
}
