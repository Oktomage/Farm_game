using Game.Characters;
using Game.Events;
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
        public Slider Slider_enemy_health;
        public TextMeshProUGUI Text_enemy_name;
        public TextMeshProUGUI Text_enemy_bonuses;

        //Internal variables
        internal GameObject Current_enemy_target_obj;

        private void Awake()
        {
            Game_events.Player_character_took_damage.AddListener((dmg, enemy) => Show_enemy_stats(enemy));
            Game_events.Player_character_killed_enemy.AddListener(Hide_enemy_stats);
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

            Panel_enemy_stats.GetComponent<CanvasGroup>().alpha = 1f;

            StartCoroutine(Update_enemy_stats_UI());
        }

        private void Hide_enemy_stats(GameObject enemy_obj)
        {
            if (enemy_obj == Current_enemy_target_obj)
            {
                // Set
                Panel_enemy_stats.GetComponent<CanvasGroup>().alpha = 0f;
            }
        }

        private IEnumerator Update_enemy_stats_UI()
        {
            while (Current_enemy_target_obj != null)
            {
                yield return new WaitForSeconds(0.1f);

                if (Current_enemy_target_obj == null)
                    break;

                Character_behaviour character_behaviour = Current_enemy_target_obj.GetComponent<Character_behaviour>();

                // Update health bar
                Slider_enemy_health.value = character_behaviour.Health / character_behaviour.Max_health;
            }
        }
    }
}