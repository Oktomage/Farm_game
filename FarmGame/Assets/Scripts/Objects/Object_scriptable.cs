using Game.Items;
using UnityEngine;

namespace Game.Objects
{
    [CreateAssetMenu(fileName = "New Object", menuName = "Objects/Object")]
    public class Object_scriptable : ScriptableObject
    {
        [Header("Object Settings")]
        public string ObjectName = "New Object";
        public string Description = "Object Description";
        public Sprite Object_sprite;

        [Header("Drop settings")]
        public Item_scriptable Item_drop;
        [Range(0, 1f)]
        public float Drop_chance = 0.5f;
        public int Drop_amount = 1;
    }
}