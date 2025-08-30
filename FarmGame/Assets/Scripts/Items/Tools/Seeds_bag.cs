using Game.Crops;
using Game.Events;
using Game.Objects;
using Game.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Items.Tools
{
    [System.Serializable]
    public class Seeds_bag : MonoBehaviour
    {
        [Header("Data")]
        public Crop_scriptable CropData => Item_behaviour.ItemData.Crop;
        internal Item_behaviour Item_behaviour => this.gameObject.GetComponent<Item_behaviour>();
        internal Tool_behaviour Tool_behaviour => this.gameObject.GetComponent<Tool_behaviour>();

        [Header("Settings")]
        [Range(1, 16)]
        public int Bag_capacity = 4;

        [Header("State")]
        public int Seed_ammount = 0;

        private void Start()
        {
            if(CropData != null)
                Configure_bag();
        }

        /// CORE METHODS
        private void Configure_bag()
        {
            //Set
            Seed_ammount = Bag_capacity;
        }

        internal void Delete_bag()
        {
            // Remove item from character inventory
            Item_behaviour.Delete_item();

            // Events
            if (Tool_behaviour.Character != null && Tool_behaviour.Character.IsPlayer)
            {
                Game_events.Player_character_used_item.Invoke();
            }

            Destroy(this.gameObject);
        }

        /// MAIN METHODS
        internal void Plant_seeds(int ammount, List<Plantable_spot> spots)
        {
            if(CropData == null) { return; }
            if(Seed_ammount <= 0) { Delete_bag(); return; }

            for (int i = 0; i < ammount; i++)
            {
                // Plant
                if(spots.Count > 0)
                {
                    Seed_ammount--;

                    spots[i].Plant_crop(CropData);

                    // Audio
                    Game_utils.Instance.Create_sound("Hoe_sound", "Audios/Tools/Plant_1", spots[i].transform.position);

                    // Effects
                    Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Seed_sparks", spots[i].transform.position);
                }

                if (Seed_ammount <= 0)
                {
                    Delete_bag();
                    return;
                }
            }
        }
    }
}