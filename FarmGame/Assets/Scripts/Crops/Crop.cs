using Game.Events;
using Game.Utils;
using UnityEngine;

namespace Game.Crops
{
    public class Crop : MonoBehaviour
    {
        public Crop_scriptable CropData;

        [Header("Settings")]
        public int GrowthStage_max = 0;
        public int GrowthStage = 0;

        [Header("Components")]
        public SpriteRenderer Render;
        public Collider2D Collider;

        [Header("States")]
        public bool IsGrowing = false;
        public bool IsReadyToHarvest = false;

        private void Awake()
        {
            //Events
            Game_events.New_day.AddListener(Grow);
        }

        /// CORE METHODS
        internal void Configure_crop(Crop_scriptable cropData)
        {
            //Set
            CropData = cropData;
        
            GrowthStage_max = CropData.Crop_stages.Count;
            GrowthStage = 0;

            IsGrowing = true;

            //Sprite
            Render.sprite = CropData.Crop_stages[GrowthStage];
        }

        private void Complete_growth()
        {
            //Set
            IsGrowing = false;
            IsReadyToHarvest = true;
        }

        /// MAIN METHODS
        private void Grow()
        {
            if(IsReadyToHarvest) { return; }

            //Set
            GrowthStage++;
            GrowthStage = Mathf.Clamp(GrowthStage, 0, GrowthStage_max - 1);

            if(GrowthStage >= GrowthStage_max - 1)
            {
                Complete_growth();
            }

            //Sprite
            Render.sprite = CropData.Crop_stages[GrowthStage];
        }

        internal void Harvest()
        {
            if(!IsReadyToHarvest) { return; }

            //Create item
            Game_utils.Instance.Create_item(CropData.Crop_item, this.transform.position);

            //Destroy crop
            Destroy(this.gameObject);
        }
    }
}