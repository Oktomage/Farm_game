using Game.Effects;
using System.Collections;
using UnityEngine;

namespace Game.Characters.Enemies
{
    public class Enemy_controller : MonoBehaviour
    {
        [Header("Data")]
        public Character_scriptable characterData;

        [Header("Components")]
        public Character_behaviour Character;

        [Header("Mind")]
        public GameObject Target;

        //Internal variables
        internal GameObject Player_character;

        private void Start()
        {
            if(characterData != null) { Configure(); }
        
            Player_character = GameObject.FindGameObjectWithTag("Player");
            Set_target(Player_character);
        }

        /// CORE METHODS
        private void Configure()
        {
            //Set
            Character.Configure(characterData);
            Character.IsEnemy = true;

            Character.Render.sprite = characterData.Icon;

            StopAllCoroutines();
            StartCoroutine(Brain_controller());
        }

        internal void Set_characterData(Character_scriptable data)
        {
            //Set
            characterData = data;

            Configure();
        }

        private void Set_target(GameObject obj)
        {
            Target = obj;
        }

        /// MAIN METHODS
        IEnumerator Brain_controller()
        {
            while(true)
            {
                yield return new WaitForSeconds(0.5f);

                if(Target != null) 
                {
                    //Get the direction to the target
                    Vector3 direction = (Target.transform.position - transform.position).normalized;

                    Character.Move(direction);

                    //Effects
                    if(this.gameObject.TryGetComponent<Effects_controller>(out Effects.Effects_controller effectsController))
                    {
                        effectsController.Force_effect(Effects_controller.EffectType.SquashStretch);
                    }

                    //Check if the enemy can attack the target
                    if (Vector3.Distance(transform.position, Target.transform.position) < Character.detectionRadius)
                    {
                        Character.Hit_other_entity(Target);
                    }
                }
            }
        }
    }
}