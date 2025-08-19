using System.Collections;
using UnityEngine;

namespace Game.Utils
{
    public class Game_garbage_collector : MonoBehaviour
    {
        [Header("Settings")]
        [Range(0, 30f)]
        public float Garbage_read_time = 10f;

        private void Start()
        {
            StartCoroutine(Read_timer());
        }

        private void Remove_all_obsolete_audios()
        {
            // Get
            AudioSource[] allSources = FindObjectsByType<AudioSource>(sortMode: FindObjectsSortMode.None);

            foreach (var source in allSources)
            {
                if(!source.isPlaying && !source.loop)
                {
                    Destroy(source);
                }
            }

        }

        private IEnumerator Read_timer()
        {
            while (true)
            {
                yield return new WaitForSeconds(Garbage_read_time);

                Remove_all_obsolete_audios();
            }
        }
    }
}
