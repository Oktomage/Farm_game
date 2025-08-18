using Game.Characters.Data;
using Game.Characters.Shopper;
using Game.Effects;
using Game.Events;
using Game.Grids;
using Game.Items;
using Game.Items.Tools;
using Game.Objects;
using Game.UI.Notifications;
using Game.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Characters
{
    public class Character_behaviour : MonoBehaviour
    {
        [Header("Character Settings")]
        public float Health = 5f;
        public float Max_health = 5f;
        public float MoveSpeed = 5f;
        public float AttackDamage = 1f;
        public float detectionRadius = 1.5f;
        public int Souls = 1;

        [Header("States")]
        public bool IsPlayer = false;
        public bool IsAlly = false;
        public bool IsEnemy = false;

        [Space]
        public bool IsUsingTool = false;
        public bool IsAlive = true;
        public bool IsMoving = false;
        public bool IsPlowing = false;
        public bool IsWatering = false;
        public bool IsAttacking = false;
        public bool IsHarvesting = false;
        public bool IsMining = false;
        public bool IsCutting = false;

        [Header("Inventory")]
        public int InventorySize = 2;
        public List<GameObject> Inventory = new List<GameObject>();
        public Tool_behaviour Equipped_tool;
        public int Selected_item_index = 0;

        [Header("Audio settings")]
        public AudioClip Move_sound;
        public AudioClip Attack_sound;
        public AudioClip Hurt_sound;
        public AudioClip Death_sound;

        [Header("Components")]
        public SpriteRenderer Render;
        public Animator Anim;
        public Rigidbody2D Rigid;
        public Collider2D Collider;

        [Space(30)]
        //Internal variables
        [SerializeField] internal bool IsGodMode = false; // For testing purposes
        [SerializeField] internal List<Item_behaviour> Items_nearby = new List<Item_behaviour>();
        [SerializeField] internal Grid_controller Current_grid;
        internal Vector3 Moving_dir = Vector3.zero;
        [SerializeField] internal List<Object_behaviour> Objects_nearby = new List<Object_behaviour>();
        [SerializeField] internal List<Character_behaviour> Characters_nearby = new List<Character_behaviour>();
        [SerializeField] internal GameObject Last_damaged_by_obj;

        private void Awake()
        {
#if UNITY_EDITOR
            if (IsPlayer)
                Enable_editor_settings();
#endif
        }

        private void Start()
        {
            //Routines
            StartCoroutine(Regens());
            StartCoroutine(Detect_items_nearby());
            StartCoroutine(Animator_controller());
            StartCoroutine(Detect_grid());
            StartCoroutine(Detect_objects_nearby());
            StartCoroutine(Detect_characters_nearby());
        }

        private void LateUpdate()
        {
            Fix_character();
        }

        ///  CORE METHODS
        private void Enable_editor_settings()
        {
            IsGodMode = true;
        }

        internal void Configure(Character_scriptable character_scriptable)
        {
            //Set
            Max_health = character_scriptable.Max_health;
            Health = Max_health;
            MoveSpeed = character_scriptable.Move_speed;
            AttackDamage = character_scriptable.Damage;

            InventorySize = 2;
            Inventory = new List<GameObject>(new GameObject[InventorySize]);

            Souls = character_scriptable.Souls_reward;

            //Audios
            Move_sound = character_scriptable.Move_sound;
            Attack_sound = character_scriptable.Attack_sound;
            Hurt_sound = character_scriptable.Hurt_sound;
            Death_sound = character_scriptable.Death_sound;
        }

        private void Die()
        {
            if (IsGodMode) { return; }

            //Set
            IsAlive = false;

            if(Last_damaged_by_obj.CompareTag("Player"))
            {
                Player_data.Instance.Add_souls(Souls);

                //Events
                Game_events.Player_collected_souls.Invoke(Souls);
                Game_events.Player_character_killed_enemy.Invoke(this.gameObject);
            }

            //Audio
            Game_utils.Instance.Create_sound("Character_hurt", Death_sound, transform.position);

            //Particles
            Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Death_souls", transform.position);

            Destroy(this.gameObject);
        }

        private void TakeDamage(float dmg, GameObject attacker)
        {
            if (IsAlive)
            {
                //Set
                Health -= dmg;
                Health = Mathf.Clamp(Health, 0, Max_health);

                if (Health <= 0)
                {
                    Die();
                }

                //Set last damaged by
                Last_damaged_by_obj = attacker;

                //Events
                if (IsPlayer)
                {
                    Game_events.Player_character_took_damage.Invoke(Last_damaged_by_obj);
                }

                //Effects
                if (this.gameObject.TryGetComponent<Effects_controller>(out Effects_controller effects))
                {
                    effects.Force_effect(Effects_controller.EffectType.Boing);
                    effects.Force_effect(Effects_controller.EffectType.Flash);
                }

                //Audio
                Game_utils.Instance.Create_sound("Character_hurt", Hurt_sound, transform.position);
            }
        }

        private void Regenerate(float hlth_regen)
        {
            Health += hlth_regen; // Regenerate health
            Health = Mathf.Clamp(Health, 0, Max_health);

            //Events
            if(IsPlayer)
            {
                Game_events.Player_character_regen.Invoke(hlth_regen);
            }
        }

        ///  ACTIONS METHODS
        internal void Move(Vector3 dir)
        {
            if(IsUsingTool) { return; }

            if (!IsMoving)
            {
                IsMoving = true;
                Moving_dir = dir;

                Vector3 target_pos = transform.position + dir;

                if (Check_walkable_targetPosition(target_pos))
                {
                    StartCoroutine(Move_routine(target_pos));
                }
                else
                {
                    IsMoving = false;
                }
            }
        }

        private bool Check_walkable_targetPosition(Vector3 targetPosition)
        {
            bool walkable = true;

            int layerMask = 1 << LayerMask.NameToLayer("NotWalkable");
            Collider2D[] hits = Physics2D.OverlapCircleAll(targetPosition, 0.1f, layerMask);

            if (hits.Length > 0)
            {
                walkable = false;
            }

            return walkable;
        }

        private IEnumerator Move_routine(Vector3 targetPosition)
        {
            float t = 0;

            while ((targetPosition - transform.position).sqrMagnitude > Mathf.Epsilon)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);

                //Effects
                t += Time.deltaTime;

                if (t > 0.2f)
                {
                    t = 0;

                    //Particles
                    Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Walk_grass", transform.position);

                    //Audio
                    Game_utils.Instance.Create_sound("Move", Move_sound, this.transform.position);
                }

                yield return null;
            }

            //Clamp the position to the target position to avoid overshooting
            transform.position = targetPosition;

            IsMoving = false;
        }

        internal void Collect()
        {
            if (Inventory.Count < InventorySize)
            {
                //Get closest item
                float closest_distance = float.PositiveInfinity;
                Item_behaviour item_behaviour = null;

                foreach (Item_behaviour item in Items_nearby.ToArray())
                {
                    if (item.IsCollectable) 
                    {
                        float distance = Vector2.Distance(transform.position, item.transform.position);

                        if (distance < closest_distance)
                        {
                            closest_distance = distance;

                            //Set
                            item_behaviour = item;
                        }
                    }
                }

                if(item_behaviour == null) { return; }

                //Add
                Inventory.Add(item_behaviour.gameObject);

                item_behaviour.Set_collected_settings();

                //Events
                if (IsPlayer)
                {
                    Notifications_controller.Instance.Set_notification_task(new Notifications_controller.Notification
                    {
                        Title = item_behaviour.ItemData.ItemName,
                        Message = $"You collected {item_behaviour.ItemData.ItemName}.",
                        Duration = 1f,
                        Icon = item_behaviour.ItemData.Icon
                    });

                    Game_events.Player_character_collected_item.Invoke();
                }
            }
        }

        internal void Use_tool()
        {
            if(IsUsingTool) { return; }
            if (Equipped_tool == null) { return; }

            Equipped_tool.Use(this);
        }

        internal void Hit_other_entity(GameObject obj)
        {
            //Check if the object is a character
            if (obj.TryGetComponent<Character_behaviour>(out Character_behaviour other_character))
            {
                other_character.TakeDamage(AttackDamage, this.gameObject);
            }
        }

        internal void Select_item_from_inventory(int id)
        {
            if (Inventory.Count > 0)
            {
                // Clamp the id to the inventory size
                Selected_item_index = Mathf.Clamp(id, 0, Inventory.Count - 1);
                
                // Check if isnt null
                if (Inventory[Selected_item_index] == null)
                {
                    Equipped_tool = null;
                }

                // Check if the selected item is a tool
                if (Inventory[Selected_item_index].GetComponent<Tool_behaviour>())
                {
                    Equipped_tool = Inventory[Selected_item_index].GetComponent<Tool_behaviour>();
                }
                else
                {
                    Equipped_tool = null;
                }

                // Events
                if (IsPlayer)
                {
                    Game_events.Player_character_changed_selected_item.Invoke(Selected_item_index, Inventory[Selected_item_index].GetComponent<Item_behaviour>());
                }
            }
        }

        internal void Drop_selected_item_from_inventory()
        {
            if (Inventory.Count > 0 && Selected_item_index < Inventory.Count && Inventory[Selected_item_index] != null)
            {
                //Drop the item
                Inventory[Selected_item_index].transform.position = transform.position;
                Inventory[Selected_item_index].GetComponent<Item_behaviour>().Set_dropped_settings();

                Inventory.RemoveAt(Selected_item_index);
                Equipped_tool = null;

                //Events
                if (IsPlayer)
                {
                    Game_events.Player_character_dropped_item.Invoke();
                }
            }
        }

        internal void Interact()
        {
            foreach (var other_character in Characters_nearby)
            {
                if (other_character.TryGetComponent<Shopper_controller>(out Shopper_controller shopper_ctrl))
                {
                    shopper_ctrl.Call_shop();

                    break;
                }
            }
        }

        internal void Fix_character()
        {
            for (int i = 0; i < Inventory.Count; i++)
            {
                if (Inventory[i] == null)
                {
                    Inventory.RemoveAt(i);
                }
            }
        }

        /// AUX METHODS
        private IEnumerator Regens()
        {
            while (IsAlive)
            {
                yield return new WaitForSeconds(1f);

                Regenerate(Max_health * 0.03f); // Regenerate 3% of max health per second
            }
        }

        private IEnumerator Detect_items_nearby()
        {
            while (detectionRadius > 0)
            {
                yield return new WaitForSeconds(0.1f);

                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
                List<Item_behaviour> detectedItems = new List<Item_behaviour>();

                foreach (var hit in hits)
                {
                    Item_behaviour item = hit.GetComponentInParent<Item_behaviour>();

                    if (item != null)
                    {
                        detectedItems.Add(item);

                        // Add
                        if (!Items_nearby.Contains(item))
                        {
                            Items_nearby.Add(item);
                        }
                    }
                }

                //Remove items that are no longer detected
                for (int i = Items_nearby.Count - 1; i >= 0; i--)
                {
                    if (!detectedItems.Contains(Items_nearby[i]))
                    {
                        Items_nearby.RemoveAt(i);
                    }
                }
            }
        }

        private IEnumerator Detect_objects_nearby()
        {
            while (detectionRadius > 0)
            {
                yield return new WaitForSeconds(0.1f);

                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
                List<Object_behaviour> detectedObjects = new List<Object_behaviour>();

                foreach (var hit in hits)
                {
                    Object_behaviour obj = hit.GetComponentInParent<Object_behaviour>();
                    if (obj != null)
                    {
                        detectedObjects.Add(obj);

                        // Add
                        if (!Objects_nearby.Contains(obj))
                        {
                            Objects_nearby.Add(obj);
                        }
                    }
                }

                //Remove objects that are no longer detected
                for (int i = Objects_nearby.Count - 1; i >= 0; i--)
                {
                    if (!detectedObjects.Contains(Objects_nearby[i]))
                    {
                        Objects_nearby.RemoveAt(i);
                    }
                }
            }
        }

        private IEnumerator Detect_characters_nearby()
        {
            while (detectionRadius > 0)
            {
                yield return new WaitForSeconds(0.1f);

                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
                List<Character_behaviour> detectedCharacters = new List<Character_behaviour>();

                foreach (var hit in hits)
                {
                    Character_behaviour character = hit.GetComponentInParent<Character_behaviour>();
                    if (character != null && character != this) // Avoid self
                    {
                        detectedCharacters.Add(character);

                        // Add
                        if (!Characters_nearby.Contains(character))
                        {
                            Characters_nearby.Add(character);
                        }
                    }
                }

                //Remove characters that are no longer detected
                for (int i = Characters_nearby.Count - 1; i >= 0; i--)
                {
                    if (!detectedCharacters.Contains(Characters_nearby[i]))
                    {
                        Characters_nearby.RemoveAt(i);
                    }
                }
            }
        }

        private IEnumerator Animator_controller()
        {
            while (Anim != null)
            {
                yield return new WaitForSeconds(0.1f);

                Anim.SetBool("IsMoving", IsMoving);
                Anim.SetBool("IsUsingTool", IsPlowing || IsMining || IsCutting || IsAttacking);
                if (Equipped_tool != null)
                {
                    Anim.SetFloat("Tool_index", (int)Equipped_tool.Type);
                }
                else
                {
                    Anim.SetFloat("Tool_index", 0);
                }

                Anim.SetFloat("X_dir", Moving_dir.x);
                Anim.SetFloat("Y_dir", Moving_dir.y);

                Render.flipX = (Moving_dir.x > 0);
            }
        }

        private IEnumerator Detect_grid()
        {
            while (detectionRadius > 0)
            {
                yield return new WaitForSeconds(0.1f);

                // Detect grids within the detection radius
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
                Grid_controller foundGrid = null;

                foreach (var hit in hits)
                {
                    if (hit != null)
                    {
                        Grid_controller grid = hit.GetComponent<Grid_controller>();

                        if (grid != null)
                        {
                            foundGrid = grid;
                            break;
                        }
                    }
                }

                Current_grid = foundGrid;
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}