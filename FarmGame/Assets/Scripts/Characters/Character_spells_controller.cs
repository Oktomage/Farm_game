using Game.Events;
using Game.Magic;
using Game.Magic.Spell;
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
        internal class Cast_spell_data
        {
            internal Spell_scriptable SpellData;

            internal Vector2 Origin_pos;
            internal GameObject Target_obj;
        }

        internal void Cast_spell(Cast_spell_data data)
        {
            switch(data.SpellData.Spell_name)
            {
                case "Ground hit":
                    GameObject spell_obj = Game_utils.Instance.Create_gameObject(data.Origin_pos);
                    Ground_hit_spell spell_script = spell_obj.AddComponent<Ground_hit_spell>();

                    // Active
                    spell_script.Active(Character, data);
                    break;
            }
        }
    }
}