using Game.Characters;
using Game.Grids;
using Game.Items.Weapons;
using Game.Objects;
using Game.Objects.Entitys;
using Game.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items.Tools
{
    [System.Serializable]
    public class Tool_behaviour : MonoBehaviour
    {
        public enum ToolType
        {
            Axe,
            Pickaxe,
            Hoe,
            WateringCan,
            Scythe,
            Hammer,
            Sword,
            Seeds,
            Staff,
            Bow
        }

        [Header("Data")]
        internal Character_behaviour Character;
        internal Item_behaviour Item => this.gameObject.GetComponent<Item_behaviour>();

        [Header("Tool Settings")]
        public ToolType Type = ToolType.Hoe;

        /// CORE METHODS
        internal void Set_toolType (ToolType type)
        {
            // Set
            Type = type;
        }

        /// MAIN METHODS
        internal void Use(Character_behaviour character, Vector2 dir)
        {
            //Set
            Character = character;

            switch (Type)
            {
                case ToolType.Axe:
                    Cut(dir);
                    break;

                case ToolType.Pickaxe:
                    Mine(dir);
                    break;

                case ToolType.Hoe:
                    Plow(Character.Current_grid, dir);
                    break;

                case ToolType.WateringCan:
                    break;

                case ToolType.Scythe:
                    Harvest();
                    break;

                case ToolType.Hammer:
                    break;

                case ToolType.Sword:
                    Melee_attack(dir);
                    break;

                case ToolType.Seeds:
                    Plant();
                    break;

                case ToolType.Staff:
                    Magic_attack(dir);
                    break;

                case ToolType.Bow:
                    Fire_bow(dir);
                    break;
            }
        }

        /// TOOL ACTIONS METHODS
        private void Cut(Vector2 dir)
        {
            if(Character.Objects_nearby.Count > 0)
            {
                //Search for a tree in the nearby objects
                Tree_entity tree = null;

                foreach (Object_behaviour obj in Character.Objects_nearby.ToArray())
                {
                    if (obj.TryGetComponent<Tree_entity>(out Tree_entity found_tree))
                    {
                        tree = found_tree;
                        break;
                    }
                }

                if(tree != null)
                {
                    tree.Take_damage(1f);

                    // Set character state
                    Character.IsCutting = true;
                    Character.Swing_hand(dir);

                    StartCoroutine(Action_time(0.3f));

                    // Audio
                    Game_utils.Instance.Create_sound("Axe_sound", "Audios/Tools/Axe_1", Character.transform.position);
                }
            }
        }

        private void Mine(Vector2 dir)
        {
            if (Character.Objects_nearby.Count > 0)
            {
                //Search for a stone in the nearby objects
                Stone_entity stone = null;

                foreach(Object_behaviour obj in Character.Objects_nearby.ToArray())
                {
                    if (obj.TryGetComponent<Stone_entity>(out Stone_entity found_stone))
                    {
                        // Set
                        stone = found_stone;
                        break;
                    }
                }

                if(stone != null)
                {
                    stone.Take_damage(1f);

                    // Set character state
                    Character.IsMining = true;
                    Character.Swing_hand(dir);

                    StartCoroutine(Action_time(0.3f));

                    // Audio
                    Game_utils.Instance.Create_sound("Pickaxe_sound", "Audios/Tools/Pickaxe_1", Character.transform.position);
                }
            }
        }

        private void Melee_attack(Vector2 dir)
        {
            if (Character.IsAttacking)
                return;

            Sword sword = this.gameObject.GetComponent<Sword>();
            sword.Attack(dir);

            // Set character state
            Character.IsAttacking = true;
            Character.Swing_hand(dir);

            StartCoroutine(Action_time(0.3f));
        }

        private void Fire_bow(Vector2 dir)
        {
            if (Character.IsAttacking)
                return;

            // Fire
            Bow bow = this.gameObject.GetComponent<Bow>();
            bow.Fire(dir);

            // Set character state
            Character.IsAttacking = true;

            StartCoroutine(Action_time(0.3f));

            // Audio
            Game_utils.Instance.Create_sound("Sword_sound", "Audios/Tools/Bow_1", Character.transform.position);
        }

        private void Magic_attack(Vector2 dir)
        {
            if (Character.IsAttacking)
                return;

            // Fire
            Magic_wand wand = this.gameObject.GetComponent<Magic_wand>();
            wand.Fire(dir);

            // Set character state
            Character.IsAttacking = true;
            Character.Swing_hand(dir);

            StartCoroutine(Action_time(0.3f));

            // Audio
            Game_utils.Instance.Create_sound("Sword_sound", "Audios/Spells/Fireball_1", Character.transform.position);
        }

        private void Plow(Grid_controller grid, Vector2 dir)
        {
            if (grid == null) return;

            // Get the grid position based on the character's position
            Vector2 character_world_grid_position = new Vector2(Mathf.FloorToInt(Character.transform.position.x), Mathf.FloorToInt(Character.transform.position.y));

            //Set
            grid.Change_local_grid(character_world_grid_position, Grid_controller.Cell.CellType.Plowed_land);

            //Set character state
            Character.IsPlowing = true;

            StartCoroutine(Action_time(0.3f));

            //Audio
            Game_utils.Instance.Create_sound("Hoe_sound", "Audios/Tools/Hoe_1", Character.transform.position);
        }

        private void Plant()
        {
            if(Character.Objects_nearby.Count > 0)
            {
                // Get
                this.gameObject.TryGetComponent<Seeds_bag>(out Seeds_bag seeds_bag);

                if (seeds_bag == null)
                    return;

                List<Plantable_spot> spots_list = new List<Plantable_spot>();

                // Get plantable spots from nearby objects
                foreach (var obj in Character.Objects_nearby)
                {
                    if(obj.TryGetComponent<Plantable_spot>(out Plantable_spot plantable_spot))
                    {
                        //Check if the spot is not occupied
                        if(!plantable_spot.Is_occupied)
                        {
                            spots_list.Add(plantable_spot);
                        }
                    }
                }

                // Use seeds bag
                int plant_ammount = 1;

                seeds_bag.Plant_seeds(plant_ammount, spots_list);
            }
        }

        private void Harvest()
        {
            if (Character.Objects_nearby.Count > 0)
            {
                List<Plantable_spot> spots_list = new List<Plantable_spot>();

                //Get filled spots from nearby objects
                foreach (var obj in Character.Objects_nearby)
                {
                    if (obj.TryGetComponent<Plantable_spot>(out Plantable_spot plantable_spot))
                    {
                        //Check if the spot is not occupied
                        if (plantable_spot.Is_occupied)
                        {
                            spots_list.Add(plantable_spot);
                        }
                    }
                }

                // Use scythe
                foreach(Plantable_spot spot in spots_list.ToArray())
                {
                    spot.Harvest_crop();
                }
            }
        }

        private IEnumerator Action_time(float time)
        {
            Character.IsUsingTool = true;

            yield return new WaitForSeconds(time);

            Character.IsUsingTool = false;

            Character.IsPlowing = false;
            Character.IsWatering = false;
            Character.IsAttacking = false;
            Character.IsHarvesting = false;
            Character.IsMining = false;
            Character.IsCutting = false;
        }
    }
}