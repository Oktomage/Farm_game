using UnityEngine;
using NaughtyAttributes;
using Game.Crops;
using System.Collections.Generic;
using UnityEditor;
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

        [Header("Item Properties")]
        public ItemType Type = ItemType.Miscellaneous;

        [ShowIf("IsTool")]
        public Tool_behaviour.ToolType ToolType = Tool_behaviour.ToolType.Axe;

        [ShowIf("IsSeed")]
        public Crop_scriptable Crop = null;

        [Space]
        public bool Have_other_behaviours = false;
        [ShowIf("Have_other_behaviours")]
        public List<MonoScript> Other_behaviours = new List<MonoScript>();

        private bool IsConsumable() => Type == ItemType.Consumable;
        private bool IsEquipment() => Type == ItemType.Equipment;
        private bool IsMaterial() => Type == ItemType.Material;
        private bool IsSeed() => Type == ItemType.Seed;
        private bool IsTool() => Type == ItemType.Tool;
    }
}
