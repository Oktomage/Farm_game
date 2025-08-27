using Game.Characters;
using Game.Events;
using Game.Utils;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UI_enemy_stats_controller : MonoBehaviour
    {
        [Header("Components")]
        public GameObject Panel_enemy_stats;

        [Space]
        public UI_filler Enemy_health_bar;
        public TextMeshProUGUI Text_enemy_name;
        public TextMeshProUGUI Text_enemy_bonuses;

        //Internal variables
        internal bool IsEnabled = false;
        internal GameObject Current_enemy_target_obj;

        private void Awake()
        {
            Game_events.Player_character_took_damage.AddListener((dmg, enemy) => Show_enemy_stats(enemy));
            Game_events.Player_character_killed_enemy.AddListener(Hide_enemy_stats);
            Game_events.Enemy_took_damage.AddListener((dmg, enemy) => Update_enemy_stats_UI());
        }

        /// CORE METHODS
        private void Set_bonuses_text(Character_behaviour character)
        {
            string txt = "";

            // Set
            Text_enemy_bonuses.enabled = false;

            if (character == null)
                return;

            if (character.IsExtraStrong)
                txt += "Extra strong, ";

            if (character.IsExtraFast)
                txt += "Extra fast, ";

            if (character.HaveMagicalResistance)
                txt += "Magical reistance, ";

            if (character.HavePhysicalResistance)
                txt += "Physical resistance";

            if (txt.Length == 0)
                return;

            // Set
            Text_enemy_bonuses.enabled = true;

            Text_enemy_bonuses.text = txt;
        }

        /// MAIN METHODS
        private void Show_enemy_stats(GameObject enemy_obj)
        {
            // Set
            Current_enemy_target_obj = enemy_obj;
            Current_enemy_target_obj.TryGetComponent<Character_behaviour>(out Character_behaviour character);

            if(character == null) 
                return;

            // Set
            Text_enemy_name.text = character.Name;
            Set_bonuses_text(character);

            Update_enemy_stats_UI();

            if (IsEnabled)
                return;

            IsEnabled = true;

            // Effects
            Panel_enemy_stats.GetComponent<CanvasGroup>().alpha = 1f;
        }

        private void Hide_enemy_stats(GameObject enemy_obj)
        {
            if (enemy_obj == Current_enemy_target_obj)
            {
                if (!IsEnabled)
                    return;

                IsEnabled = false;

                // Effects
                Panel_enemy_stats.GetComponent<CanvasGroup>().alpha = 0f;
            }
        }

        private void Update_enemy_stats_UI()
        {
            if (Current_enemy_target_obj == null)
                return;

            Character_behaviour character_behaviour = Current_enemy_target_obj.GetComponent<Character_behaviour>();

            // Update health bar
            float health = character_behaviour.Health / character_behaviour.Max_health;

            Enemy_health_bar.Set_value(health);
        }
    }
}