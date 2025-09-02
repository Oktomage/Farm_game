using Game.Events;
using System.Collections;
using UnityEngine;

namespace Game.Cameras
{
    public class Camera_controller : MonoBehaviour
    {
        [Header("Components")]
        public Camera Main_camera;

        [Header("Camera Settings")]
        public Transform Target;
        public float MoveSpeed = 3f;

        //Internal variables
        internal Vector3 shakeOffset = Vector3.zero;
        internal Coroutine Shake_coroutine;

        private void Awake()
        {
            Game_events.Camera_shake.AddListener(DoCameraShake);
        }

        void LateUpdate()
        {
            if (Target == null) return;

            Vector3 desiredPosition = Target.position;
            desiredPosition.z = transform.position.z;

            // Move
            transform.position = Vector3.Lerp(transform.position, desiredPosition, MoveSpeed * Time.deltaTime) + shakeOffset;
        }

        //MAIN METHODS
        private void DoCameraShake()
        {
            if (Shake_coroutine != null) 
                StopCoroutine(Shake_coroutine);
            
            // Start
            Shake_coroutine = StartCoroutine(ShakeRoutine());
        }

        private IEnumerator ShakeRoutine()
        {
            float shakeDuration = 1.6f;
            float shakeMagnitude = 0.25f;
            AnimationCurve shakeDamping = AnimationCurve.EaseInOut(0, 1, 1, 0);

            float t = 0f;            

            while (t < shakeDuration)
            {
                // Set
                t += Time.deltaTime;

                float damper = shakeDamping.Evaluate(t / shakeDuration); // 1→0
                Vector2 rnd = Random.insideUnitCircle * shakeMagnitude * damper;
                shakeOffset = new Vector3(rnd.x, rnd.y, 0f);

                yield return null;
            }

            // Clear
            shakeOffset = Vector3.zero;
            Shake_coroutine = null;
        }
    }
}
