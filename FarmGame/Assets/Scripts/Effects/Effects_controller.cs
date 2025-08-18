using Game.Utils;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

namespace Game.Effects
{
    public class Effects_controller : MonoBehaviour
    {
        public enum EffectType
        {
            None,
            Pulse,
            Boing,
            SquashStretch,
            Flash
        }

        [Header("Settings")]
        public EffectType effectType = EffectType.None;

        [Header("Secundary effects settings")]
        public bool Souls_aura = false;
        [ShowIf("Souls_aura")]
        public Color Souls_aura_color = Color.white;

        [Header("Components")]
        public SpriteRenderer Render => GetComponentInChildren<SpriteRenderer>();

        //Internals
        internal Coroutine Pulse_effect_routine;
        internal Coroutine Boing_effect_routine;
        internal Coroutine SquashStretch_effect_routine;
        internal Coroutine Flash_effect_routine;

        internal ParticleSystem Souls_aura_particle;

        private void Start()
        {
            Read_effect();
            Create_secunday_effects();
        }

        ///CORE METHODS
        private void Read_effect()
        {
            switch (effectType)
            {
                case EffectType.None:
                    break;
                case EffectType.Pulse:
                    DoPulse();
                    break;
                case EffectType.Boing:
                    DoBoing();
                    break;
                case EffectType.SquashStretch:
                    DoSquash();
                    break;
                case EffectType.Flash:
                    DoFlash();
                    break;

                default:
                    Debug.LogWarning("Unknown effect type: " + effectType);
                    break;
            }
        }

        internal void Force_effect(EffectType effect)
        {
            //Set
            effectType = effect;

            Read_effect();
        }

        ///MAIN METHODS
        private void DoPulse()
        {
            if(Pulse_effect_routine == null)
                Pulse_effect_routine = StartCoroutine(Pulse_effect());
        }

        private IEnumerator Pulse_effect()
        {
            Vector3 minScale = new Vector3(0.9f, 0.9f, 1f);
            Vector3 maxScale = new Vector3(1.1f, 1.1f, 1f);
            float duration = 0.5f;

            while (true)
            {
                // Crescer
                yield return StartCoroutine(ScaleOverTime(transform, minScale, maxScale, duration));

                // Encolher
                yield return StartCoroutine(ScaleOverTime(transform, maxScale, minScale, duration));
            }
        }

        private IEnumerator ScaleOverTime(Transform target, Vector3 startScale, Vector3 endScale, float time)
        {
            float elapsed = 0f;

            while (elapsed < time)
            {
                float t = elapsed / time;
                target.localScale = Vector3.Lerp(startScale, endScale, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            target.localScale = endScale;
        }

        private void DoBoing()
        {
            if (Boing_effect_routine == null)
                Boing_effect_routine = StartCoroutine(Boing());
        }

        IEnumerator Boing()
        {
            float scaleAmount = 1.2f;
            float duration = 0.1f;

            Vector3 originalScale = transform.localScale;
            Vector3 targetScale = originalScale * scaleAmount;

            // Scaling up
            float t = 0;

            while (t < 1)
            {
                t += Time.deltaTime / duration;
                transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.Sin(t * Mathf.PI * 0.5f));
                yield return null;
            }

            // Scaling down
            t = 0;

            while (t < 1)
            {
                t += Time.deltaTime / duration;
                transform.localScale = Vector3.Lerp(targetScale, originalScale, Mathf.Sin(t * Mathf.PI * 0.5f));
                yield return null;
            }

            Boing_effect_routine = null;
        }

        private void DoSquash()
        {
            if(SquashStretch_effect_routine == null)
                SquashStretch_effect_routine = StartCoroutine(SquashStretch());
        }

        private IEnumerator SquashStretch()
        {
            float stretchX = 0.2f;   // quanto estica na horizontal
            float squashY = 0.1f;    // quanto achata na vertical
            float stretchTime = 0.1f; // tempo para esticar
            float settleTime = 0.15f; // tempo para voltar

            Vector3 baseScale = transform.localScale;

            // Escala alvo
            Vector3 targetScale = new Vector3(
                    baseScale.x * (1f + stretchX),
                    baseScale.y * (1f - squashY),
                    baseScale.z
                );

            // Etapa 1: ir para o esticado
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / stretchTime;
                transform.localScale = Vector3.Lerp(baseScale, targetScale, t);
                yield return null;
            }

            // Etapa 2: voltar para o normal
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / settleTime;
                transform.localScale = Vector3.Lerp(targetScale, baseScale, t);
                yield return null;
            }

            transform.localScale = baseScale;

            SquashStretch_effect_routine = null;
        }

        private void DoFlash()
        {
            if(Flash_effect_routine == null)
                Flash_effect_routine = StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            float flashDuration = 0.2f;
            float flashIntensity = 1f;

            //Set material
            Material flash_mat = Game_utils.Instance.Get_material("Materials/Flash_material");
            Render.material = flash_mat;

            flash_mat.SetFloat("_Flash_intensity", flashIntensity);

            yield return new WaitForSeconds(flashDuration);

            flash_mat.SetFloat("_Flash_intensity", 0f);

            //Reset material
            Material default_mat = Game_utils.Instance.Get_material("Materials/Default_material");
            Render.material = default_mat;

            //Disable flashing 
            Flash_effect_routine = null;
        }

        /// SECUNDARY EFFECTS
        private void Create_secunday_effects()
        {
            if (Souls_aura)
            {
                GameObject aura = Game_utils.Instance.Create_particle_from_resources("Prefabs/Particles/Souls_aura", transform.position);
                aura.transform.SetParent(transform);
                aura.transform.localPosition = new Vector3(0, -0.5f, 0);

                Souls_aura_particle = aura.GetComponent<ParticleSystem>();

                var main = Souls_aura_particle.main;
                main.startColor = Souls_aura_color;
            }
        }
    }
}
