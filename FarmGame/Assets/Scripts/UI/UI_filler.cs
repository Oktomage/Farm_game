using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UI_filler : MonoBehaviour
    {
        [Header("Components")]
        public Image fillInstant;
        public Image fillLerped;

        [Header("Config")]
        [Range(0, 1f)]
        public float Delay = 0.2f;
        [Range(0, 2f)]
        public float Speed = 1f;

        private Coroutine Animate_routine;

        void Start()
        {
            Set_value(target_value: 1f);
        }

        internal void Set_value(float target_value)
        {
            target_value = Mathf.Clamp01(target_value);

            // Set
            fillInstant.fillAmount = target_value;

            if (target_value < fillLerped.fillAmount)
            {
                if (Animate_routine != null) 
                    StopCoroutine(Animate_routine);

                Animate_routine = StartCoroutine(Animate_fill(target_value));
            }
            else
            {
                fillLerped.fillAmount = target_value;
            }
        }

        private IEnumerator Animate_fill(float target_value)
        {
            yield return new WaitForSeconds(Delay);

            // Vai “alcançando” com MoveTowards (suave e frame-independente)
            while (!Mathf.Approximately(fillLerped.fillAmount, target_value))
            {
                fillLerped.fillAmount = Mathf.MoveTowards(fillLerped.fillAmount, target_value, Speed * Time.deltaTime);

                yield return null;
            }

            // Clear

            Animate_routine = null;
        }
    }
}