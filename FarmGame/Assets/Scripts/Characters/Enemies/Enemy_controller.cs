using Game.Effects;
using Game.Events;
using Game.Utils;
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
        public GameObject Target;

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

            Character.Render.sprite = characterData.Icon;

            // Boss
            if (characterData.IsBoss)
                Start_boss_battle();

            StopAllCoroutines();
            StartCoroutine(Brain_controller());

            Set_target(Player_character);
        }

        private void Start_boss_battle()
        {
            // Events
            Game_events.Boss_battle_started.Invoke(Character);
            Game_events.Warning_panel_called.Invoke("Boss encounter !");

            //Audio
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
            Target = obj;

            Character.Set_stance(Character_behaviour.Character_stances.Agressive);
        }

        /// MAIN METHODS
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
                        if (Target != null)
                        {
                            //Get the direction to the target
                            Vector3 direction = (Target.transform.position - transform.position).normalized;

                            Character.Move(direction);

                            //Check if the enemy can attack the target
                            if (Vector3.Distance(transform.position, Target.transform.position) < Character.detectionRadius)
                            {
                                Character.Hit_other_entity(Target);
                            }
                        }
                        break;
                }

                //Effects
                if (this.gameObject.TryGetComponent<Effects_controller>(out Effects.Effects_controller effectsController))
                {
                    effectsController.Force_effect(Effects_controller.EffectType.SquashStretch);
                }
            }
        }
    }
}