using Game.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UI_Inventory_slot : MonoBehaviour
    {
        [Header("Components")]
        public Image Icon;

        //Internal variables
        [SerializeField] internal GameObject Current_item;

        public void Update_UI(GameObject item)
        {
            //Set
            Current_item = item;

            if (item == null) 
            { 
                Icon.enabled = false; 
                Icon.sprite = null; 
                return; 
            }

            Icon.enabled = true;

            //Image
            Icon.sprite = item.GetComponent<Item_behaviour>().ItemData.Icon;
        }
    }
}