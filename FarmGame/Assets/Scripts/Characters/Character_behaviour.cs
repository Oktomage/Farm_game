using Game.Characters.Data;
using Game.Characters.Shopper;
using Game.Characters.Spells;
using Game.Crafting;
using Game.Effects;
using Game.Events;
using Game.Grids;
using Game.Items;
using Game.Items.Tools;
using Game.Magic;
using Game.Objects;
using Game.Objects.Cristal;
using Game.UI.Notifications;
using Game.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Game.Characters
{
    public class Character_behaviour : MonoBehaviour
    {
        [Header("Character Settings")]
        public string Name;
        public float Health = 5f;
        public float Max_health = 5f;
        public float MoveSpeed = 5f;
        public float AttackDamage = 1f;
        public float detectionRadius = 1.5f;

        [Space]
        public int Souls = 1;

        [Space]
        internal int Combat_level = 0;
        internal int Magic_level = 0;
        internal int Farming_level = 0;
        internal int Mining_level = 0;
        internal int Harvest_level = 0;

        [Space]
        public bool IsExtraStrong;
        public bool IsExtraFast;
        public bool HaveMagicalResistance;
        public bool HavePhysicalResistance;

        [Space]
        internal bool CanUseMagic;
        internal List<Spell_scriptable> Spells = new List<Spell_scriptable>();

        public enum Character_stances
        {
            Idle,
            Neutral,
            Agressive
        }

        [Header("States")]
        public Character_stances Current_stance;

        [Space]
        public bool IsPlayer = false;
        public bool IsAlly = false;
        public bool IsEnemy = false;
        public bool IsBoss = false;
        public bool IsEnraged = false;

        [Space]
        internal bool IsUsingTool = false;
        internal bool IsUsingSpell = false;
        internal bool IsAlive = true;
        internal bool IsMoving = false;
        internal bool IsPlowing = false;
        internal bool IsWatering = false;
        internal bool IsAttacking = false;
        internal bool IsHarvesting = false;
        internal bool IsMining = false;
        internal bool IsCutting = false;

        [Header("Inventory")]
        internal int InventorySize = 2;
        public List<GameObject> Inventory = new List<GameObject>();
        [SerializeField] internal Tool_behaviour Equipped_tool;
        internal int Selected_item_index = 0;

        [Header("Audio settings")]
        public AudioClip Idle_sound;
        public AudioClip Move_sound;
        public AudioClip Attack_sound;
        public AudioClip Hurt_sound;
        public AudioClip Death_sound;

        [Header("Components")]
        public SpriteRenderer Render => this.gameObject.GetComponentInChildren<SpriteRenderer>();
        public Animator Anim => this.gameObject.GetComponentInChildren<Animator>();
        public Rigidbody2D Rigid => this.gameObject.GetComponent<Rigidbody2D>();
        public Collider2D Collider => this.gameObject.GetComponentInChildren<Collider2D>();
        public Character_spells_controller character_spells_controller => this.gameObject.GetComponent<Character_spells_controller>();
        [SerializeField] internal Transform Hand_pivot;
        internal GameObject Swing_shadow_obj;
        internal TrailRenderer Swing_trail;

        [Space(30)]
        //Internal variables
        [SerializeField] internal bool IsGodMode = false; // For testing purposes

        [SerializeField] internal List<Item_behaviour> Items_nearby = new List<Item_behaviour>();
        [SerializeField] internal Grid_controller Current_grid;
        internal Vector3 Moving_dir = Vector3.zero;
        [SerializeField] internal List<Object_behaviour> Objects_nearby = new List<Object_behaviour>();
        [SerializeField] internal List<Character_behaviour> Characters_nearby = new List<Character_behaviour>();
        [SerializeField] internal GameObject Last_damaged_by_obj;
        internal Vector2 Hand_grip_offset => new Vector2(0.5f * transform.localScale.x, 0);

        private void Awake()
        {
#if UNITY_EDITOR
            if (IsPlayer)
                Enable_editor_settings();
#endif

            if(IsEnemy)
            {
                Game_events.New_day.AddListener(Enrage);
            }
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

        private void Recieve_extra_bonuses()
        {
            if (IsExtraStrong)
                AttackDamage *= 1.3f;

            if (IsExtraFast)
                MoveSpeed *= 1.3f;
        }

        internal void Configure(Character_scriptable character_scriptable)
        {
            // Set
            transform.localScale *= character_scriptable.Size_multiplier;

            Name = character_scriptable.Name;

            Max_health = character_scriptable.Max_health;
            Health = Max_health;
            MoveSpeed = character_scriptable.Move_speed;
            AttackDamage = character_scriptable.Damage;

            IsExtraStrong = character_scriptable.IsExtraStrong;
            IsExtraFast = character_scriptable.IsExtraFast;
            HaveMagicalResistance = character_scriptable.HaveMagicalResistance;
            HavePhysicalResistance = character_scriptable.HavePhysicalResistance;
            Recieve_extra_bonuses();

            CanUseMagic = character_scriptable.CanUseMagic;

            foreach(Spell_scriptable spell in character_scriptable.Spells)
            {
                Spells.Add(spell);
            }

            Current_stance = Character_stances.Idle;

            InventorySize = 2;
            Inventory = new List<GameObject>(new GameObject[InventorySize]);

            Souls = character_scriptable.Souls_reward;

            IsBoss = character_scriptable.IsBoss;

            // Audios
            Idle_sound = character_scriptable.Idle_sound;
            Move_sound = character_scriptable.Move_sound;
            Attack_sound = character_scriptable.Attack_sound;
            Hurt_sound = character_scriptable.Hurt_sound;
            Death_sound = character_scriptable.Death_sound;

            // Hand
            Hand_pivot = Game_utils.Instance.Create_gameObject(this.gameObject).transform;
            Hand_pivot.name = "Hand_pivot";

            Swing_shadow_obj = Game_utils.Instance.Create_gameObject(Hand_pivot.gameObject);
            Swing_shadow_obj.name = "Swing_shadow";
            Swing_shadow_obj.transform.localPosition = Hand_grip_offset;

            Swing_trail = Swing_shadow_obj.AddComponent<TrailRenderer>();

            Swing_trail.time = 0.2f;
            Swing_trail.widthCurve = new AnimationCurve(
                new Keyframe(0f, 0.5f),   // Início: tempo 0, valor 0.2
                new Keyframe(1f, 0f)      // Fim: tempo 1, valor 0
            );
            Swing_trail.material = Game_utils.Instance.Get_material("Materials/Default_material");
            Swing_trail.colorGradient = new Gradient
            {
                colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.white, 1f)
                },
                alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(0.3f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            };
            Swing_trail.sortingOrder = 2;

            Swing_trail.emitting = false;

            // Routines
            StartCoroutine(Regens());
            StartCoroutine(Detect_items_nearby());
            StartCoroutine(Animator_controller());
            StartCoroutine(Detect_grid());
            StartCoroutine(Detect_objects_nearby());
            StartCoroutine(Detect_characters_nearby());
            StartCoroutine(Idle_actions());
        }

        private void Die()
        {
            if (IsGodMode)
                return;

            // Set
            IsAlive = false;

            if (Last_damaged_by_obj != null && Last_damaged_by_obj.CompareTag("Player"))
            {
                Player_data.Instance.Add_souls(Souls);

                // Events
                Game_events.Player_collected_souls.Invoke(Souls);
                Game_events.Player_character_killed_enemy.Invoke(this.gameObject);
            }

            // Audio
            Game_utils.Instance.Create_sound("Character_hurt", Death_sound, transform.position);

            // Particles
            Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Death_souls", transform.position);

            Destroy(this.gameObject);

            if(IsPlayer)
                Application.Quit();
        }

        internal void TakeDamage(float dmg, GameObject attacker)
        {
            if (!IsAlive)
                return;

            // Set last damaged by
            if (attacker != null)
                Last_damaged_by_obj = attacker;

            // Set
            Health -= dmg;
            Health = Mathf.Clamp(Health, 0, Max_health);

            if (Health <= 0)
                Die();

            // Events
            if (IsPlayer)
                Game_events.Player_character_took_damage.Invoke(dmg, Last_damaged_by_obj);
            else
                Game_events.Enemy_took_damage.Invoke(dmg, this.gameObject);

            // Effects
            if (this.gameObject.TryGetComponent<Effects_controller>(out Effects_controller effects))
            {
                effects.Force_effect(Effects_controller.EffectType.Boing);
                effects.Force_effect(Effects_controller.EffectType.Flash);
            }

            // Audio
            Game_utils.Instance.Create_sound("Character_hurt", Hurt_sound, transform.position);
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

        internal void Set_stance(Character_stances stance)
        {
            // Set
            Current_stance = stance;
        }

        private void Enrage()
        {
            if (IsEnraged)
                return;

            // Set
            IsEnraged = true;

            Max_health *= 1.5f;
            MoveSpeed *= 2f;
            AttackDamage *= 2f;
        }

        ///  ACTIONS METHODS
        internal void Move(Vector3 dir)
        {
            if(IsUsingTool) 
                return;

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

                // Effects
                t += Time.deltaTime;

                if (t > 0.2f)
                {
                    t = 0;

                    //Particles
                    Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Walk_grass", transform.position);

                    //Audio
                    Game_utils.Instance.Create_sound("Move_sound", Move_sound, this.transform.position);
                }

                yield return null;
            }

            // Clamp the position to the target position to avoid overshooting
            transform.position = targetPosition;

            IsMoving = false;
        }

        internal void Collect()
        {
            if (Inventory.Count < InventorySize)
            {
                // Get closest item
                GameObject closest_item = Game_utils.Instance.Get_closest_obj(transform.position, Items_nearby.Select(gm => gm.gameObject).ToArray());
                closest_item.TryGetComponent<Item_behaviour>(out Item_behaviour item_bhv);

                if (!item_bhv.IsCollectable)
                    return;

                if (item_bhv == null) 
                    return;

                // Add
                Inventory.Add(item_bhv.gameObject);

                item_bhv.Set_collected_settings(character: this);

                // Update equipped item
                Select_item_from_inventory(Selected_item_index);

                // Events
                if (IsPlayer)
                {
                    Notifications_controller.Instance.Set_notification_task(new Notifications_controller.Notification
                    {
                        Title = item_bhv.ItemData.ItemName,
                        Message = $"You collected {item_bhv.ItemData.ItemName}.",
                        Duration = 1f,
                        Icon = item_bhv.ItemData.Icon
                    });

                    Game_events.Player_character_collected_item.Invoke();
                }
            }
        }

        internal void Use_tool()
        {
            if(IsUsingTool) 
                return;

            if (Equipped_tool == null)
                return;

            if (IsPlayer)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mouseDir = (mousePos - new Vector2(transform.position.x, transform.position.y)).normalized;

                Equipped_tool.Use(this, mouseDir);
            }
            else
            {
                Equipped_tool.Use(this, dir: Vector2.zero);
            }
        }

        internal void Hit_other_entity(GameObject obj)
        {
            // Check if the object is a character
            obj.TryGetComponent<Character_behaviour>(out Character_behaviour other_character);

            if(other_character != null)
                other_character.TakeDamage(AttackDamage, this.gameObject);
        }

        internal void Select_item_from_inventory(int id)
        {
            if (Inventory.Count > 0)
            {
                // Set
                Selected_item_index = Mathf.Clamp(id, 0, Inventory.Count - 1);
                
                if (Inventory[Selected_item_index] == null)
                    Equipped_tool = null;

                // Check for tool
                Inventory[Selected_item_index].TryGetComponent<Tool_behaviour>(out Tool_behaviour tool);

                if (tool == null)
                    Equipped_tool = null;
                else
                    Equipped_tool = tool;

                // Events
                if (IsPlayer)
                    Game_events.Player_character_changed_selected_item.Invoke(Selected_item_index, Inventory[Selected_item_index].GetComponent<Item_behaviour>());
            }
        }

        internal void Drop_selected_item_from_inventory()
        {
            if (Inventory.Count > 0 && Selected_item_index < Inventory.Count && Inventory[Selected_item_index] != null)
            {
                // Drop the item
                Inventory[Selected_item_index].transform.position = transform.position;
                Inventory[Selected_item_index].GetComponent<Item_behaviour>().Set_dropped_settings();

                Inventory.RemoveAt(Selected_item_index);

                Equipped_tool = null;

                // Update equipped item
                Select_item_from_inventory(Selected_item_index);

                // Events
                if (IsPlayer)
                {
                    Game_events.Player_character_dropped_item.Invoke();
                }
            }
        }

        internal void Interact(KeyCode key)
        {
            switch(key)
            {
                case KeyCode.E:
                    foreach (var other_character in Characters_nearby)
                    {
                        if (other_character.TryGetComponent<Shopper_controller>(out Shopper_controller shopper_ctrl))
                        {
                            shopper_ctrl.Call_shop();

                            break;
                        }
                    }

                    foreach (var other_objects in Objects_nearby)
                    {
                        // Cristal
                        if (other_objects.TryGetComponent<Cristal_controller>(out Cristal_controller cristal_ctrl))
                        {
                            if (Inventory.Count > 0)
                                cristal_ctrl.Trade(Inventory[Selected_item_index]);
                        }

                        // Workbench
                        if (other_objects.TryGetComponent<Workbench_controller>(out Workbench_controller workbench_ctrl))
                        {
                            if (Inventory.Count > 0)
                                workbench_ctrl.Put_item(Inventory[Selected_item_index], this);
                        }

                        // Furnace
                        if (other_objects.TryGetComponent<Furnace_controller>(out Furnace_controller furnace_ctrl))
                        {
                            if (Inventory.Count > 0)
                                furnace_ctrl.Put_item(Inventory[Selected_item_index], this);
                        }
                    }
                    break;

                case KeyCode.R:
                    foreach (var other_objects in Objects_nearby)
                    {
                        // Workbench
                        if (other_objects.TryGetComponent<Workbench_controller>(out Workbench_controller workbench_ctrl))
                        {
                            workbench_ctrl.Craft(this);
                        }
                    }
                    break;
            }
        }

        internal void Swing_hand(Vector2 dir)
        {
            dir.Normalize();
            float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            StartCoroutine(Swing_routine(baseAngle));

            // Audio
            Game_utils.Instance.Create_sound("Swing_sound", "Audios/Tools/Swing_1", transform.position);
        }

        internal void Cast_spell(Spell_scriptable spell, GameObject target_obj)
        {
            if (!CanUseMagic)
                return;

            if (IsAttacking || IsUsingSpell)
                return;

            // Set
            IsUsingSpell = true;

            // Cast
            character_spells_controller.Cast_spell(new Spells.Character_spells_controller.Cast_spell_data { SpellData = spell, Origin_pos = this.transform.position, Target_obj = target_obj });
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
        private IEnumerator Idle_actions()
        {
            while(IsAlive)
            {
                yield return new WaitForSeconds(Random.Range(10f, 30f));

                float c = Random.value;

                if(c <= 0.5f)
                {
                    // Audio
                    Game_utils.Instance.Create_sound("Idle_sound", Idle_sound, transform.position);
                }
            }
        }

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
                List<Character_behaviour> detected_Characters = new List<Character_behaviour>();

                foreach (var hit in hits)
                {
                    Character_behaviour character = hit.GetComponentInParent<Character_behaviour>();
                    
                    if (character != null && character != this) // Avoid self
                    {
                        detected_Characters.Add(character);

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
                    if (!detected_Characters.Contains(Characters_nearby[i]))
                    {
                        Characters_nearby.RemoveAt(i);
                    }
                }
            }
        }

        private IEnumerator Animator_controller()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);

                if (Anim == null)
                    yield break;

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

        private IEnumerator Swing_routine(float baseAngle)
        {
            float swingArc = 170f;
            float swingDuration = 0.15f;

            float startAngle = baseAngle - swingArc * 0.5f;
            float endAngle = baseAngle + swingArc * 0.5f;

            // Set
            Swing_trail.emitting = true;

            float t = 0f;

            // Swing
            while (t < swingDuration)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / swingDuration);

                float current = Mathf.LerpAngle(startAngle, endAngle, k);
                Hand_pivot.localRotation = Quaternion.AngleAxis(current, Vector3.forward);

                yield return null;
            }

            // Set
            Swing_trail.emitting = false;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}