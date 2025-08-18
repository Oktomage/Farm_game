using Game.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI.Notifications
{
    public class Notifications_controller : MonoBehaviour
    {
        public static Notifications_controller Instance { get; private set; }

        public class Notification
        {
            public string Title;
            public string Message;

            public float Duration;

            public Sprite Icon;
        }

        [Header("State")]
        public List<Notification> Notifications_queue = new List<Notification>();

        [Header("Compoenents")]
        public GameObject Panel_target;

        //Internal variables
        internal Coroutine Notification_queue_reader;

        private void Awake()
        {
            Instance = this;
        }

        /// CORE METHODS
        public void Set_notification_task(Notification not)
        {
            Notifications_queue.Add(not);

            if(Notification_queue_reader == null)
            {
                Notification_queue_reader = StartCoroutine(Notifications_queue_reader());
            }
        }

        private IEnumerator Notifications_queue_reader()
        {
            while (Notifications_queue.Count > 0)
            {
                Notification current_notification = Notifications_queue[0];
                Notifications_queue.RemoveAt(0);

                //Create object
                GameObject ui_notification_obj = Game_utils.Instance.Create_prefab_from_resources("Prefabs/UI/PNotification", Panel_target);
                UI_Notification ui_not = ui_notification_obj.GetComponent<UI_Notification>();

                ui_not.Configure(current_notification.Title, current_notification.Message, current_notification.Icon);

                //Sound
                Game_utils.Instance.Create_2d_sound("Notification_sound", "Audios/UI/Pop_1");

                yield return new WaitForSeconds(current_notification.Duration);

                Destroy(ui_notification_obj);
            }

            Notifications_queue.Clear();

            Notification_queue_reader = null;
        }
    }
}