using Game.Characters;
using Game.Characters.Spells;
using Game.Events;
using Game.Utils;
using Game.Utils.Misc;
using System.Collections;
using UnityEngine;
using static Game.Characters.Spells.Character_spells_controller;

namespace Game.Magic.Spell
{
    public class Ice_spikes_spell : Base_spell
    {
        private Character_behaviour Character;
        private Cast_spell_data Cast_data;

        private int Spikes_amount = 6;
        private float Scale_amount_per_spike = 1.05f;

        private GameObject Current_spike_obj;

        public override void Active(Character_behaviour character, Character_spells_controller.Cast_spell_data data)
        {
            // Set
            Character = character;
            Cast_data = data;

            StartCoroutine(Cast_ice_spikes());

            // Events
            Game_events.Attack_indicator.Invoke(new Attack_indicator_controller.Indicator_info { Format = data.SpellData.Area_effect_type, Duration = data.SpellData.Cast_time, Radius = data.SpellData.Radius, Target_obj = data.Target_obj });
        }

        /// MAIN METHODS
        public override void DoDamage()
        {
            // Detect characters
            Collider2D[] hits = Physics2D.OverlapCircleAll(Current_spike_obj.transform.position, Cast_data.SpellData.Radius);

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject.GetComponentInParent<Character_behaviour>())
                {
                    Character_behaviour other_character = hit.gameObject.GetComponentInParent<Character_behaviour>();

                    // Not himself
                    if (other_character == Character)
                        continue;

                    // Do damage
                    other_character?.TakeDamage(Cast_data.SpellData.Damage, Character?.gameObject);
                }
            }
        }

        private GameObject Create_spike(Vector2 pos, Vector2 scale)
        {
            GameObject spike_obj = Game_utils.Instance.Create_gameObject();
            spike_obj.transform.SetParent(this.gameObject.transform);

            // Set
            spike_obj.transform.position = pos;
            spike_obj.transform.localScale = scale;

            // Render
            SpriteRenderer render = spike_obj.AddComponent<SpriteRenderer>();
            render.sprite = Game_utils.Instance.Get_sprite("Graphics/Magic/Spells/Ice_spike");
            render.color = new Color(1, 1, 1, 0.6f);
            render.sortingOrder = 3; // Just to be above the characters

            return spike_obj;
        }

        private IEnumerator Cast_ice_spikes()
        {
            yield return new WaitForSeconds(Cast_data.SpellData.Cast_time);

            // Effects
            Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Ice_spike_floor", this.gameObject);

            for (int i = 0; i < Spikes_amount; i++)
            {
                Vector2 dir = (Cast_data.Target_obj.transform.position - transform.position).normalized;
                Vector2 pos = Cast_data.Origin_pos + (dir * i);

                GameObject spike_obj = Create_spike(pos, Vector2.one * (Scale_amount_per_spike * i));
                Current_spike_obj = spike_obj;

                // Audio
                Game_utils.Instance.Create_sound("Ice_spike", Game_utils.Instance.Get_audio_clip("Audios/Spells/Ice_spike_1"), pos);

                DoDamage();

                StartCoroutine(Kill_spell_obj(spike_obj, Cast_data.SpellData.Duration / (Spikes_amount * 0.5f)));

                float spike_interval = 0.1f;
                yield return new WaitForSeconds(spike_interval);
            }

            StartCoroutine(Kill_spell_obj(this.gameObject, Cast_data.SpellData.Duration));
        }

        private IEnumerator Kill_spell_obj(GameObject obj, float t)
        {
            yield return new WaitForSeconds(t);

            // Audio
            Game_utils.Instance.Create_sound("Ice_spike", Game_utils.Instance.Get_audio_clip("Audios/Spells/Ice_spike_1"), obj.transform.position);

            // Effects
            Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Ice_spike_sparks", obj.transform.position);

            Destroy(obj);
        }
    }
}