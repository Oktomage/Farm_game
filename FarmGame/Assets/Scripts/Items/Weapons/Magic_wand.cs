using Game.Entitys.Projectiles;
using Game.Items.Tools;
using Game.Utils;
using UnityEngine;

namespace Game.Items.Weapons
{
    public class Magic_wand : MonoBehaviour
    {
        [Header("Data")]
        public Tool_behaviour Tool => this.gameObject.GetComponent<Tool_behaviour>();

        /// MAIN METHODS
        internal void Fire(Vector2 dir)
        {
            GameObject orb_obj = Game_utils.Instance.Create_prefab_from_resources("Prefabs/Projectiles/Projectile_entity", Tool.Character.transform.position);
            Projectile_entity projec_entity = orb_obj.GetComponent<Projectile_entity>();

            // Call
            projec_entity.Configure(char_parent_obj: Tool.Character.gameObject, sprite: Tool.Item.ItemData.Magic_sprite, Min_dmg: 0, Max_dmg: 1f, speed: 10f, dir);
        }
    }
}