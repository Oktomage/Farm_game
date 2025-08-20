using Game.Characters;
using Game.Items;
using Game.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Crafting
{
    public class Crafting_controller : MonoBehaviour
    {
        [System.Serializable]
        public class Recipe
        {
            public string Recipe_name;

            [Space]
            public List<string> Materials_resources_path = new List<string>();

            [Space]
            public string Craft_result_resources_path;
        }

        [Header("Table")]
        public List<Recipe> Recipes_table;

        [Header("State")]
        public List<GameObject> Current_materials_inside = new List<GameObject>();

        private void Start()
        {
            // Save to JSON
            //Game_utils.Instance.Export_to_Json(Recipes_table, folder_path: "Resources/JSON/Recipes", file_name: "Recipes_table");
        }

        internal void Put_item(GameObject item_obj, Character_behaviour character)
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
        }

        internal void Craft()
        {

        }
    }
}
