using Game.Events;
using Game.UI;
using Game.Utils;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Characters.Shopper
{
    public class Shopper_controller : MonoBehaviour
    {
        [Header("Settings")]
        [Range(0.1f, 10f)]
        public float Shopper_detector_range = 2.5f;

        [Header("State")]
        public bool Player_in_range = false;
        public bool isShopOpen = false;

        [Header("Components")]
        public GameObject Interact_key_obj;

        //Internal variables
        internal GameObject Player_character_obj => GameObject.FindGameObjectWithTag("Player");

        private void Start()
        {
            StartCoroutine(Detector());
        }

        internal void Call_shop()
        {
            if (!isShopOpen)
            {
                isShopOpen = true;

                //Events
                Game_events.Player_character_opened_shop.Invoke(this);

                //Audio
                Game_utils.Instance.Create_sound("Shop_Open", "Audios/Characters/Shopper_1", transform.position);
            }
        }

        private IEnumerator Detector()
        {
            while(true)
            {
                yield return new WaitForSeconds(0.1f);
                if (Player_character_obj == null) { continue; }

                float distance = Vector2.Distance(transform.position, Player_character_obj.transform.position);

                if(Interact_key_obj)
                    Interact_key_obj.SetActive(distance <= Shopper_detector_range);

                Player_in_range = distance <= Shopper_detector_range;

                //Out of range
                if (isShopOpen && distance > Shopper_detector_range)
                {
                    isShopOpen = false;

                    //Events
                    Game_events.Player_character_closed_shop.Invoke();
                }
            }
        }
    }
}