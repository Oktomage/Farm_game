using Game.Characters;
using Game.Characters.Player;
using Game.Events;
using Game.Objects;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Utils.Misc
{
    public class Detector_manager : MonoBehaviour
    {
        [Header("Settings")]
        [Range(0.1f, 10f)]
        public float Detector_range = 2.5f;

        [Header("State")]
        public bool Object_in_range = false;
        public bool Character_in_range = false;
        public bool Player_in_range = false;

        [Header("Components")]
        public GameObject Interact_key_obj;

        [Header("Events")]
        public UnityEvent Detector_event_fire;

        private void Start()
        {
            StartCoroutine(Passive_detector());
        }

        IEnumerator Passive_detector()
        {
            while (true)
            {
                yield return null;

                Check_detections();
            }
        }

        private void Check_detections()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, Detector_range);

            bool object_detected = false;
            bool character_detected = false;
            bool player_detected = false;

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject.GetComponentInParent<Object_behaviour>())
                    object_detected = true;

                if (hit.gameObject.GetComponentInParent<Character_behaviour>())
                    character_detected = true;

                if (hit.gameObject.GetComponentInParent<Player_controller>())
                    player_detected = true;
            }

            // Set
            Object_in_range = object_detected;
            Character_in_range = character_detected;
            Player_in_range = player_detected;

            // Events
            if (Player_in_range)
                Detector_event_fire.Invoke();

            // Interact icon
            if (Interact_key_obj != null)
                Interact_key_obj.SetActive(Player_in_range);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, Detector_range);
        }
    }
}