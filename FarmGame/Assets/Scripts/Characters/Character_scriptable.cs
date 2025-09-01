using Game.Items;
using Game.Magic;
using NaughtyAttributes;
using System.Collections.Generic;
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

        [Header("Settings")]
        public string Name;
        [Range(0.1f, 5f)]
        public float Size_multiplier = 1f;

        public float Max_health;
        public float Move_speed;
        public float Damage;

        public int Souls_reward = 1;

        [Space(10)]
        public bool IsExtraStrong;
        public bool IsExtraFast;
        public bool HaveMagicalResistance;
        public bool HavePhysicalResistance;

        [Header("Class settings")]
        public bool IsBoss = false;
        public Character_menance_class Menance_class = Character_menance_class.Common;

        [Header("Magic settings")]
        public bool CanUseMagic = false;
        [ShowIf("CanUseMagic")]
        public List<Spell_scriptable> Spells = new List<Spell_scriptable>();

        [Space]
        public Sprite Icon;

        [Header("Drop settings")]
        public Item_scriptable Item_drop;
        [Range(0, 1)]
        [ShowIf("Item_drop")]
        public float Drop_chance = 0.5f;

        [Header("Audio settings")]
        public AudioClip Idle_sound;
        public AudioClip Move_sound;
        public AudioClip Attack_sound;
        public AudioClip Hurt_sound;
        public AudioClip Death_sound;
    }
}