using Game.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.Notifications
{
    public class UI_Notification : MonoBehaviour
    {
        [Header("Components")]
        public TextMeshProUGUI Title_text;
        public TextMeshProUGUI Message_text;
        public Image Icon_image;

        internal void Configure(string title, string message, Sprite icon)
        {
            //Title_text.text = title;
            Message_text.text = message;

            if (icon != null)
            {
                Icon_image.sprite = icon;
                Icon_image.enabled = true;
            }
            else
            {
                Icon_image.enabled = false;
            }

            //Effects
            Game_utils.Instance.Do_UI_pop_effect(this.gameObject);
        }
    }
}