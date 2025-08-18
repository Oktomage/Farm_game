using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Game.Cameras
{
    public class Camera_controller : MonoBehaviour
    {
        [Header("Components")]
        public Camera Main_camera;

        [Header("Camera Settings")]
        public Transform Target;
        public float MoveSpeed = 3f;

        void LateUpdate()
        {
            if (Target == null) return;

            Vector3 desiredPosition = Target.position;
            desiredPosition.z = transform.position.z;

            transform.position = Vector3.Lerp(transform.position, desiredPosition, MoveSpeed * Time.deltaTime);
        }
    }
}
