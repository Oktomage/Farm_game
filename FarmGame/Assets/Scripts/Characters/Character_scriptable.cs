using Game.Items;
using UnityEngine;

namespace Game.Characters
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Characters/Character")]
    public class Character_scriptable : ScriptableObject
    {
        public enum Character_menance_class
        {
            Peaceful,
            Pathetic,
            Common,
            Menacing,
            Relentless,
            Cataclysmic,
            Apocalyptic
        }

        public string Name;

        public float Max_health;
        public float Move_speed;
        public float Damage;

        public int Souls_reward = 1;

        [Space]
        public Character_menance_class Menance_class = Character_menance_class.Common;

        [Space]
        public Sprite Icon;

        [Header("Drop settings")]
        public Item_scriptable Item_drop;
        [Range(0, 1)]
        public float Drop_chance = 0.5f;

        [Header("Audio settings")]
        public AudioClip Move_sound;
        public AudioClip Attack_sound;
        public AudioClip Hurt_sound;
        public AudioClip Death_sound;
    }
}