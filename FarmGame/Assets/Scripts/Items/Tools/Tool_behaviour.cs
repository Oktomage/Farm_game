using Game.Characters;
using Game.Grids;
using Game.Objects;
using Game.Objects.Entitys;
using Game.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items.Tools
{
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
            Seeds
        }

        [Header("Data")]
        internal Character_behaviour Character;

        [Header("Tool Settings")]
        public ToolType Type = ToolType.Hoe;

        public void Use(Character_behaviour character)
        {
            //Set
            Character = character;

            switch (Type)
            {
                case ToolType.Axe:
                    Cut();
                    break;

                case ToolType.Pickaxe:
                    Mine();
                    break;

                case ToolType.Hoe:
                    Plow(Character.Current_grid);
                    break;

                case ToolType.WateringCan:
                    break;

                case ToolType.Scythe:
                    Harvest();
                    break;

                case ToolType.Hammer:
                    break;

                case ToolType.Sword:
                    Attack();
                    break;

                case ToolType.Seeds:
                    Plant();
                    break;
            }
        }

        /// TOOL ACTIONS METHODS
        private void Cut()
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

                    //Set character state
                    Character.IsCutting = true;

                    StartCoroutine(Action_time(0.3f));

                    //Audio
                    Game_utils.Instance.Create_sound("Axe_sound", "Audios/Tools/Axe_1", Character.transform.position);
                }
            }
        }

        private void Mine()
        {
            if (Character.Objects_nearby.Count > 0)
            {
                //Search for a stone in the nearby objects
                Stone_entity stone = null;

                foreach(Object_behaviour obj in Character.Objects_nearby.ToArray())
                {
                    if (obj.TryGetComponent<Stone_entity>(out Stone_entity found_stone))
                    {
                        stone = found_stone;
                        break;
                    }
                }

                if(stone != null)
                {
                    stone.Take_damage(1f);

                    //Set character state
                    Character.IsMining = true;

                    StartCoroutine(Action_time(0.3f));

                    //Audio
                    Game_utils.Instance.Create_sound("Pickaxe_sound", "Audios/Tools/Pickaxe_1", Character.transform.position);
                }
            }
        }

        private void Attack()
        {
            if (Character.Characters_nearby.Count > 0)
            {
                //Get character to attack
                //Character_behaviour other_character = null;

                foreach (Character_behaviour other_char in Character.Characters_nearby.ToArray())
                {
                    Character.Hit_other_entity(other_char.gameObject);

                    //Set character state
                    Character.IsAttacking = true;

                    StartCoroutine(Action_time(0.3f));

                    //Audio
                    Game_utils.Instance.Create_sound("Sword_sound", "Audios/Tools/Sword_1", Character.transform.position);

                    break;
                }
            }
        }

        private void Plow(Grid_controller grid)
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
                List<Plantable_spot> spots_list = new List<Plantable_spot>();

                //Get plantable spots from nearby objects
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
                if(this.gameObject.TryGetComponent<Seeds_bag>(out Seeds_bag seeds_bag))
                {
                    int plant_ammount = 1;

                    //Plant seeds
                    seeds_bag.Plant_seeds(plant_ammount, spots_list);
                }
                else
                {
                    Debug.LogError("Cant find seeds bag");
                }
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