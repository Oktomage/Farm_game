using UnityEngine;

namespace Game.Objects
{
    public class Object_behaviour : MonoBehaviour
    {
        [Header("Components")]
        public Object_scriptable ObjectData;
        [Space]
        public SpriteRenderer Render;
        public Collider2D Collider;
    }
}
