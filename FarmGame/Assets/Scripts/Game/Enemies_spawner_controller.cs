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
        [Range(0, 1f)]
        public float Boss_chance_per_day = 0.05f;

        [Header("State")]
        public bool IsSpawning = false;
        public bool Can_SpawnBoss = false;

        //Internal variables
        internal GameObject Player_character_obj => GameObject.FindGameObjectWithTag("Player");

        private void Awake()
        {
            Game_events.Day_stage_changed.AddListener(Read_day_stage);
        }

        private void Start()
        {
#if UNITY_EDITOR
            Can_SpawnBoss = true;
#endif
        }

        /// CORE METHODS
        private void Read_day_stage()
        {
            switch(Game_controller.Current_day_stage)
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

            // Enable extra settings
            if(Game_controller.Instance.Total_days >= 5)
            {
                Can_SpawnBoss = true;
            }
        }

        /// MAIN METHODS
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
        
        internal class Enemy_info
        {
            internal enum Enemy_types { Normal, Boss }
            internal Enemy_types Type;

            internal Character_scriptable.Character_menance_class Menance_Class;
        }

        private ScriptableObject Get_enemy_to_spawn(Enemy_info enemy_info)
        {
            ScriptableObject enemy_scriptable = null;
            List<ScriptableObject> target_class_enemies = Game_utils.Instance.Get_enemies_from_menance_class(enemy_info.Menance_Class);
            enemy_scriptable = target_class_enemies[Random.Range(0, target_class_enemies.Count)];

            switch (enemy_info.Type)
            {
                case Enemy_info.Enemy_types.Normal:
                    break;

                case Enemy_info.Enemy_types.Boss:
                    break;
            }

            return enemy_scriptable;
        }

        private void Spawn_enemy()
        {
            Vector2 pos = new Vector2(Player_character_obj.transform.position.x, Player_character_obj.transform.position.y) + Random.insideUnitCircle * 10f;

            Enemy_info info = new Enemy_info
            {
                Type = Enemy_info.Enemy_types.Normal,
                Menance_Class = Character_scriptable.Character_menance_class.Peaceful
            };

            GameObject enemy = Game_utils.Instance.Create_enemy(Get_enemy_to_spawn(info), pos);
        }

        private void Spawn_boss()
        {
            Vector2 pos = new Vector2(Player_character_obj.transform.position.x, Player_character_obj.transform.position.y) + Random.insideUnitCircle * 10f;

            Enemy_info info = new Enemy_info
            {
                Type = Enemy_info.Enemy_types.Boss,
                Menance_Class = Character_scriptable.Character_menance_class.Peaceful
            };

            GameObject boss = Game_utils.Instance.Create_enemy(Get_enemy_to_spawn(info), pos);
        }

        private IEnumerator Spawner()
        {
            while (IsSpawning)
            {
                yield return new WaitForSeconds(Spawn_interval);

                Spawn_enemy();

                if (Can_SpawnBoss)
                {
                    float c = Random.value;

                    if (c <= Boss_chance_per_day)
                    {
                        Spawn_boss();
                    }
                }
            }
        }
    }
}
