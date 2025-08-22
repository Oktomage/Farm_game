using System.Collections.Generic;
using UnityEngine;

namespace Game.Objects.Entitys
{
    public class Flowers_entity : MonoBehaviour
    {
        [Header("Settings")]
        public List<Sprite> Sprite_variations = new List<Sprite>();

        [Header("Components")]
        public Object_behaviour Object_behaviour => this.gameObject.GetComponent<Object_behaviour>();

        private void Start()
        {
            // Set
            Object_behaviour.Set_sprite(Sprite_variations[Random.Range(0, Sprite_variations.Count)]);
        }
    }
}