using Game.Effects;
using Game.Events;
using Game.Magic;
using Game.Utils;
using Game.Utils.Misc;
using System.Collections;
using UnityEngine;

namespace Game.Characters.Enemies
{
    public class Enemy_controller : MonoBehaviour
    {
        [Header("Data")]
        public Character_scriptable characterData;

        [Header("Components")]
        public Character_behaviour Character => gameObject.GetComponent<Character_behaviour>();

        [Header("Mind")]
        public GameObject Target_obj;

        //Internal variables
        internal GameObject Player_character => GameObject.FindGameObjectWithTag("Player");

        private void Start()
        {
            if(characterData != null)
                Configure();
        }

        /// CORE METHODS
        private void Configure()
        {
            // Set
            Character.Configure(characterData);
            Character.IsEnemy = true;

            // Boss
            if (characterData.IsBoss)
            {
                Start_boss_battle();
            }

            // Render
            Character.Render.sprite = characterData.Icon;

            StopAllCoroutines();
            StartCoroutine(Brain_controller());

            Set_target(Player_character);
        }

        private void Start_boss_battle()
        {
            // Events
            Game_events.Boss_battle_started.Invoke(Character);
            Game_events.Warning_panel_called.Invoke("Boss encounter !");
            Game_events.Camera_shake.Invoke();

            // Audio
            Game_utils.Instance.Create_2d_sound("Boss_encounter", "Audios/Characters/Roar_1");
        }

        internal void Set_characterData(Character_scriptable data)
        {
            // Set
            characterData = data;

            Configure();
        }

        private void Set_target(GameObject obj)
        {
            // Set
            Target_obj = obj;

            Character.Set_stance(Character_behaviour.Character_stances.Agressive);
        }

        /// MAIN METHODS
        private void UseAttack()
        {
            if (Character.IsAttacking || Character.IsUsingSpell)
                return;
        
            // Use magic or just hit
            if (Character.CanUseMagic)
            {
                int spell_id = Random.Range(0, Character.Spells.Count);
                Spell_scriptable spellData = Character.Spells[spell_id];

                // Set
                Character.IsUsingSpell = true;

                // Cast
                Character.character_spells_controller.Cast_spell(spell: spellData, origin_pos: transform.position, target_obj: Target_obj);

                // Audio
                Game_utils.Instance.Create_sound("Boss_encounter", "Audios/Characters/Strong_roar_1", transform.position);

                // Events
                Game_events.Attack_indicator.Invoke(new Attack_indicator_controller.Indicator_info { Format = spellData.Area_effect_type, Duration = spellData.Cast_time, Radius = spellData.Radius }, this.gameObject.transform.position);

                // Wait
                StartCoroutine(Action_time(spellData.Duration));
            }
            else
            {
                // Set
                Character.IsAttacking = true;
                Character.Hit_other_entity(Target_obj);

                // Wait
                StartCoroutine(Action_time(1f));
            }
        }
        
        private IEnumerator Brain_controller()
        {
            while(true)
            {
                yield return new WaitForSeconds(0.5f);

                switch (Character.Current_stance)
                {
                    case Character_behaviour.Character_stances.Idle:
                        break;

                    case Character_behaviour.Character_stances.Neutral:
                        break;

                    case Character_behaviour.Character_stances.Agressive:
                        if (Target_obj != null && !Character.IsAttacking && !Character.IsUsingSpell)
                        {
                            // Move towards target
                            Vector3 direction = (Target_obj.transform.position - transform.position).normalized;
                            Character.Move(direction);

                            // Check if the enemy can attack the target
                            if (Vector3.Distance(transform.position, Target_obj.transform.position) < Character.detectionRadius)
                            {
                                UseAttack();
                            }
                        }
                        break;
                }

                // Effects
                if (this.gameObject.TryGetComponent<Effects_controller>(out Effects.Effects_controller effectsController))
                {
                    effectsController.Force_effect(Effects_controller.EffectType.SquashStretch);
                }
            }
        }

        private IEnumerator Action_time(float time)
        {
            yield return new WaitForSeconds(time);

            Character.IsAttacking = false;
            Character.IsUsingSpell = false;

            Character.IsPlowing = false;
            Character.IsWatering = false;
            Character.IsAttacking = false;
            Character.IsHarvesting = false;
            Character.IsMining = false;
            Character.IsCutting = false;
        }
    }
}