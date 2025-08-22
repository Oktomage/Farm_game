using Game.Characters;
using Game.Items.Tools;
using Game.Utils;
using UnityEngine;

namespace Game.Items
{
    public class Item_behaviour : MonoBehaviour
    {
        [Header("Data")]
        public Item_scriptable ItemData;
        public Character_behaviour Character;

        [Header("Components")]
        public SpriteRenderer Render;
        public SpriteRenderer Render_shadow;
        public Collider2D Collider;

        [Header("Settings")]
        public bool IsCollectable = true;

        private void Start()
        {
            if(ItemData != null)
                Configure_item();
        }

        /// CORE METHODS
        internal void Set_item_data(Item_scriptable data)
        {
            // Set
            ItemData = data;

            Configure_item();
        }

        private void Configure_item()
        {
            // Sprite
            Render.sprite = ItemData.Icon;
        
            // Add essential scripts to item_obj
            if(ItemData.Have_other_behaviours)
            {
                foreach (var ms in ItemData.Other_behaviours)
                {
                    if (ms == null) continue;
                    var t = ms.GetClass();

                    if (t != null && typeof(MonoBehaviour).IsAssignableFrom(t))
                    {
                        // evita duplicar
                        if (this.gameObject.GetComponent(t) == null)
                            this.gameObject.AddComponent(t);
                    }
                }
            }

            // Configure it
            if (this.gameObject.TryGetComponent<Tool_behaviour>(out Tool_behaviour tool))

            switch (ItemData.Type)
            {
                case Item_scriptable.ItemType.Tool:
                    tool.Set_toolType(ItemData.ToolType);
                    break;

                case Item_scriptable.ItemType.Seed:
                    tool.Set_toolType(Tool_behaviour.ToolType.Seeds);
                    break;
            }
        }

        /// MAIN METHODS
        internal void Delete_item()
        {
            if(Character == null)
                return;

            //Remove item from character inventory
            Character.Drop_selected_item_from_inventory();

            Destroy(this.gameObject);
        }

        internal void Set_collected_settings() => Set_collected_settings(null);
        internal void Set_collected_settings(Character_behaviour character)
        {
            // Set
            if(character != null)
                Character = character;

            IsCollectable = false;
            
            Collider.enabled = false;
            Render.enabled = false;

            if(Render_shadow != null)
                Render_shadow.enabled = false;

            // Audio
            Game_utils.Instance.Create_sound("Item_collect_sound", "Audios/Items/Grab_item_1", this.transform.position);
        }

        internal void Set_dropped_settings()
        {
            // Set
            Character = null;

            IsCollectable = true;
            
            Collider.enabled = true;
            Render.enabled = true;

            if (Render_shadow != null)
                Render_shadow.enabled = true;

            // Audio
            Game_utils.Instance.Create_sound("Item_collect_sound", "Audios/Items/Grab_item_1", this.transform.position);
        }
    }
}
