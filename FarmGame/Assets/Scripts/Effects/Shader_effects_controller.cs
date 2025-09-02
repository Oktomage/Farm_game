using Game.Events;
using Game.Utils;
using System.Collections;
using UnityEngine;

namespace Game.Effects.Shaders
{
    public class Shader_effects_controller : MonoBehaviour
    {
        private void Awake()
        {
            Game_events.ShaderEffect.AddListener(DoShader_effect);
        }

        private void DoShader_effect(string shader_name, Vector2 pos)
        {
            switch(shader_name)
            {
                case "Shockwave":
                    // Create
                    GameObject shockwave_obj = Game_utils.Instance.Create_prefab_from_resources("Prefabs/Misc/Shockwave", pos);


                    StartCoroutine(Shock_wave_routine(shockwave_obj, start_value: -0.1f, end_value: 1f));
                    break;
            }
        }

        private IEnumerator Shock_wave_routine(GameObject shader_obj, float start_value, float end_value)
        {
            int waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");
            Material shockwave_material = shader_obj.GetComponent<SpriteRenderer>().material;

            // Set
            shockwave_material.SetFloat(waveDistanceFromCenter, start_value);

            float lerped_amount = 0f;
            
            float elapsed_time = 0f;
            float shader_effect_time = 0.75f;

            StartCoroutine(Kill_shader_obj(shader_obj, shader_effect_time));

            // Expand
            while (elapsed_time < shader_effect_time)
            {
                elapsed_time += Time.deltaTime;

                // Set
                lerped_amount = Mathf.Lerp(start_value, end_value, elapsed_time / shader_effect_time);
                shockwave_material.SetFloat(waveDistanceFromCenter, lerped_amount);

                yield return null;
            }      
        }

        private IEnumerator Kill_shader_obj(GameObject obj, float t)
        {
            yield return new WaitForSeconds(t);

            Destroy(obj);
        }
    }
}