using Game.Effects;
using Game.Items;
using Game.Utils;
using UnityEngine;

namespace Game.Objects.Entitys
{
    public class Stone_entity : MonoBehaviour
    {
        public enum StoneSizes
        {
            Small,
            Medium,
            Large
        }

        public enum StoneTypes
        {
            Normal,
            Iron,
            Silver,
            Gold
        }

        [Header("Data")]
        public Object_behaviour Object;

        [Header("Settings")]
        public StoneSizes Size = StoneSizes.Small;
        public StoneTypes Type = StoneTypes.Normal;

        [Header("Data")]
        public float Health = 3;

        private void Start()
        {
            Object = this.gameObject.GetComponent<Object_behaviour>();
        }

        public void Take_damage(float dmg)
        {
            Health -= dmg;

            if(Health <= 0)
            {
                Break();
            }

            //Effects
            if(this.gameObject.TryGetComponent<Effects_controller>(out Effects_controller effectController))
            {
                effectController.Force_effect(Effects_controller.EffectType.Boing);
            }

            // Effects
            Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Stone_sparks", transform.position);
        }

        private void Drop_items()
        {
            if(Object.ObjectData.Item_drop == null) { return; }

            //Check drop chance & drop items
            for (int i = 0; i < Object.ObjectData.Drop_amount; i++)
            {
                //Create item
                GameObject item = Game_utils.Instance.Create_item(Object.ObjectData.Item_drop, this.transform.position);
            }
        }

        private void Break()
        {
            Drop_items();

            //Audio
            Game_utils.Instance.Create_sound("Stone_break_sound", "Audios/Objects/Rock_smash_1", this.transform.position);

            //Particles
            Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Small_stone_break", this.transform.position);

            Destroy(this.gameObject);
        }
    }
}
