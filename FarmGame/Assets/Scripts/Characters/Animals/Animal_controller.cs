using Game.Effects;
using Game.Events;
using System.Collections;
using UnityEngine;

namespace Game.Characters.Animals
{
    public class Animal_controller : MonoBehaviour
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
            if (characterData != null)
                Configure();
        }

        /// CORE METHODS
        private void Configure()
        {
            // Set
            Character.Configure(characterData);

            Character.Set_stance(Character_behaviour.Character_stances.Neutral);

            Character.Render.sprite = characterData.Icon;

            StopAllCoroutines();
            StartCoroutine(Brain_controller());
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
        } // NOT USING YET

        /// MAIN METHODS
        private IEnumerator Brain_controller()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);

                Vector3 move_dir = Vector3.zero;

                switch (Character.Current_stance)
                {
                    case Character_behaviour.Character_stances.Idle:
                        break;

                    case Character_behaviour.Character_stances.Neutral:
                        // Get random direction
                        move_dir = Random.onUnitSphere;

                        Character.Move(move_dir);

                        yield return new WaitForSeconds(Random.Range(1f, 5f));
                        break;

                    case Character_behaviour.Character_stances.Agressive:
                        if (Target != null)
                        {
                            // Get the direction to the target
                            move_dir = (Target.transform.position - transform.position).normalized;

                            Character.Move(move_dir);
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
