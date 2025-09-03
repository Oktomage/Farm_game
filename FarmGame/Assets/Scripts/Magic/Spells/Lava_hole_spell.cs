using Game.Characters;
using Game.Events;
using Game.Utils;
using System.Collections;
using UnityEngine;
using static Game.Characters.Spells.Character_spells_controller;

namespace Game.Magic.Spell
{
    public class Lava_hole_spell : Base_spell
    {
        private Character_behaviour Character;
        private Cast_spell_data Cast_data;

        private bool CanDamage = true;

        public override void Active(Character_behaviour character, Cast_spell_data data)
        {
            // Set
            Character = character;
            Cast_data = data;

            StartCoroutine(Cast_lava_hole());
            StartCoroutine(Kill_spell(data.SpellData.Duration));
        }

        /// MAIN METHODS
        private void DoDamage()
        {
            if (!CanDamage)
                return;

            // Set
            CanDamage = false;

            // Detect characters
            Collider2D[] hits = Physics2D.OverlapCircleAll(Cast_data.Origin_pos, Cast_data.SpellData.Radius);

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject.GetComponentInParent<Character_behaviour>())
                {
                    Character_behaviour other_character = hit.gameObject.GetComponentInParent<Character_behaviour>();

                    // Not himself
                    if (other_character == Character)
                        continue;

                    // Do damage
                    other_character.TakeDamage(Cast_data.SpellData.Damage, Character.gameObject);
                }
            }

            StartCoroutine(Damage_interval(0.5f));
        }

        private IEnumerator Cast_lava_hole()
        {
            yield return new WaitForSeconds(Cast_data.SpellData.Cast_time);

            // Effects
            Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Lava_hole", this.gameObject);

            // Audio
            Game_utils.Instance.Create_sound("Ground_hit", Game_utils.Instance.Get_audio_clip("Audios/Spells/Heavy_ground_impact_1"), Cast_data.Origin_pos);

            // Events
            Game_events.Camera_shake.Invoke();
            Game_events.ShaderEffect.Invoke("Shockwave", Cast_data.Origin_pos);

            float t = 0;

            // Continuous damage
            while (t < Cast_data.SpellData.Duration)
            {
                t += Time.deltaTime;

                DoDamage();

                yield return null;
            }
        }

        private IEnumerator Kill_spell(float t)
        {
            yield return new WaitForSeconds(t);

            Destroy(this.gameObject);
        }

        private IEnumerator Damage_interval(float t)
        {
            yield return new WaitForSeconds(t);

            // Set
            CanDamage = true;
        }
    }
}