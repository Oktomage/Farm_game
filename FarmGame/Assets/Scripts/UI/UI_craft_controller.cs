using Game.Crafting;
using Game.Items;
using Game.Utils;
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
        internal GameObject Craft_workbench_obj;
        internal Crafting_controller Crafting_ctrl => Craft_workbench_obj.GetComponent<Crafting_controller>();

        internal void Set_workbench(GameObject workbench)
        {
            Craft_workbench_obj = workbench;

            Put_items_to_craft_UI();
        }

        private void Put_items_to_craft_UI()
        {
            foreach(GameObject material in Crafting_ctrl.Current_materials_inside)
            {
                GameObject image_obj = Game_utils.Instance.Create_gameObject(Panel_target);
                image_obj.AddComponent<Image>();

                Item_behaviour item_bhv = material.GetComponent<Item_behaviour>();
                
                // Set
                image_obj.GetComponent<Image>().sprite = item_bhv.ItemData.Icon;
            }
        }
    }
}