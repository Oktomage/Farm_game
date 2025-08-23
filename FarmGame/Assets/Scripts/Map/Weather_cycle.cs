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
        public Weathers Weather;

        //Internal variables
        private GameObject Player_character_obj => GameObject.FindGameObjectWithTag("Player");
        private List<GameObject> Water_drops_objs = new List<GameObject>();
        private int Max_water_drops = 412;

        private void Awake()
        {
            Game_events.New_day.AddListener(Randomize_weather);
        }

        private void Start()
        {
            Start_rain();
            StartCoroutine(Move_water_drops());
        }

        /// CORE METHODS
        private void Randomize_weather()
        {
            float rain_c = Random.value;
            float storm_c = Random.value;

            // Rain
            if (rain_c <= Rain_weather_chance)
            {
                // Storm
                if (storm_c <= Rain_weather_chance)
                    Start_storm();
                // Rain
                else
                    Start_rain();
            }
        }

        /// MAIN METHODS
        private void Start_rain()
        {
            // Set
            Weather = Weathers.Rain;

            Rain_routine = StartCoroutine(Rain_particles_routine());

            // Events
            Game_events.Rain_started.Invoke();
        }

        private void Start_storm()
        {
            // Set
            Weather = Weathers.Storm;

            Rain_routine = StartCoroutine(Rain_particles_routine());

            // Events
            Game_events.Storm_started.Invoke();
        }

        private Coroutine Rain_routine;

        private IEnumerator Rain_particles_routine()
        {
            while(Weather == Weathers.Rain || Weather == Weathers.Storm)
            {
                // Spawn water drop
                if(Water_drops_objs.Count < Max_water_drops)
                {
                    float max_distance = 30f;
                    Vector3 pos = new Vector3(Player_character_obj.transform.position.x + Random.Range(-max_distance, max_distance), Player_character_obj.transform.position.y + max_distance, 0);

                    // Create
                    GameObject wtr_drop = Game_utils.Instance.Create_prefab_from_resources("Prefabs/Misc/Water_drop", pos);

                    // Add
                    Water_drops_objs.Add(wtr_drop);
                
                    StartCoroutine(Kill_water_drop_routine(wtr_drop, Random.Range(0.5f, 8f)));
                }

                yield return new WaitForSeconds(Random.Range(0.001f, 0.005f));
            }

            // Free routine
            Rain_routine = null;
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
                Water_drops_objs.Remove(obj);

                GameObject wtr_drop_splash = Game_utils.Instance.Create_prefab_from_resources("Prefabs/Misc/Water_drop_splash", obj.transform.position);

                StartCoroutine(Kill_obj_routine(wtr_drop_splash, 0.2f));

                Destroy(obj);
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