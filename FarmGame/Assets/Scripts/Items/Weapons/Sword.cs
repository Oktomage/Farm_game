using Game.Characters;
using Game.Items.Tools;
using Game.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items.Weapons
{
    public class Sword : MonoBehaviour
    {
        [Header("Data")]
        public Tool_behaviour Tool => this.gameObject.GetComponent<Tool_behaviour>();

        /// MAIN METHODS
        internal void Attack(Vector2 dir)
        {
            Vector2 detections_point = new Vector2(Tool.Character.transform.position.x + dir.x, Tool.Character.transform.position.y + dir.y);
            Collider2D[] hits = Physics2D.OverlapCircleAll(detections_point, 0.5f);
            List<Character_behaviour> detected_Characters = new List<Character_behaviour>();

            foreach (Collider2D hit in hits)
            {
                Character_behaviour character = hit.GetComponentInParent<Character_behaviour>();

                if (character != null && character != Tool.Character) // Avoid self
                {
                    // Set
                    detected_Characters.Add(character);
                }
            }

            if (detected_Characters.Count == 0)
                return;

            // Do damage
            foreach (Character_behaviour character in detected_Characters)
            {
                character.TakeDamage(1f, Tool.Character.gameObject);
            }
        }
    }
}