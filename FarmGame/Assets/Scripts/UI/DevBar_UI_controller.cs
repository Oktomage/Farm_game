using Game.Characters;
using Game.Items;
using Game.Utils;
using UnityEngine;

namespace Game.Dev
{
    public class DevBar_UI_controller : MonoBehaviour
    {
        //Internal variables
        private GameObject Player_character_obj => GameObject.FindGameObjectWithTag("Player");

        public void Force_item_creation(Item_scriptable item)
        {
            Game_utils.Instance.Create_item(item, new Vector2(Player_character_obj.transform.position.x, Player_character_obj.transform.position.y + 0.5f));
        }

        public void Force_enemy_creation(Character_scriptable enemy)
        {
            Game_utils.Instance.Create_enemy(enemy, new Vector2(Player_character_obj.transform.position.x, Player_character_obj.transform.position.y + 0.5f));
        }
    }
}