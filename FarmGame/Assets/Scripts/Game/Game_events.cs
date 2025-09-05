using Game.Characters;
using Game.Characters.Shopper;
using Game.Items;
using Game.Utils.Misc;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Events
{
    public class Game_events : MonoBehaviour
    {
        public static UnityEvent Camera_shake = new UnityEvent();

        public class ShaderEffectEvent : UnityEvent<string, Vector2> { }
        public static ShaderEffectEvent ShaderEffect = new ShaderEffectEvent();

        public static UnityEvent<Character_behaviour> Player_clicked_on_enemy = new UnityEvent<Character_behaviour>();
        public static UnityEvent Player_clicked_on_recipe_book = new UnityEvent();
        public static UnityEvent<int> Player_collected_souls = new UnityEvent<int>();
        public static UnityEvent<int> Player_lost_souls = new UnityEvent<int>();

        public static UnityEvent Close_Recipe_book = new UnityEvent();

        public static UnityEvent Player_character_collected_item = new UnityEvent();
        public class InventoryEvent : UnityEvent<int, Item_behaviour> { }
        public static InventoryEvent Player_character_changed_selected_item = new InventoryEvent();
        public static UnityEvent Player_character_used_item = new UnityEvent();
        public static UnityEvent Player_character_dropped_item = new UnityEvent();

        public class Player_character_took_damageEvent : UnityEvent<float, GameObject> { }
        public static Player_character_took_damageEvent Player_character_took_damage = new Player_character_took_damageEvent();

        public static UnityEvent<GameObject> Player_character_killed_enemy = new UnityEvent<GameObject>();
        public static UnityEvent Player_character_died = new UnityEvent();
        public static UnityEvent<float> Player_character_regen = new UnityEvent<float>();
        public static UnityEvent Player_character_used_cristal_to_trade = new UnityEvent();

        public static UnityEvent<Shopper_controller> Player_character_opened_shop = new UnityEvent<Shopper_controller>();
        public static UnityEvent Player_character_closed_shop = new UnityEvent();

        public static UnityEvent<GameObject> Player_character_nearby_workbench = new UnityEvent<GameObject>();
        public static UnityEvent<GameObject> Player_character_used_workbench = new UnityEvent<GameObject>();
        public static UnityEvent Player_character_closed_workbench = new UnityEvent();
        public static UnityEvent Player_character_crafted_item = new UnityEvent();

        public static UnityEvent Furnace_smelt_done = new UnityEvent();

        public static UnityEvent<Character_behaviour> Boss_battle_started = new UnityEvent<Character_behaviour>();
        public static UnityEvent<Character_behaviour> Boss_defeated = new UnityEvent<Character_behaviour>();

        public static UnityEvent<string> Warning_panel_called = new UnityEvent<string>();

        public class Enemy_took_damageEvent : UnityEvent<float, GameObject> { }
        public static Enemy_took_damageEvent Enemy_took_damage = new Enemy_took_damageEvent();

        public static UnityEvent Enemy_died = new UnityEvent();

        public class Attack_indicatorEvent : UnityEvent<Attack_indicator_controller.Indicator_info> { }
        public static Attack_indicatorEvent Attack_indicator = new Attack_indicatorEvent();

        public static UnityEvent New_day = new UnityEvent();
        public static UnityEvent New_hour = new UnityEvent();
        public static UnityEvent New_season = new UnityEvent();

        public static UnityEvent Day_stage_changed = new UnityEvent();

        public static UnityEvent Sunny_day_started = new UnityEvent();
        public static UnityEvent Rain_day_started = new UnityEvent();
        public static UnityEvent Storm_day_started = new UnityEvent();
        public static UnityEvent Boss_day_started = new UnityEvent();
    }
}