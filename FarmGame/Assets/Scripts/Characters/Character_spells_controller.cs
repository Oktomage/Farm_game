using Game.Magic;
using Game.Utils;
using System.Collections;
using UnityEngine;

namespace Game.Characters.Spells
{
    public class Character_spells_controller : MonoBehaviour
    {
        [Header("Components")]
        internal Character_behaviour Character => this.gameObject.GetComponent<Character_behaviour>();

        /// CORE METHODS
        internal void Cast_spell(Spell_scriptable spell, Vector2 origin_pos, GameObject target_obj)
        {
            switch(spell.Spell_name)
            {
                case "Ground hit":
                    StartCoroutine(Cast_ground_hit(spell, origin_pos));
                    break;
            }
        }

        /// MAIN METHODS
        private IEnumerator Cast_ground_hit(Spell_scriptable spellData, Vector2 pos)
        {
            yield return new WaitForSeconds(spellData.Cast_time);

            // Effects
            Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Impact_dirt", pos);

            // Audio
            Game_utils.Instance.Create_sound("Ground_hit", Game_utils.Instance.Get_audio_clip("Audios/Spells/Heavy_ground_impact_1"), pos);
        }
    }
}