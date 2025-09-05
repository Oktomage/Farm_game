using Game.Characters;
using Game.Events;
using Game.Utils;
using Game.Utils.Misc;
using System.Collections;
using UnityEngine;
using static Game.Characters.Spells.Character_spells_controller;

namespace Game.Magic.Spell
{
    public class Meteor_spell : Base_spell
    {
        private Character_behaviour Character;
        private Cast_spell_data Cast_data;

        private GameObject Current_meteor_obj;

        public override void Active(Character_behaviour character, Cast_spell_data data)
        {
            // Set
            Character = character;
            Cast_data = data;

            StartCoroutine(Cast_meteor());

            // Events
            Game_events.Attack_indicator.Invoke(new Attack_indicator_controller.Indicator_info { Format = data.SpellData.Area_effect_type, Duration = data.SpellData.Cast_time, Radius = data.SpellData.Radius, Target_obj = data.Target_obj });
        }

        /// MAIN METHODS
        public override void DoDamage()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(Current_meteor_obj.transform.position, Cast_data.SpellData.Radius);

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject.GetComponentInParent<Character_behaviour>())
                {
                    Character_behaviour other_character = hit.gameObject.GetComponentInParent<Character_behaviour>();

                    if (other_character == Character)
                        continue;

                    // Do damage
                    other_character?.TakeDamage(Cast_data.SpellData.Damage, Character?.gameObject);
                }
            }
        }

        private GameObject Create_meteor(Vector2 pos, Vector2 scale)
        {
            GameObject meteor_obj = Game_utils.Instance.Create_gameObject();
            meteor_obj.transform.SetParent(this.gameObject.transform);

            // Set
            meteor_obj.transform.position = pos;
            meteor_obj.transform.localScale = scale;

            // Render
            SpriteRenderer render = meteor_obj.AddComponent<SpriteRenderer>();
            render.sprite = Game_utils.Instance.Get_sprite("Graphics/Magic/Spells/Meteor_spell"); 
            render.sortingOrder = 10; 

            return meteor_obj;
        }

        private IEnumerator Cast_meteor()
        {
            yield return new WaitForSeconds(Cast_data.SpellData.Cast_time);

            Vector3 dir = new Vector2(1, 1);
            float initial_distance = 10;
            Vector3 initial_target_pos = Cast_data.Target_obj.transform.position;
            Vector2 meteor_pos = initial_target_pos + (dir * initial_distance);

            GameObject Meteor_obj = Create_meteor(meteor_pos, scale: Vector2.one);
            Current_meteor_obj = Meteor_obj;

            // Audio
            Game_utils.Instance.Create_sound("Ground_hit", Game_utils.Instance.Get_audio_clip("Audios/Spells/Meteor_1"), Current_meteor_obj.transform.position);

            // Move meteor
            float meteor_speed = 16;

            while (Vector2.Distance(initial_target_pos, Meteor_obj.transform.position) > Mathf.Epsilon)
            {
                Meteor_obj.transform.position = Vector2.MoveTowards(Meteor_obj.transform.position, initial_target_pos, meteor_speed * Time.deltaTime);

                yield return null;
            }

            DoDamage();

            // Effects
            Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Impact_dirt", Current_meteor_obj.transform.position);

            // Audio
            Game_utils.Instance.Create_sound("Ground_hit", Game_utils.Instance.Get_audio_clip("Audios/Spells/Explosion_1"), Current_meteor_obj.transform.position);

            // Events
            Game_events.Camera_shake.Invoke();
            Game_events.ShaderEffect.Invoke("Shockwave", Current_meteor_obj.transform.position);
        
            Destroy(Current_meteor_obj);
        }
    }
}