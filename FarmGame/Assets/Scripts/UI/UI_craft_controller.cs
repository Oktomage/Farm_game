using Game.Crafting;
using Game.Events;
using Game.Items;
using Game.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Craft
{
    public class UI_craft_controller : MonoBehaviour
    {
        [Header("Components")]
        public GameObject Panel_crafting;
        public GameObject Panel_target;

        // Internal variables
        internal bool IsVisible = false;

        internal GameObject Workbench_obj;
        internal Workbench_controller Workbench_ctrl => Workbench_obj.GetComponent<Workbench_controller>();
        internal List<GameObject> Images_objs = new List<GameObject>();

        private void Awake()
        {
            Game_events.Player_character_used_workbench.AddListener(Set_workbench);
            Game_events.Player_character_nearby_workbench.AddListener(Set_workbench);

            Game_events.Player_character_closed_workbench.AddListener(Disable_UI);
            Game_events.Player_character_crafted_item.AddListener(Disable_UI);
        }

        private void Update()
        {
            if(Workbench_obj != null)
                Move_UI();
        }

        /// CORE METHODS
        private void Set_workbench(GameObject workbench)
        {
            // Set
            if (workbench != Workbench_obj)
                Workbench_obj = workbench;

            if(!IsVisible)
                Enable_UI();

            Put_items_to_craft_UI();
        }

        /// MAIN METHODS
        private void UI_effects()
        {
            // Effects
            Game_utils.Instance.Do_UI_pop_effect(Panel_crafting);
            Game_utils.Instance.Switch_visibility_UI_panel(Panel_crafting);
        }

        private void Enable_UI()
        {
            if (IsVisible)
                return;

            // Set
            IsVisible = true;

            UI_effects();
        }
        private void Disable_UI()
        {
            if (!IsVisible)
                return;

            // Set
            IsVisible = false;

            UI_effects();
        }

        private void Move_UI()
        {
            RectTransform rect = Panel_crafting.GetComponent<RectTransform>();

            Vector2 offset = new Vector2(0f, 0f);
            Vector2 screenPos = Camera.main.WorldToScreenPoint(new Vector2(Workbench_obj.transform.position.x, Workbench_obj.transform.position.y) + offset);

            float speed = 10f;

            // Set
            rect.position = Vector2.Lerp(rect.position, screenPos, speed * Time.deltaTime);
        }

        private void Put_items_to_craft_UI()
        {
            // Clear
            if(Images_objs.Count > 0)
            {
                foreach(GameObject obj in Images_objs)
                {
                    Destroy(obj);
                }
            }

            // Create
            foreach(GameObject material in Workbench_ctrl.Current_materials_inside)
            {
                GameObject image_obj = Game_utils.Instance.Create_gameObject(Panel_target);
                Images_objs.Add(image_obj);
                image_obj.AddComponent<Image>();

                Item_behaviour item_bhv = material.GetComponent<Item_behaviour>();
                
                // Set
                image_obj.GetComponent<Image>().sprite = item_bhv.ItemData.Icon;
            }
        }
    }
}