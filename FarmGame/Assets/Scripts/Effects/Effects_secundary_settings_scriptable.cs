using NaughtyAttributes;
using UnityEngine;

namespace Game.Effects
{
    [CreateAssetMenu(fileName = "New secundary settings", menuName = "Effects/Secundary_settings")]
    public class Effects_secundary_settings_scriptable : ScriptableObject
    {
        public bool Have_souls_aura = false;
        [ShowIf("Have_souls_aura")]
        public Color Souls_aura_color = Color.white;

        public bool Have_trail = false;
        [ShowIf("Have_trail")]
        public Trail_effect_scriptable Trail_Effect = null;
    }
}