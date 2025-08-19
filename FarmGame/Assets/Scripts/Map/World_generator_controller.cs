using Game.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Map.Controller
{
    public class World_generator_controller : MonoBehaviour
    {
        [Header("Map Settings")]
        public Vector2 MapSize = new Vector2(100, 100);
        public float CellSize = 1f;
        [Range(1, 1000)]
        public int Max_world_objects = 100;
        [Range(1, 1024)]
        public float Spawn_cells_per_frame = 512;

        [Range(0, 1000)]
        public int Max_world_animals = 50;

        [System.Serializable]
        public class Spawn_option
        {
            public string Entity_name;
            public string Entity_resources_path;
            [Space]
            [Range(0f, 1f)] 
            public float Probability;
        }

        internal List<Spawn_option> World_spawn_table = new List<Spawn_option>();

        [Header("UI Components (For testing pourposes)")]
        public GameObject Panel_loading;
        public Slider Slider_progress_bar;

        //Internal variables
        [SerializeField] internal int Objects_generated = 0;
        [SerializeField] internal int Animals_generated = 0;

        private void Start()
        {
            Generate_world();

            // Save to JSON
            //Game_utils.Instance.ExportToJson(SpawnTable, folder_path: "Resources/JSON/World", file_name: "World_spawn_table");
        }

        /// CORE METHODS
        private void Generate_world()
        {
            // Get JSON
            World_spawn_table = Game_utils.Instance.Read_from_Json<List<Spawn_option>>("JSON/World/World_spawn_table");

            if(World_spawn_table.Count > 0)
                StartCoroutine(Generate_world_entitys());
        }

        /// MAIN METHODS
        internal class Entity_info
        {
            internal enum Entity_types
            {
                Object,
                Animal
            }

            internal Entity_types Type;
        }
        private GameObject Spawn_entity(Entity_info info, Vector2 pos)
        {
            float totalProbability = 0f;

            // Get total probabilities
            for (int i = 0; i < World_spawn_table.Count; i++)
            {
                totalProbability += World_spawn_table[i].Probability;
            }

            float randomValue = Random.value;

            if (randomValue > totalProbability)
                return null;

            // Acumulador para encontrar em qual faixa de probabilidade caiu
            float probabilityAccumulator = 0f;

            for (int i = 0; i < World_spawn_table.Count; i++)
            {
                probabilityAccumulator += World_spawn_table[i].Probability;

                //Spawn entity
                if (randomValue <= probabilityAccumulator)
                {
                    GameObject spawnedEntity = Game_utils.Instance.Create_prefab_from_resources(World_spawn_table[i].Entity_resources_path);
                    spawnedEntity.transform.position = pos;

                    Objects_generated++;

                    return spawnedEntity;
                }
            }

            return null; // fallback (não deveria chegar aqui)
        }

        private bool Check_if_can_spawn_at_cell(Vector2 pos)
        {
            bool check = false;

            Collider2D hit = Physics2D.OverlapCircle(pos, 0.5f);
            check = hit == null; //If do not collide, set true

            return check;
        }

        private IEnumerator Generate_world_entitys()
        {
            int total = Mathf.RoundToInt(MapSize.x * MapSize.y);
            int done = 0;

            // Initialize UI
            Panel_loading.GetComponent<CanvasGroup>().alpha = 1;
            SetProgress(0f);

            int processedThisFrame = 0;

            for (float x = 0; x < MapSize.x; x++)
            {
                for (float y = 0; y < MapSize.y; y++)
                {
                    Vector2 pos = new Vector2(x + CellSize / 2, y + CellSize / 2);
                    GameObject entity = null;
                    Entity_info entity_Info = new Entity_info();

                    if (Check_if_can_spawn_at_cell(pos))
                    {
                        // Object
                        if (Objects_generated < Max_world_objects)
                        {
                            entity_Info.Type = Entity_info.Entity_types.Object;
                        }
                        else if (Animals_generated < Max_world_animals)
                        {
                            entity_Info.Type = Entity_info.Entity_types.Animal;
                        }

                        entity = Spawn_entity(entity_Info, pos);
                    }

                    // Set
                    done++;
                    processedThisFrame++;

                    // atualiza UI de tempos em tempos (barato)
                    if ((done & 0x3F) == 0) // a cada 64 células
                        SetProgress((float)done / total);

                    if (processedThisFrame >= Spawn_cells_per_frame)
                    {
                        processedThisFrame = 0;
                        yield return null;
                    }
                }
            }

            // End
            SetProgress(1f);
            Panel_loading.GetComponent<CanvasGroup>().alpha = 0;
        }

        private void SetProgress(float t)
        {
            if (Slider_progress_bar) Slider_progress_bar.value = Mathf.Clamp01(t);
            //if (progressLabel) progressLabel.text = $"Loading world... {(int)(t * 100f)}%";
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            for (int x = 0; x < MapSize.x; x++)
            {
                for (int y = 0; y < MapSize.y; y++)
                {
                    Vector3 pos = new Vector3(transform.position.x + x + 0.5f, transform.position.y + y + 0.5f, 0);
                    Gizmos.DrawWireCube(pos, Vector3.one);
                }
            }
        }
    }
}