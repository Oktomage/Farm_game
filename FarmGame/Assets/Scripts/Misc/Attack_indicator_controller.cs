using Game.Events;
using System.Collections;
using UnityEngine;

namespace Game.Utils.Misc
{
    public class Attack_indicator_controller : MonoBehaviour
    {
        private void Awake()
        {
            Game_events.Attack_indicator.AddListener(Create_indicator);
        }

        public class Indicator_info
        {
            public enum Indicator_formats
            {
                Circle,
                Box
            }

            public Indicator_formats Format;

            public float Radius;
            public float Duration;
        }

        private void Create_indicator(Indicator_info info, Vector2 pos)
        {
            // Create
            GameObject indicator_obj = Game_utils.Instance.Create_gameObject();
            SpriteRenderer render = indicator_obj.AddComponent<SpriteRenderer>();

            // Set
            indicator_obj.transform.position = pos;
            render.color = new Color(1, 0, 0, 0.4f);

            switch(info.Format)
            {
                case Indicator_info.Indicator_formats.Circle:
                    render.sprite = Game_utils.Instance.Get_sprite("Graphics/Magic/Circle");
                    break;

                case Indicator_info.Indicator_formats.Box:
                    render.sprite = Game_utils.Instance.Get_sprite("Graphics/Magic/Square");
                    break;
            }

            StartCoroutine(ScaleTo(indicator_obj, new Vector3(info.Radius, info.Radius, 1), 0.5f));
            StartCoroutine(Indicator_timer(indicator_obj, info));
        }

        IEnumerator ScaleTo(GameObject obj, Vector3 to, float dur)
        {
            Vector3 from = Vector3.zero;
            float t = 0f;
            float invDur = dur <= 0f ? 1f : 1f / dur;
            AnimationCurve Anim_curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            // Scale
            while (t < 1f)
            {
                t += Time.deltaTime * invDur;

                float eased = Anim_curve.Evaluate(Mathf.Clamp01(t));
                obj.transform.localScale = Vector3.LerpUnclamped(from, to, eased);

                yield return null;
            }

            // Set end scale
            obj.transform.localScale = to;
        }

        private IEnumerator Indicator_timer (GameObject obj, Indicator_info info)
        {
            yield return new WaitForSeconds(info.Duration);

            Destroy(obj);
        }
    }
}