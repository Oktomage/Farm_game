using Game.Items;
using UnityEngine;

namespace Game.Characters.Enemies
{
    public class Enemy_scriptable : ScriptableObject
    {
        public string Name;

        public float Max_health;
        public float Move_speed;
        public float Damage;

        public int Souls_reward = 1;

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


