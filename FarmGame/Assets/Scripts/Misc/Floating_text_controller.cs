using Game.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Utils.Misc
{
    public class Floating_text_controller : MonoBehaviour
    {
        //Internal variables
        private GameObject Player_character_obj => GameObject.FindGameObjectWithTag("Player");
        private List<GameObject> Txts_objs = new List<GameObject>();

        private void Awake()
        {
            Game_events.Player_character_took_damage.AddListener((dmg, obj) => Read_events(dmg, Player_character_obj));
            Game_events.Enemy_took_damage.AddListener(Read_events);
        }

        private void Start()
        {
            StartCoroutine(Move_floating_text());
        }

        /// CORE METHODS
        private void Read_events(float value, GameObject obj)
        {
            Draw_text(value.ToString("N2"), obj.transform.position);
        }

        /// MAIN METHODS
        private void Draw_text(string txt, Vector2 origin_pos) 
        {
            GameObject txt_obj = Game_utils.Instance.Create_prefab_from_resources("Prefabs/Misc/Floating_txt", origin_pos);

            // Set
            txt_obj.GetComponent<TextMeshPro>().text = txt;

            Txts_objs.Add(txt_obj);
        
            StartCoroutine(Kill_txt_routine(txt_obj, 0.8f));
        }

        private IEnumerator Move_floating_text()
        {
            float txt_speed = 2f;

            while(true)
            {
                if(Txts_objs.Count > 0)
                {
                    foreach(GameObject obj in Txts_objs)
                    {
                        obj.transform.Translate(0, txt_speed * Time.deltaTime, 0);
                    }
                }

                yield return null;
            }
        }

        private IEnumerator Kill_txt_routine(GameObject obj, float t)
        {
            yield return new WaitForSeconds(t);

            if (obj != null)
            {
                Txts_objs.Remove(obj);

                Destroy(obj);
            }
        }
    }
}