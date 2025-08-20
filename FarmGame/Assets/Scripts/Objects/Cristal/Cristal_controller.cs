using Game.Characters.Data;
using Game.Events;
using Game.Items;
using Game.Utils;
using Game.Utils.Misc;
using UnityEngine;

namespace Game.Objects.Cristal
{
    public class Cristal_controller : MonoBehaviour
    {
        [Header("Components")]
        public Detector_manager Detector;

        //Internal variables
        internal GameObject Player_character_obj => GameObject.FindGameObjectWithTag("Player");

        internal void Trade(GameObject item_obj)
        {
            if(item_obj == null) 
                return;

            Item_behaviour item_bhv = item_obj.GetComponent<Item_behaviour>();

            if (!item_bhv.ItemData.Can_trade) 
                return;
            
            int souls_trade_value = item_bhv.ItemData.Price;

            // Set
            Player_data.Instance.Add_souls(souls_trade_value);

            item_bhv.Delete_item();

            // Events
            Game_events.Player_character_used_cristal_to_trade.Invoke();

            // Audio
            Game_utils.Instance.Create_sound("Cristal_trade", "Audios/Objects/Cristal_trade_1", transform.position);
        }
    }
}