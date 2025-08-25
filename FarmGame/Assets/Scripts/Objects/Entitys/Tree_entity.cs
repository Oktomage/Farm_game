using Game.Effects;
using Game.Utils;
using System.Collections;
using UnityEngine;

namespace Game.Objects.Entitys
{
    public class Tree_entity : MonoBehaviour
    {
        public enum Tree_Sizes
        {
            Normal
        }

        public enum TreeTypes
        {
            Spring,
            Summer,
            Autumn,
            Winter
        }

        [Header("Data")]
        public Object_behaviour Object => this.gameObject.GetComponent<Object_behaviour>();

        [Header("Settings")]
        public Tree_Sizes Size = Tree_Sizes.Normal;
        public TreeTypes Type = TreeTypes.Spring;

        [Header("Data")]
        public float Health = 5;

        //Internal variables
        internal bool IsFalling = false;

        ///MAIN METHODS
        public void Take_damage(float dmg)
        {
            if (IsFalling) 
                return;

            // Set
            Health -= dmg;

            if (Health <= 0)
            {
                if(!IsFalling)
                {
                    IsFalling = true;

                    StartCoroutine(Fall());
                }
            }

            // Effects
            if (this.gameObject.TryGetComponent<Effects_controller>(out Effects_controller effectController))
            {
                effectController.Force_effect(Effects_controller.EffectType.Boing);
            }
        }

        private void Drop_items()
        {
            if (Object.ObjectData.Item_drop == null) { return; }

            // Check drop chance & drop items
            for (int i = 0; i < Object.ObjectData.Drop_amount; i++)
            {
                // Create item
                GameObject item = Game_utils.Instance.Create_item(Object.ObjectData.Item_drop, this.transform.position);
            }
        }

        private IEnumerator Fall()
        {
            // Create stump
            GameObject stump = Game_utils.Instance.Create_prefab_from_resources("Prefabs/Objects/Tree_stump_object");
            stump.transform.position = new Vector3(transform.position.x, transform.position.y, 0);

            // Audio
            Game_utils.Instance.Create_sound("Tree_falling_sound", "Audios/Objects/Tree_falling_1", this.transform.position);

            float startAngle = transform.localEulerAngles.z;
            if (startAngle > 180f) startAngle -= 360f; // normaliza para -180~180
            float targetAngle = -90f;

            float elapsed = 0f;
            float duration = 4.5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                float newAngle = Mathf.Lerp(startAngle, targetAngle, t);

                transform.localEulerAngles = new Vector3(
                    transform.localEulerAngles.x,
                    transform.localEulerAngles.y,
                    newAngle
                );

                yield return null;
            }

            Drop_items();

            // Effects
            Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Small_stone_break", this.transform.position);

            Destroy(this.gameObject);
        }
    }
}