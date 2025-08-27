using Game.Events;
using Game.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class Recipe_book_UI_controller : MonoBehaviour
    {
        [Header("Components")]
        public GameObject Panel_recipe_book;
        public GameObject Page_holder;

        //Internal variables
        internal bool IsEnabled;
        internal List<GameObject> Recipes_slots_obj = new List<GameObject>();

        private void Awake()
        {
            Game_events.Player_clicked_on_recipe_book.AddListener(Show_recipe_panel_UI);
            Game_events.Close_Recipe_book.AddListener(Hide_recipe_panel_UI);
        }

        /// MAIN METHODS
        private void Show_recipe_panel_UI()
        {
            if (IsEnabled)
                return;

            IsEnabled = true;

            Put_recipes_on_screen();

            // Effects
            Game_utils.Instance.Do_UI_fade_effect(Panel_recipe_book);
        }

        private void Hide_recipe_panel_UI()
        {
            if(!IsEnabled)
                return;

            IsEnabled = false;

            Clean_book();

            // Effects
            Game_utils.Instance.Do_UI_fade_effect(Panel_recipe_book);
        }

        private void Put_recipes_on_screen()
        {
            foreach(Game_utils.Converted_recipe recipe in Game_utils.Instance.Recipes_table.ToArray())
            {
                GameObject recipe_slot_obj = Game_utils.Instance.Create_prefab_from_resources("Prefabs/UI/PRecipe_slot", Page_holder);

                // Set
                Recipes_slots_obj.Add(recipe_slot_obj);

                recipe_slot_obj.GetComponent<UI_recipe_slot>().Configure(recipe);
            }
        }

        private void Clean_book()
        {
            foreach(GameObject slot in Recipes_slots_obj)
            {
                Destroy(slot);
            }
        }
    }
}