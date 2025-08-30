using Game.Characters;
using Game.Events;
using Game.Items;
using Game.Utils;
using Game.Utils.Misc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Crafting
{
    public class Workbench_controller : MonoBehaviour
    {
        [Header("State")]
        public List<GameObject> Current_materials_inside = new List<GameObject>();

        [Header("Components")]
        public Detector_manager Detector => this.gameObject.GetComponent<Detector_manager>();

        //Internal variables
        internal bool Crafting_UI_visible = false;
        internal GameObject Player_character_obj => GameObject.FindGameObjectWithTag("Player");

        private void Start()
        {
            StartCoroutine(Read_detector_state());
        }

        /// CORE METHODS
        private void Clear_workbench()
        {
            Current_materials_inside.Clear();
        }

        private void Fail_craft()
        {
            Crafting_UI_visible = false;

            // Audio
            Game_utils.Instance.Create_2d_sound("Fail_sound", "Audios/UI/Fail_1");

            //Events
            Game_events.Player_character_closed_workbench.Invoke();
        }

        /// MAIN METHODS
        internal void Put_item (GameObject item_obj, Character_behaviour character)
        {
            // Checks
            if (item_obj == null) 
                return;

            Item_behaviour item_bhv = item_obj.GetComponent<Item_behaviour>();

            if (!item_bhv.ItemData.Can_trade) 
                return;

            // Set
            character.Drop_selected_item_from_inventory();
        
            Current_materials_inside.Add(item_obj);
            item_bhv.Set_collected_settings();

            if (character.IsPlayer)
            {
                if(!Crafting_UI_visible)
                {
                    Crafting_UI_visible = true;
                }

                // Events
                Game_events.Player_character_used_workbench.Invoke(this.gameObject);
            }
        }

        internal void Craft (Character_behaviour character)
        {
            if(Current_materials_inside.Count == 0)
                return;

            // Get
            Item_scriptable craft_result = Game_utils.Instance.Get_recipe_result(Current_materials_inside);

            Clear_workbench();

            if (craft_result != null)
            {
                // Create
                GameObject craft_obj = Game_utils.Instance.Create_item(craft_result, this.gameObject.transform.position);

                // Events
                if (character.IsPlayer)
                    Game_events.Player_character_crafted_item.Invoke();

                // Audio
                Game_utils.Instance.Create_sound("Craft_sound", "Audios/Objects/Anvil_1", transform.position);

                // Effects
                Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Craft_sparks", transform.position);
            }
            else
            {
                Fail_craft();
            }
        }

        private IEnumerator Read_detector_state()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);

                if (Player_character_obj == null)
                    yield return null;

                // Out of range
                if (!Detector.Player_in_range)
                {
                    // Close
                    if (Crafting_UI_visible)
                    {
                        Crafting_UI_visible = false;

                        //Events
                        Game_events.Player_character_closed_workbench.Invoke();
                    }
                }
                // On range
                else
                {
                    // Open
                    if(!Crafting_UI_visible && Current_materials_inside.Count > 0)
                    {
                        Crafting_UI_visible = true;

                        //Events
                        Game_events.Player_character_nearby_workbench.Invoke(this.gameObject);
                    }
                }
            }
        }
    }
}
