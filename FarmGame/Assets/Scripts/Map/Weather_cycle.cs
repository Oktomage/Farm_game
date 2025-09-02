using Game.Events;
using Game.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Map.Controller
{
    public class Weather_cycle : MonoBehaviour
    {
        public enum Weathers
        {
            Sunny,
            Rain,
            Storm
        }

        [Header("Settings")]
        [Range(0f, 1f)]
        public float Rain_weather_chance;
        [Range(0, 1f)]
        public float Storm_weather_chance;

        [Header("State")]
        public static Weathers Weather = Weathers.Sunny;

        //Internal variables
        private GameObject Player_character_obj => GameObject.FindGameObjectWithTag("Player");
        [SerializeField] private List<GameObject> Water_drops_objs = new List<GameObject>();
        private int Max_water_drops = 312;

        private void Awake()
        {
            Game_events.New_day.AddListener(Randomize_weather);
        }

        private void Start()
        {
            //Start_sunny();
            StartCoroutine(Move_water_drops());
        }

        private void Update()
        {
#if UNITY_EDITOR
            if(Input.GetKeyDown(KeyCode.F6))
            {
                Start_rain();
            }
            else if (Input.GetKeyDown(KeyCode.F7))
            {
                Start_storm();
            }
#endif
        }

        /// CORE METHODS
        private void Randomize_weather()
        {
            float rain_c = Random.value;
            float storm_c = Random.value;

            // Rain
            switch(Weather)
            {
                case Weathers.Sunny:
                    if (rain_c <= Rain_weather_chance)
                    {
                        // Storm
                        if (storm_c <= Rain_weather_chance)
                            Start_storm();
                        // Rain
                        else
                            Start_rain();
                    }
                    break;

                // Back to normal
                case Weathers.Rain:
                case Weathers.Storm:
                    Start_sunny();
                    break;
            }
        }

        /// MAIN METHODS
        private void Start_sunny()
        {
            // Set
            Weather = Weathers.Sunny;

            // Events
            Game_events.Sunny_day_started.Invoke();
        }

        private void Start_rain()
        {
            // Set
            Weather = Weathers.Rain;

            Rain_routine = StartCoroutine(Rain_particles_routine());

            // Events
            Game_events.Rain_day_started.Invoke();
        }

        private void Start_storm()
        {
            // Set
            Weather = Weathers.Storm;

            Rain_routine = StartCoroutine(Rain_particles_routine());
            Storm_routine = StartCoroutine(Storm_effects_routine());

            // Events
            Game_events.Storm_day_started.Invoke();
        }

        private void Do_thunder()
        {
            // Audio
            Game_utils.Instance.Create_2d_sound("Thunder_sound", "Audios/Ambiance/Thunder_" + Random.Range(1,2).ToString());
        }

        private Coroutine Rain_routine;

        private IEnumerator Rain_particles_routine()
        {
            int create_ammount_per_tick = 5;

            while(Weather == Weathers.Rain || Weather == Weathers.Storm)
            {
                // Spawn water drop
                if(Water_drops_objs.Count < Max_water_drops)
                {
                    float max_distance = 20f;

                    for (int i = 0; i < create_ammount_per_tick; i++)
                    {
                        Vector3 pos = new Vector3(Player_character_obj.transform.position.x + Random.Range(-max_distance, max_distance), Player_character_obj.transform.position.y + max_distance, 0);

                        // Create
                        GameObject wtr_drop = Game_utils.Instance.Create_prefab_from_resources("Prefabs/Misc/Water_drop", pos);

                        // Add
                        Water_drops_objs.Add(wtr_drop);

                        StartCoroutine(Kill_water_drop_routine(wtr_drop, Random.Range(0.5f, 8f)));
                    }

                    yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
                }       
            }

            // Free routine
            Rain_routine = null;
        }

        private Coroutine Storm_routine;

        private IEnumerator Storm_effects_routine()
        {
            float min_interval = 4f;

            while(Weather == Weathers.Storm)
            {
                switch (Weather)
                {
                    case Weathers.Rain:
                        break;

                    case Weathers.Storm:
                        float c = Random.value;
                        float thunder_c = 0.05f;

                        if (c <= thunder_c)
                        {
                            Do_thunder();
                        }
                        break;
                }

                yield return new WaitForSeconds(Random.Range(min_interval, min_interval * 2));
            }

            // Free routine
            Storm_routine = null;
        }

        private IEnumerator Move_water_drops()
        {
            float water_drop_speed = 16f;

            while (true)
            {
                if (Water_drops_objs.Count > 0)
                {
                    foreach (GameObject obj in Water_drops_objs)
                    {
                        obj.transform.Translate(0, -water_drop_speed * Time.deltaTime, 0);
                    }
                }

                yield return null;
            }
        }

        private IEnumerator Kill_water_drop_routine(GameObject obj, float t)
        {
            yield return new WaitForSeconds(t);

            if (obj != null)
            {
                // Set
                Water_drops_objs.Remove(obj);

                Destroy(obj);

                // Create
                GameObject wtr_drop_splash = Game_utils.Instance.Create_prefab_from_resources("Prefabs/Misc/Water_drop_splash", obj.transform.position);

                StartCoroutine(Kill_obj_routine(wtr_drop_splash, 0.2f));
            }
        }

        private IEnumerator Kill_obj_routine(GameObject obj, float t)
        {
            yield return new WaitForSeconds(t);

            if (obj != null)
            {
                Destroy(obj);
            }
        }
    }
}