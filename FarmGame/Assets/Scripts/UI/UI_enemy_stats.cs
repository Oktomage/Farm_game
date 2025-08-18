using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UI_enemy_stats : MonoBehaviour
    {
        [Header("Components")]
        public TextMeshProUGUI Text_enemy_name;
        public TextMeshProUGUI Text_enemy_bonuses;

        [Space]
        public Slider SHealth;

        [Space]
        public Image Icon;
    }
}