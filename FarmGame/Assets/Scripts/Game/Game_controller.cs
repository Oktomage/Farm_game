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
            Blood_moon,
            Boss
        }

        [Header("Day - settings")]
        [Range(1, 30)]
        public int Days_per_season = 30;
        [Range(0, 20f)]
        public float Seconds_per_hour = 20f;

        [Header("Day - states")]
        public int Total_days = 0;
        public int Day_hour = 8;
        public static Day_stages Current_day_stage = Day_stages.Day;

        [Space]
        public Seasons Current_season = Seasons.Spring;
        public int Day_of_season = 0;

        private void Awake()
        {
            Instance = this;

            Game_events.Sunny_day_started.AddListener(() => Set_day_stage(stage: Day_stages.Day));
            Game_events.Rain_day_started.AddListener(() => Set_day_stage(stage: Day_stages.Rain));
            Game_events.Storm_day_started.AddListener(() => Set_day_stage(stage: Day_stages.Storm));
            Game_events.Boss_day_started.AddListener(() => Set_day_stage(stage: Day_stages.Boss));
        }

        private void Start()
        {
#if UNITY_EDITOR
            Seconds_per_hour = 5f;
#endif

            Init_new_day();
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
                Init_new_day();
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
                    New_rain_day();
                    break;
                case Day_stages.Storm:
                    // Handle storm start
                    break;
                case Day_stages.Blood_moon:
                    // Handle blood moon start
                    break;

                case Day_stages.Boss:
                    New_boss_day();
                    break;
            }

            //Events
            Game_events.Day_stage_changed.Invoke();
        }

        /// MAIN METHODS
        private void Init_new_day()
        {
            //Set
            Total_days++;
            Day_of_season++;

            Set_day_stage(Day_stages.Day);

            //Events
            Game_events.New_day.Invoke();
        }

        private void New_day()
        {

        }

        private void New_night()
        {
            StartCoroutine(Night_timer(Seconds_per_hour * 2.5f));
        }

        private void New_rain_day()
        {

        }

        private void New_boss_day()
        {

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

            Init_new_day();
        }
    }
}