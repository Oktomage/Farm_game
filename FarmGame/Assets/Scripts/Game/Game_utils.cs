using Game.Characters;
using Game.Characters.Enemies;
using Game.Items;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using static Game.Utils.Game_utils;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Utils
{
    public class Game_utils : MonoBehaviour
    {
        public static Game_utils Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Get_all_recipes();
        }

        /// GAME ASSETS
        public GameObject Create_gameObject() => Create_gameObject(null);
        public GameObject Create_gameObject(GameObject parent)
        {
            GameObject new_obj = new GameObject();

            if(parent != null)
                new_obj.transform.SetParent(parent.transform, false);

            return new_obj;
        }

        public GameObject Create_prefab_from_resources(string path)
        {
            GameObject prefab = Instantiate(Resources.Load<GameObject>(path));

            return prefab;
        }
        public GameObject Create_prefab_from_resources(string path, GameObject parent)
        {
            GameObject prefab = Instantiate(Resources.Load<GameObject>(path));
            prefab.transform.SetParent(parent.transform, false);

            return prefab;
        }
        public GameObject Create_prefab_from_resources(string path, Vector3 origin_pos)
        {
            GameObject prefab = Instantiate(Resources.Load<GameObject>(path));
            prefab.transform.position = origin_pos;

            return prefab;
        }

        public GameObject Create_item(ScriptableObject item_scriptable, Vector2 origin_grid_pos)
        {
            GameObject new_item_obj = Create_prefab_from_resources("Prefabs/Items/Item");
            new_item_obj.transform.position = origin_grid_pos;

             Item_behaviour item_bhv = new_item_obj.GetComponent<Item_behaviour>();

            //Set
            item_bhv.Set_item_data(item_scriptable as Item_scriptable);
            item_bhv.Set_dropped_settings();

            return new_item_obj;
        }

        public GameObject Create_particle_from_resources(string path, Vector2 pos)
        {
            GameObject particle = Instantiate(Resources.Load<GameObject>(path));
            particle.transform.position = pos;

            return particle;
        }

        public GameObject Create_enemy(ScriptableObject char_scriptable, Vector2 origin_grid_pos)
        {
            GameObject new_enemy_obj = Create_prefab_from_resources("Prefabs/Enemies/Enemy");
            new_enemy_obj.transform.position = origin_grid_pos;

            Enemy_controller Enm_ctrl = new_enemy_obj.GetComponent<Enemy_controller>();

            //Set
            Enm_ctrl.Set_characterData(char_scriptable as Character_scriptable);

            return new_enemy_obj;
        }

        public ScriptableObject Get_scriptable(string path)
        {
            ScriptableObject scriptable = Resources.Load<ScriptableObject>(path);
            return scriptable;
        }
        public ScriptableObject[] Get_all_scriptable(string path)
        {
            ScriptableObject[] scriptables = Resources.LoadAll<ScriptableObject>(path);
            return scriptables;
        }

        public Material Get_material(string path)
        {
            Material material = Resources.Load<Material>(path);
            return material;
        }

        public Sprite Get_sprite(string path)
        {
            Sprite sprite = Resources.Load<Sprite>(path);
            return sprite;
        }

        /// SOUNDS
        private void Active_3D_audio_source(AudioSource source)
        {
            if (source == null) { return; }

            source.spatialBlend = 1f;

            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = 0;
            source.maxDistance = 12.5f;
        }
        public void Create_sound(string name, string clip_path, Vector2 pos)
        {
            //Create
            GameObject sound_obj = new GameObject(name);
            sound_obj.transform.position = pos;

            AudioSource audio_source = sound_obj.AddComponent<AudioSource>();
            Active_3D_audio_source(audio_source);

            //Set
            audio_source.clip = Get_audio_clip(clip_path);
            audio_source.pitch = Random.Range(0.9f, 1.1f);

            audio_source.Play();
        }
        public void Create_sound(string name, AudioClip audio_clip, Vector2 pos)
        {
            //Create
            GameObject sound_obj = new GameObject(name);
            sound_obj.transform.position = pos;

            AudioSource audio_source = sound_obj.AddComponent<AudioSource>();
            Active_3D_audio_source(audio_source);

            //Set
            audio_source.clip = audio_clip;
            audio_source.pitch = Random.Range(0.9f, 1.1f);

            audio_source.Play();
        }

        public void Create_2d_sound(string name, string clip_path)
        {
            //Create
            GameObject sound_obj = new GameObject(name);

            AudioSource audio_source = sound_obj.AddComponent<AudioSource>();

            //Set
            audio_source.clip = Get_audio_clip(clip_path);
            audio_source.pitch = Random.Range(0.9f, 1.1f);

            audio_source.Play();
        }

        public AudioClip Get_audio_clip(string clip_path)
        {
            AudioClip audio_clip = Resources.Load<AudioClip>(clip_path);
            return audio_clip;
        }

        /// UI
        private Coroutine Pop_effect_routine;
        private List<GameObject> Pop_queue_objects = new List<GameObject>();

        public void Do_UI_pop_effect(GameObject UI_obj)
        {
            // Set
            Pop_queue_objects.Add(UI_obj);

            if (Pop_effect_routine != null)
                return;

            float Duration = 0.08f;
            float scaleMultiplier = 1.1f;
           
            AnimationCurve Anim_curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            // Call
            Pop_effect_routine = StartCoroutine(PopRoutine(scaleMultiplier, Duration, Anim_curve));
        }

        IEnumerator PopRoutine(float sc_mult, float dur, AnimationCurve curve)
        {
            while (Pop_queue_objects.Count > 0)
            {
                RectTransform rect = Pop_queue_objects[0].GetComponent<RectTransform>();
                Vector3 baseScale = rect.localScale;

                yield return ScaleTo(rect, baseScale * sc_mult, dur, curve);
                yield return ScaleTo(rect, baseScale, dur, curve);

                // Set
                Pop_queue_objects.RemoveAt(0);
            }

            Pop_effect_routine = null;
        }

        IEnumerator ScaleTo(RectTransform rect, Vector3 to, float dur, AnimationCurve curve)
        {
            Vector3 from = rect.localScale;
            float t = 0f;
            float invDur = dur <= 0f ? 1f : 1f / dur;

            while (t < 1f)
            {
                t += Time.deltaTime * invDur;

                float eased = curve.Evaluate(Mathf.Clamp01(t));
                rect.localScale = Vector3.LerpUnclamped(from, to, eased);

                yield return null;
            }

            rect.localScale = to;
        }

        Coroutine Fade_effect_routine;
        private List<GameObject> Fade_queue_objects = new List<GameObject>();

        public void Do_UI_fade_effect(GameObject UI_obj)
        {
            // Set
            Fade_queue_objects.Add(UI_obj);

            if(Fade_effect_routine != null)
                return;

            float Duration = 0.25f;

            if (UI_obj.TryGetComponent<CanvasGroup>(out CanvasGroup cvn_gp))
            {
                switch(cvn_gp.alpha)
                {
                    case 0f:
                        Fade_effect_routine = StartCoroutine(FadeCoroutine(startAlpha: 0f, endAlpha: 1f, Duration));
                        break;
                    case 1f:
                        Fade_effect_routine = StartCoroutine(FadeCoroutine(startAlpha: 1f, endAlpha: 0f, Duration));
                        break;
                }
            }
        }

        private IEnumerator FadeCoroutine(float startAlpha, float endAlpha, float duration)
        {
            while(Fade_queue_objects.Count > 0)
            {
                CanvasGroup canvas_gp = Fade_queue_objects[0].GetComponent<CanvasGroup>();
                float time = 0f;

                // Set
                canvas_gp.alpha = startAlpha;

                while (time < duration)
                {
                    time += Time.deltaTime;

                    canvas_gp.alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
                    yield return null;
                }

                // Ensure the final alpha is set
                canvas_gp.alpha = endAlpha;

                // Remove from queue
                Fade_queue_objects.RemoveAt(0);
            }

            // Clear
            Fade_effect_routine = null;
        }

        /// SPECIALIZED
        public List<ScriptableObject> Get_enemies_from_menance_class(Character_scriptable.Character_menance_class enemy_class)
        {
            List<ScriptableObject> enemies = new List<ScriptableObject>();

            enemies.AddRange(Resources.LoadAll<ScriptableObject>("Scriptables/Enemies/"));

            // Filter enemies by menance class
            enemies.RemoveAll(enemy => 
            {
                if (enemy is Character_scriptable character)
                {
                    return character.Menance_class != enemy_class;
                }

                return true; // If not a Character_scriptable, remove it
            });

            return enemies;
        }

        [System.Serializable]
        public class Recipe
        {
            public string Recipe_name;
            public List<string> Materials_resources_path = new List<string>();
            public string Craft_result_resources_path;
        }

        private List<Recipe> Primitive_recipes_table;

        [System.Serializable]
        public class Converted_recipe
        {
            public string Recipe_name;
            public List<Item_scriptable> Recipe_materials = new List<Item_scriptable>();
            public Item_scriptable Craft_result;
        }

        [SerializeField] internal List<Converted_recipe> Recipes_table = new List<Converted_recipe>();

        // Save to JSON
        //Game_utils.Instance.Export_to_Json(Recipes_table, folder_path: "Resources/JSON/Recipes", file_name: "Recipes_table");

        private void Get_all_recipes()
        {
            // Get JSON
            Primitive_recipes_table = Read_from_Json<List<Recipe>>("JSON/Recipes/Recipes_table");

            foreach(Recipe recipe in Primitive_recipes_table)
            {
                Converted_recipe converted_Recipe = new Converted_recipe();

                // Set
                converted_Recipe.Recipe_name = recipe.Recipe_name;

                foreach(string material_path in recipe.Materials_resources_path)
                {
                    Item_scriptable item_scpt = Get_scriptable(material_path) as Item_scriptable;

                    // Add
                    converted_Recipe.Recipe_materials.Add(item_scpt);
                }

                converted_Recipe.Craft_result = Get_scriptable(recipe.Craft_result_resources_path) as Item_scriptable;

                // Add
                Recipes_table.Add(converted_Recipe);
            }
        }

        public Item_scriptable Get_recipe_result(List<GameObject> materials_list, bool allowSuperset)
        {
            if (materials_list == null || materials_list.Count == 0)
                return null;

            // Get
            List<Item_scriptable> available_materials = materials_list.Select(m => m.GetComponent<Item_behaviour>().ItemData).ToList();
            List<Converted_recipe> match_recipes = Recipes_table
                .Select(r => new Converted_recipe
                {
                    Recipe_name = r.Recipe_name,
                    Recipe_materials = new List<Item_scriptable>(r.Recipe_materials),
                    Craft_result = r.Craft_result
                })
                .ToList();

            for (int i = Recipes_table.Count - 1; i >= 0; i--)
            {
                // Test recipe size
                if (available_materials.Count != match_recipes[i].Recipe_materials.Count)
                {
                    // Remove
                    match_recipes.RemoveAt(i);
                    continue;
                }

                foreach (Item_scriptable item in available_materials)
                {
                    if (!match_recipes[i].Recipe_materials.Contains(item))
                    {
                        match_recipes.RemoveAt(i);
                        break;
                    }      
                }
            }

            Debug.LogWarning(string.Join(", ", match_recipes.Select(r => r.Recipe_name)));

            if (match_recipes.Count == 0)
                return null;
            else
                return match_recipes[0].Craft_result;
        }

        /// SCRIPTING
        public void Export_to_Json(object obj, string folder_path, string file_name)
        {
            // Convert to JSON formatado (indentado)
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);

            // Save
            string path = Path.Combine(Application.dataPath + "/" + folder_path, file_name + ".json");
            File.WriteAllText(path, json);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public T Read_from_Json<T>(string resources_path)
        {
            if (!string.IsNullOrEmpty(resources_path))
            {
                TextAsset jsonFile = Resources.Load<TextAsset>(resources_path);

                if (jsonFile != null)
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<T>(jsonFile.text);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"Erro lendo JSON de Resources/{resources_path}: {ex.Message}");
                        return default;
                    }
                }
                else
                {
                    Debug.LogError($"Arquivo não encontrado em Resources/{resources_path}");
                }
            }

            return default;
        }
    }
}