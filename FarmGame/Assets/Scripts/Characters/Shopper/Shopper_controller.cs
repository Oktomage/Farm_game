using Game.Events;
using Game.Utils;
using Game.Utils.Misc;
using System.Collections;
using UnityEngine;

namespace Game.Characters.Shopper
{
    public class Shopper_controller : MonoBehaviour
    {
        [Header("State")]
        public bool isShopOpen = false;

        [Header("Components")]
        public Detector_manager Detector => this.gameObject.GetComponent<Detector_manager>();

        //Internal variables
        internal GameObject Player_character_obj => GameObject.FindGameObjectWithTag("Player");

        internal void Call_shop()
        {
            if (!isShopOpen)
            {
                isShopOpen = true;
                StartCoroutine(Read_detector_state());

                //Events
                Game_events.Player_character_opened_shop.Invoke(this);

                //Audio
                Game_utils.Instance.Create_sound("Shop_Open", "Audios/Characters/Shopper_1", transform.position);
            }
        }

        private IEnumerator Read_detector_state()
        {
            while(isShopOpen)
            {
                yield return new WaitForSeconds(0.1f);
                if (Player_character_obj == null) { continue; }

                // Out of range
                if (!Detector.Player_in_range)
                {
                    if (isShopOpen)
                    {
                        isShopOpen = false;

                        //Events
                        Game_events.Player_character_closed_shop.Invoke();
                    }
                }
            }
        }
    }
}