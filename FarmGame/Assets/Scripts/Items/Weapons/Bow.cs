using Game.Items.Tools;
using UnityEngine;

namespace Game.Items.Weapons
{
    public class Bow : MonoBehaviour
    {
        [Header("Data")]
        public Tool_behaviour Tool => this.gameObject.GetComponent<Tool_behaviour>();

        internal void Fire(Vector2 dir)
        {

        }
    }
}