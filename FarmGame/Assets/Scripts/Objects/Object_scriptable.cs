using Game.Items;
using NaughtyAttributes;
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

        [ShowIf(nameof(HasItemDrop))]
        [Range(0, 1f)]
        public float Drop_chance = 0.5f;
        [ShowIf(nameof(HasItemDrop))]
        public int Drop_amount = 1;

        private bool HasItemDrop => Item_drop != null;
    }
}