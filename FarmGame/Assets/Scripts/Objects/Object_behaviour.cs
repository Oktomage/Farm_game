using UnityEngine;

namespace Game.Objects
{
    public class Object_behaviour : MonoBehaviour
    {
        [Header("Components")]
        public Object_scriptable ObjectData;
        public SpriteRenderer Render => this.gameObject.GetComponentInChildren<SpriteRenderer>();
        public Collider2D Collider => this.gameObject.GetComponentInChildren<Collider2D>();

        internal void Set_sprite(Sprite sprite)
        {
            // Set
            Render.sprite = sprite;
        }
    }
}
