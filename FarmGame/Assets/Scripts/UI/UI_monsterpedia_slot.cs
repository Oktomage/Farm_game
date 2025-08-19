using Game.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UI_monsterpedia_slot : MonoBehaviour
    {
        internal Image Image_monster => this.gameObject.GetComponentInChildren<Image>();
        internal Character_scriptable CharacterData;

        /// CORE METHODS
        internal void Set_character(Character_scriptable character)
        {
            // Set
            CharacterData = character;

            Image_monster.sprite = CharacterData.Icon;
        }
    }
}