using Game.Characters;
using Game.Characters.Spells;
using UnityEngine;

namespace Game.Magic.Spell
{
    public abstract class Base_spell : MonoBehaviour
    {
        public abstract void Active(Character_behaviour character, Character_spells_controller.Cast_spell_data data);
        public abstract void DoDamage();
    }
}