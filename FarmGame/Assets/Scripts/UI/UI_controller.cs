using Game.Characters;
using Game.Characters.Data;
using Game.Characters.Shopper;
using Game.Controller;
using Game.Events;
using Game.Items;
using Game.Utils;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UI_controller : MonoBehaviour
    {
        public static UI_controller Instance;

        [Header("Data")]
        public Character_behaviour Player_character => GameObject.FindGameObjectWithTag("Player").GetComponent<Character_behaviour>();

        [Header("Settings")]
        public Texture2D CustomCursor;

        [Header("Components UI")]
        public Slider Slider_health;
        public TextMeshProUGUI Text_souls;
        
        [Space]
        public List<UI_Inventory_slot> UI_Inventory_slots = new List<UI_Inventory_slot>();

        [Space]
        public TextMeshProUGUI Text_hour;
        public TextMeshProUGUI Text_day;

        [Space]
        public GameObject Enemy_stats_UI;
        public Slider Slider_enemy_health;

        [Space]
        public GameObject Warning_panel_UI;
        public TextMeshProUGUI Text_warning;

        [Space]
        public GameObject Shop_UI;

        //Internal variables
        internal Vector2 shop_ui_target_position;
        internal GameObject current_enemy_target_obj;

        private void Awake()
        {
            Instance = this;

            Game_events.Player_character_took_damage.AddListener(Show_enemy_stats);
            Game_events.Player_character_killed_enemy.AddListener(Hide_enemy_stats);

            Game_events.Player_character_died.AddListener(Update_player_character_stats_UI);
            Game_events.Player_character_regen.AddListener(Update_player_character_stats_UI);

            Game_events.Player_character_collected_item.AddListener(Update_player_character_inventory_UI);
            Game_events.Player_character_changed_selected_item.AddListener(Update_player_character_inventory_UI);
            Game_events.Player_character_dropped_item.AddListener(Update_player_character_inventory_UI);
            Game_events.Player_character_used_item.AddListener(Update_player_character_inventory_UI);

            Game_events.Player_collected_souls.AddListener(Update_player_souls_panel_UI);
            Game_events.Player_lost_souls.AddListener(_ => Update_player_souls_panel_UI(changed_ammount: 0));

            Game_events.Player_character_opened_shop.AddListener(Show_shop_UI);
            Game_events.Player_character_closed_shop.AddListener(Hide_shop_UI);

            Game_events.Warning_panel_called.AddListener(Show_warning_panel_UI);

            Game_events.New_hour.AddListener(Update_game_UI);
            Game_events.New_day.AddListener(Update_game_UI);
            Game_events.New_season.AddListener(Update_game_UI);
        }

        private void Start()
        {
            // Cursor
            Cursor.SetCursor(CustomCursor, Vector2.zero, CursorMode.Auto);

            Update_game_UI();

            Update_player_character_stats_UI(health_regen_ammount: 0);
            Update_player_character_inventory_UI(itemIndex: 0, null);
            Update_player_souls_panel_UI(changed_ammount: 0);
        }

        private void Update()
        {
            //FOR NOWWWW !!!!!!!
            Move_shop_UI();
        }

        ///MAIN METHODS
        private void Update_game_UI()
        {
            // Text
            Text_day.text = $"{Game_controller.Instance.Day_of_season} / {Game_controller.Instance.Days_per_season}";
            Text_hour.text = $"{Game_controller.Instance.Day_hour:00}:00";
        
            // Color
            if(Game_controller.Instance.Current_day_stage == Game_controller.Day_stages.Day)
                Text_hour.color = Color.white;

            if (Game_controller.Instance.Current_day_stage == Game_controller.Day_stages.Night)
                Text_hour.color = Color.red;

            if (Game_controller.Instance.Current_day_stage == Game_controller.Day_stages.Rain || Game_controller.Instance.Current_day_stage == Game_controller.Day_stages.Storm)
                Text_hour.color = Color.yellow;
        }

        private void Move_shop_UI()
        {
            RectTransform rect = Shop_UI.GetComponent<RectTransform>();
            
            Vector2 offset = new Vector2(0f, 3f); // Offset to position the UI above the shopper
            Vector2 screenPos = Camera.main.WorldToScreenPoint(shop_ui_target_position + offset);

            float speed = 5f;

            rect.position = Vector2.Lerp(rect.position, screenPos, speed * Time.deltaTime);
        }
        private void Show_shop_UI(Shopper_controller shopper)
        {
            //Set position
            shop_ui_target_position = shopper.transform.position;

            //Effects
            Game_utils.Instance.Do_UI_fade_effect(Shop_UI);
            Game_utils.Instance.Do_UI_pop_effect(Shop_UI);
        }
        private void Hide_shop_UI()
        {
            //Effects
            Game_utils.Instance.Do_UI_fade_effect(Shop_UI);
        }

        private void Show_warning_panel_UI(string warning)
        {
            // Set
            Text_warning.text = warning;

            // Effects
            Game_utils.Instance.Do_UI_fade_effect(Warning_panel_UI);
            Game_utils.Instance.Do_UI_pop_effect(Warning_panel_UI);

            StartCoroutine(Warning_panel_timer(time: 3f));
        }
        private IEnumerator Warning_panel_timer(float time)
        {
            yield return new WaitForSeconds(time);

            // Effects
            Game_utils.Instance.Do_UI_fade_effect(Warning_panel_UI);
        }

        ///PLAYER CHARACTER UI METHODS
        private void Update_player_character_stats_UI ()
        {
            if(Player_character == null) { return; }

            //Update health bar
            Slider_health.value = Player_character.Health / Player_character.Max_health;
        }
        private void Update_player_character_stats_UI(float health_regen_ammount)
        {
            if (Player_character == null) { return; }

            //Update health bar
            Slider_health.value = Player_character.Health / Player_character.Max_health;
        }

        private void Update_player_character_inventory_UI()
        {
            // Go to
            Update_player_character_inventory_UI(0, null);
        }
        private void Update_player_character_inventory_UI(int itemIndex, Item_behaviour item)
        {
            if(Player_character == null) { return; }

            //Update inventory slots
            for(int i = 0; i < Player_character.InventorySize; i++)
            {
                GameObject item_obj = null;

                // Só pega o item se existir nesse índice
                if (i < Player_character.Inventory.Count)
                {
                    item_obj = Player_character.Inventory[i];
                }

                UI_Inventory_slots[i].Update_UI(item_obj);
            }

            //Effects
            Game_utils.Instance.Do_UI_pop_effect(UI_Inventory_slots[itemIndex].gameObject);
        }

        private void Update_player_souls_panel_UI(int changed_ammount)
        {
            if(Player_character == null) { return; }

            Text_souls.text = $"x {Player_data.Instance.Total_souls}";

            if (changed_ammount == 0) { return; }

            //Effects
            Game_utils.Instance.Do_UI_pop_effect(Text_souls.gameObject);
        }
    
        ///ENEMY STATS UI METHODS
        private void Show_enemy_stats(GameObject enemy_obj)
        {
            //Set
            current_enemy_target_obj = enemy_obj;

            Enemy_stats_UI.GetComponent<CanvasGroup>().alpha = 1f;

            StartCoroutine(Update_enemy_stats_UI());
            Update_player_character_stats_UI();
        }
        private IEnumerator Update_enemy_stats_UI()
        {
            while(current_enemy_target_obj != null)
            {
                yield return new WaitForSeconds(0.1f);

                if(current_enemy_target_obj == null) { break; }

                Character_behaviour character_behaviour = current_enemy_target_obj.GetComponent<Character_behaviour>();

                //Update health bar
                Slider_enemy_health.value = character_behaviour.Health / character_behaviour.Max_health;
            }
        }

        private void Hide_enemy_stats(GameObject enemy_obj)
        {
            if(enemy_obj == current_enemy_target_obj)
            {
                //Set
                Enemy_stats_UI.GetComponent<CanvasGroup>().alpha = 0f;
            }
        }
    }
}
