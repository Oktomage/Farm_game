using Game.Characters;
using Game.Events;
using Game.Utils;
using Game.Utils.Misc;
using System.Collections;
using UnityEngine;
using static Game.Characters.Spells.Character_spells_controller;

namespace Game.Magic.Spell
{
    public class Ground_hit_spell : Base_spell
    {
        private Character_behaviour Character;
        private Cast_spell_data Cast_data;

        public override void Active(Character_behaviour character, Cast_spell_data data)
        {
            // Set
            Character = character;
            Cast_data = data;

            StartCoroutine(Cast_ground_hit());

            // Events
            Game_events.Attack_indicator.Invoke(new Attack_indicator_controller.Indicator_info { Format = data.SpellData.Area_effect_type, Duration = data.SpellData.Cast_time, Radius = data.SpellData.Radius, Target_obj = Character.gameObject});
        }

        /// MAIN METHODS
        public override void DoDamage()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(Cast_data.Origin_pos, Cast_data.SpellData.Radius);

            foreach (Collider2D hit in hits)
            {
                if(hit.gameObject.GetComponentInParent<Character_behaviour>())
                {
                    Character_behaviour other_character = hit.gameObject.GetComponentInParent<Character_behaviour>();

                    if(other_character == Character)
                        continue;

                    // Do damage
                    other_character?.TakeDamage(Cast_data.SpellData.Damage, Character?.gameObject);
                }
            }
        }

        private IEnumerator Cast_ground_hit()
        {
            yield return new WaitForSeconds(Cast_data.SpellData.Cast_time);

            DoDamage();

            // Effects
            Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Impact_dirt", Cast_data.Origin_pos);

            // Audio
            Game_utils.Instance.Create_sound("Ground_hit", Game_utils.Instance.Get_audio_clip("Audios/Spells/Heavy_ground_impact_1"), Cast_data.Origin_pos);

            // Events
            Game_events.Camera_shake.Invoke();
            Game_events.ShaderEffect.Invoke("Shockwave", Cast_data.Origin_pos);
        }
    }
}