using Game.Events;
using UnityEngine;

namespace Game.Cameras
{
    public class Minicam_controller : MonoBehaviour
    {
        public static Minicam_controller Instance;

        [Header("Components")]
        public Camera Minicam_camera;

        [Header("Camera Settings")]
        public Transform Target;
        public float MoveSpeed = 3f;

        private void Awake()
        {
            Instance = this;

            Game_events.Player_character_took_damage.AddListener((dmg, target) => Set_target(target));
        }

        void LateUpdate()
        {
            if (Target == null) return;

            Vector3 desiredPosition = Target.position;
            desiredPosition.z = transform.position.z;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, MoveSpeed * Time.deltaTime);
        }

        ///MAIN METHODS
        internal void Set_target(GameObject new_target_obj)
        {
            //Set
            Target = new_target_obj.transform;
        }
    }
}
