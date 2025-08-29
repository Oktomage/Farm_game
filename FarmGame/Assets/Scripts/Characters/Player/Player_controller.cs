using UnityEngine;

namespace Game.Characters.Player
{
    public class Player_controller : MonoBehaviour
    {
        [Header("Data")]
        public Character_scriptable characterData;

        [Header("Components")]
        public Character_behaviour Character;

        private void Awake()
        {
            Character.IsPlayer = true;
        }

        private void Start()
        {
            Character.Configure(characterData);
        }

        private void Update()
        {
            Read_inputs();
        }

        /// MAIN METHODS
        private void Read_inputs()
        {
            // Movement functions
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                Ask_move(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0));
            }

            // Interaction functions
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Ask_collet();
                Ask_interact(key: KeyCode.E);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                Ask_interact(key: KeyCode.R);
            }

            // Actions functions
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Character.Use_tool();
            }

            // Inventory functions
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Character.Select_item_from_inventory(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Character.Select_item_from_inventory(1);
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") != 0f)
            {
                float scroll_value = Mathf.Sign(Input.GetAxisRaw("Mouse ScrollWheel"));

                Character.Select_item_from_inventory(Character.Selected_item_index + (int)scroll_value);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Character.Drop_selected_item_from_inventory();
            }
        }

        private void Ask_move(Vector3 dir)
        {
            Character.Move(dir);
        }

        private void Ask_collet()
        {
            if (Character.Items_nearby.Count > 0)
            {
                Character.Collect();
            }
        }

        private void Ask_interact(KeyCode key)
        {
            if(Character.Characters_nearby.Count > 0)
            {
                Character.Interact(key);
            }
            else if (Character.Objects_nearby.Count > 0)
            {
                Character.Interact(key);
            }
        }
    }
}