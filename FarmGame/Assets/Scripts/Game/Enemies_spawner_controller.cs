using Game.Characters;
using Game.Events;
using Game.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Controller
{
    public class Enemies_spawner_controller : MonoBehaviour
    {
        [Header("Settings")]
        [Range(0, 15f)]
        public float Spawn_interval = 5f;

        [Header("State")]
        public bool IsSpawning = false;

        //Internal variables
        internal GameObject Player_obj;

        private void Awake()
        {
            Game_events.Day_stage_changed.AddListener(Read_day_stage);
        }

        private void Start()
        {
            Player_obj = GameObject.FindGameObjectWithTag("Player");
        }

        /// CORE METHODS
        private void Read_day_stage(Game_controller.Day_stages stage)
        {
            switch(stage)
            {
                case Game_controller.Day_stages.Day:
                    Stop_spawner();
                    break;
                case Game_controller.Day_stages.Night:
                case Game_controller.Day_stages.Rain:
                case Game_controller.Day_stages.Storm:
                case Game_controller.Day_stages.Blood_moon:
                    Start_spawner();
                    break;
            }
        }

        private void Start_spawner()
        {
            if(IsSpawning)
                return;

            IsSpawning = true;

            StartCoroutine(Spawner());
        }

        private void Stop_spawner()
        {
            if(!IsSpawning)
                return;

            IsSpawning = false;
        }

        /// MAIN METHODS
        private ScriptableObject Get_enemy_to_spawn()
        {
            ScriptableObject enemy_scriptable = null;
            List<ScriptableObject> target_class_enemies = Game_utils.Instance.Get_enemies_from_menance_class(Character_scriptable.Character_menance_class.Peaceful);

            enemy_scriptable = target_class_enemies[Random.Range(0, target_class_enemies.Count)];

            return enemy_scriptable;
        }

        private IEnumerator Spawner()
        {
            while (IsSpawning)
            {
                Vector2 pos = new Vector2(Player_obj.transform.position.x, Player_obj.transform.position.y) + Random.insideUnitCircle * 10f;

                GameObject enemy = Game_utils.Instance.Create_enemy(Get_enemy_to_spawn(), pos);

                yield return new WaitForSeconds(Spawn_interval);
            }
        }
    }
}
