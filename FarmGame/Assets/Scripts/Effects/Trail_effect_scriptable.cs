using UnityEngine;

namespace Game.Effects
{
    [CreateAssetMenu(fileName = "New trail effect", menuName = "Effects/Trail_effect")]
    public class Trail_effect_scriptable : ScriptableObject
    {
        public Material Trail_material;
        public Gradient Trail_gradient = new Gradient();
        public AnimationCurve Trail_size_curve;
        [Range(0f, 1f)]
        public float Trail_time = 0.5f;
    }
}