using Game.Characters;
using Game.Events;
using Game.Items;
using Game.Utils;
using Game.Utils.Misc;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game.Crafting
{
    public class Furnace_controller : MonoBehaviour
    {
        [Header("State")]
        public GameObject Current_material_inside;

        [Header("Components")]
        public Detector_manager Detector => this.gameObject.GetComponent<Detector_manager>();
        public Light2D Light => this.gameObject.GetComponentInChildren<Light2D>();

        //Internal variables
        internal GameObject Player_character_obj => GameObject.FindGameObjectWithTag("Player");
        internal GameObject Smoke_particle_obj;
        internal GameObject Furnace_sound_obj;

        /// CORE METHODS
        private void Clear_furnace()
        {
            // Clean
            Current_material_inside = null;

            Light.enabled = false;

            if (Smoke_particle_obj != null )
                Destroy(Smoke_particle_obj);

            if(Furnace_sound_obj != null)
                Destroy(Furnace_sound_obj);
        }

        /// MAIN METHODS
        internal void Put_item(GameObject item_obj, Character_behaviour character)
        {
            // Checks
            if (Current_material_inside != null)
                return;

            if (item_obj == null)
                return;

            item_obj.TryGetComponent<Item_behaviour>(out Item_behaviour item_bhv);

            if (!item_bhv.ItemData.Can_smelt)
                return;

            // Set
            character.Drop_selected_item_from_inventory();

            Current_material_inside = item_obj;
            item_bhv.Set_collected_settings();

            // Smelt
            StartCoroutine(Smelt());

            if (character.IsPlayer)
            {
                //StartCoroutine(Read_detector_state());

                // Events
                //Game_events.Player_character_used_workbench.Invoke(this.gameObject);
            }
        }

        private IEnumerator Smelt()
        {
            if (Current_material_inside == null)
                yield break;

            // Get
            Current_material_inside.TryGetComponent<Item_behaviour>(out Item_behaviour item_bhv);

            // Audio & Effects
            Light.enabled = true;
            Furnace_sound_obj = Game_utils.Instance.Create_sound("Furnace_sound", "Audios/Objects/Furnace_1", transform.position);
            Smoke_particle_obj = Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Furnace_smoke", new Vector2(transform.position.x, transform.position.y + 0.5f));

            yield return new WaitForSeconds(item_bhv.ItemData.Smelt_time);

            // Get
            Item_scriptable smelt_result = item_bhv.ItemData.Smelt_result;

            if (smelt_result != null)
            {
                // Create
                GameObject craft_obj = Game_utils.Instance.Create_item(smelt_result, this.gameObject.transform.position);

                // Events
                Game_events.Furnace_smelt_done.Invoke();

                // Audio
                Game_utils.Instance.Create_sound("Craft_sound", "Audios/Objects/Anvil_1", transform.position);

                // Effects
                Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Craft_sparks", transform.position);
            }

            // Free furnace
            Clear_furnace();
        }

        /*
        private IEnumerator Read_detector_state()
        {
            while (Crafting_UI_visible)
            {
                yield return new WaitForSeconds(0.1f);
                if (Player_character_obj == null) { continue; }

                // Out of range
                if (!Detector.Player_in_range)
                {
                    if (Crafting_UI_visible)
                    {
                        Crafting_UI_visible = false;

                        //Events
                        Game_events.Player_character_closed_workbench.Invoke();
                    }
                }
            }
        }*/
    }
}