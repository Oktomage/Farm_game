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

            // Set
            Character.IsAttacking = true;
            Character.Hit_other_entity(Target_obj);

            // Wait
            StartCoroutine(Action_time(1f));
        }

        private void UseSpell(Spell_scriptable.Spell_range_styles Spell_range)
        {
            int spell_id = Random.Range(0, Character.Spells.Count);
            Spell_scriptable spellData = Character.Spells[spell_id];

            // Call
            Character.Cast_spell(spellData, Target_obj);

            // Audio
            Game_utils.Instance.Create_sound("Boss_encounter", "Audios/Characters/Strong_roar_1", transform.position);

            // Wait if needed
            if (spellData.Need_rest)
                StartCoroutine(Action_time(spellData.Cast_time));
            else
                Character.IsUsingSpell = false;
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

                            // Check if the target is in the attack range
                            if (Vector3.Distance(transform.position, Target_obj.transform.position) < Character.detectionRadius)
                            {
                                if(Character.CanUseMagic)
                                    UseSpell(Spell_scriptable.Spell_range_styles.Melee);
                                else
                                    UseAttack();
                            }
                            // Use attack anyway
                            else
                            {
                                if (Character.CanUseMagic)
                                {
                                    float c = Random.value;
                                    float attack_chance = 0.2f;

                                    if (c <= attack_chance)
                                        UseSpell(Spell_scriptable.Spell_range_styles.Ranged);
                                }
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