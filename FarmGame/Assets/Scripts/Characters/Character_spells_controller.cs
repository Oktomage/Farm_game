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
        public class Cast_spell_data
        {
            public Spell_scriptable SpellData;

            public Vector2 Origin_pos;
            public GameObject Target_obj;
        }

        internal void Cast_spell(Cast_spell_data data)
        {
            GameObject spell_obj = Game_utils.Instance.Create_gameObject(data.Origin_pos);
            Base_spell spell_script = null;

            switch (data.SpellData.Spell_name)
            {
                case "Ground hit":

                    spell_script = spell_obj.AddComponent<Ground_hit_spell>();
                    break;

                case "Lava hole":
                    spell_script = spell_obj.AddComponent<Lava_hole_spell>();
                    break;

                case "Ice spikes":
                    spell_script = spell_obj.AddComponent<Ice_spikes_spell>();
                    break;
            }

            if (spell_script != null)
                spell_script.Active(Character, data);
        }
    }
}