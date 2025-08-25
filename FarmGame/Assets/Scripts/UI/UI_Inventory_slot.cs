using Game.Items;
using Game.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UI_Inventory_slot : MonoBehaviour
    {
        [Header("Components")]
        public Image Icon;
        public Image Hand_icon;

        //Internal variables
        internal GameObject Current_item;

        internal void Update_UI(GameObject item, bool selected)
        {
            // Set
            Current_item = item;

            Hand_icon.enabled = selected;

            if (item == null) 
            { 
                Icon.enabled = false; 
                Icon.sprite = null;

                Hand_icon.enabled = false;
                return; 
            }

            // Set
            Icon.enabled = true;

            // Image
            Icon.sprite = item.GetComponent<Item_behaviour>().ItemData.Icon;

            
            if(selected)
            {
                // Effects
                Game_utils.Instance.Do_UI_pop_effect(this.gameObject);
            }
        }
    }
}