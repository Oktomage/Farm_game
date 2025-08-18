using Game.Controller;
using Game.Events;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Game.Map
{
    public class Day_night_cycle : MonoBehaviour
    {
        [Header("Settings")]
        public int Start_fade_hour = 16;
        public int End_fade_hour = 23;

        [Header("Components")]
        public Light2D Global_light;

        private void Awake()
        {
            Game_events.New_hour.AddListener(Update_light);
        }

        public void Update_light()
        {
            float GetDayValue(float hour)
            {
                // Normaliza o valor para 0~1 dentro do intervalo
                float t = Mathf.InverseLerp(Start_fade_hour, End_fade_hour, hour);

                // Inverte (1 no início, 0 no fim)
                return 1f - t;
            }

            float dayValue = GetDayValue(Game_controller.Instance.Day_hour);
            dayValue = Mathf.Clamp(dayValue, 0.05f, 1f);

            float dayLight_blend_time = 1f;

            StopAllCoroutines();
            StartCoroutine(BlendLight(dayValue, dayLight_blend_time));
        }

        private IEnumerator BlendLight(float targetValue, float duration)
        {
            float startValue = Global_light.intensity;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                Global_light.intensity = Mathf.Lerp(startValue, targetValue, t);

                yield return null;
            }

            // Set final value
            Global_light.intensity = targetValue;
        }
    }
}