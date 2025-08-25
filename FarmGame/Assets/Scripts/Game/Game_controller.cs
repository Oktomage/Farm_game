using Game.Events;
using Game.Utils;
using System.Collections;
using UnityEngine;

namespace Game.Controller
{
    public class Game_controller : MonoBehaviour
    {
        public static Game_controller Instance;

        public enum Seasons 
        {
            Spring,
            Summer,
            Autumn,
            Winter
        }

        public enum Day_stages
        {
            Day,
            Night,
            Rain,
            Storm,
            Blood_moon
        }

        [Header("Day - settings")]
        [Range(1, 30)]
        public int Days_per_season = 30;
        [Range(0, 20f)]
        public float Seconds_per_hour = 20f;

        [Header("Day - states")]
        public int Total_days = 0;
        public int Day_hour = 8;
        public Day_stages Current_day_stage = Day_stages.Day;

        [Space]
        public Seasons Current_season = Seasons.Spring;
        public int Day_of_season = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
#if UNITY_EDITOR
            Seconds_per_hour = 5f;
#endif

            Set_day_stage(Day_stages.Day);
            StartCoroutine(Hour_timer());

            //Events
            Game_events.Warning_panel_called.Invoke("Fields of the Full Moon");
        }

        private void Update()
        {
#if UNITY_EDITOR
            if(Input.GetKeyDown(KeyCode.F1))
            {
                New_hour();
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                New_day();
            }
            else if (Input.GetKeyDown(KeyCode.F5))
            {
                Game_utils.Instance.Create_enemy(Game_utils.Instance.Get_scriptable("Scriptables/Enemies/Ant_king"), new Vector2(15, 15));
            }
#endif
        }

        /// CORE METHODS
        private void Set_day_stage(Day_stages stage)
        {
            //Set
            Current_day_stage = stage;

            switch (stage)
            {
                case Day_stages.Day:
                    New_day();
                    break;
                case Day_stages.Night:
                    New_night();
                    break;
                case Day_stages.Rain:
                    // Handle rain start
                    break;
                case Day_stages.Storm:
                    // Handle storm start
                    break;
                case Day_stages.Blood_moon:
                    // Handle blood moon start
                    break;
            }

            //Events
            Game_events.Day_stage_changed.Invoke(Current_day_stage);
        }

        private void New_day()
        {
            //Set
            Total_days++;
            Day_of_season++;

            //Events
            Game_events.New_day.Invoke();
        }

        private void New_night()
        {
            StartCoroutine(Night_timer(Seconds_per_hour * 2.5f));
        }

        private void New_hour()
        {
            //Set
            Day_hour++;
            Day_hour = Mathf.Clamp(Day_hour, 0, 23);

            //Start night
            if (Day_hour >= 18 && Current_day_stage != Day_stages.Night)
            {
                Set_day_stage(Day_stages.Night);
            }

            //Events
            Game_events.New_hour.Invoke();
        }

        private IEnumerator Hour_timer()
        {
            while (true)
            {
                yield return new WaitForSeconds(Seconds_per_hour);

                New_hour();
            }
        }

        private IEnumerator Night_timer(float duration)
        {
            // Wait for midnight
            float timeToMidnight = (24 - Day_hour) * Seconds_per_hour;
            yield return new WaitForSeconds(timeToMidnight);

            // Night duration
            yield return new WaitForSeconds(duration);

            //End night
            Day_hour = 0;

            Set_day_stage(Day_stages.Day);
        }
    }
}