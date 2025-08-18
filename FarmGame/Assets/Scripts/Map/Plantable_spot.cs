using Game.Crops;
using Game.Utils;
using UnityEngine;

namespace Game.Objects
{
    public class Plantable_spot : MonoBehaviour
    {
        [Header("State")]
        public bool Is_occupied = false;

        [Header("Components")]
        public GameObject Local_crop;

        /// MAIN METHODS
        internal void Plant_crop(Crop_scriptable crop_scripable)
        {
            if (Is_occupied)
            {
                Debug.LogWarning("A crop is already planted here.");
                return;
            }

            //Set
            GameObject crop = Game_utils.Instance.Create_prefab_from_resources("Prefabs/Objects/Crop_object");

            Local_crop = crop;
            Local_crop.transform.SetParent(transform);
            Local_crop.transform.localPosition = Vector3.zero;

            crop.GetComponent<Crop>().Configure_crop(crop_scripable);

            Is_occupied = true;
        }

        internal void Harvest_crop()
        {
            if (Local_crop != null)
                Local_crop.GetComponent<Crop>().Harvest();

            if (Local_crop == null)
                Is_occupied = false;
        }
    }
}