using Game.Events;
using Game.UI.Notifications;
using Game.Utils;
using UnityEngine;

namespace Game.Characters.Data
{
    public class Player_data : MonoBehaviour
    {
        public static Player_data Instance { get; private set; }

        [Header("Data")]
        public int Total_souls;

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if(Input.GetKeyDown(KeyCode.F4))
            {
                Add_souls(1000);
            }
#endif
        }

        /// MAIN METHODS
        public void Add_souls(int amount)
        {
            //Set
            Total_souls += amount;

            //Events
            Game_events.Player_collected_souls.Invoke(amount);

            //Notification
            Notifications_controller.Instance.Set_notification_task(new Notifications_controller.Notification
            {
                Title = "You collected souls",
                Message = $"You recivied {amount} souls.",
                Duration = 2f,
                Icon = Game_utils.Instance.Get_sprite("Graphics/Icons/Soul") // Optionally set an icon
            });
        }

        public void Decrease_souls(int amount)
        {
            //Set
            Total_souls -= amount;

            //Events
            Game_events.Player_lost_souls.Invoke(amount);
        }
    }
}