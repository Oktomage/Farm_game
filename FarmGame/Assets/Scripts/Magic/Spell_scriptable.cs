using Game.Utils.Misc;
using UnityEngine;

namespace Game.Magic
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "Magic/Spell")]
    public class Spell_scriptable : ScriptableObject
    {
        [Header("Settings")]
        public string Spell_name;
        public string Spell_description;

        [Space]
        public Attack_indicator_controller.Indicator_info.Indicator_formats Area_effect_type;

        [Space]
        public float Damage;
        [Range(0f, 5f)]
        public float Radius;
        [Range(0f, 10f)]
        public float Cast_time;
        [Range(0f, 10f)]
        public float Duration;

        [Space]
        public bool Need_rest = false;
    }
}