using Game.Events;
using Game.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Controller.Audio
{
    public class Dynamic_music_controller : MonoBehaviour
    {
        public enum Audio_souces
        {
            A,
            B
        }

        [Header("Settings")]
        public float Fade_time = 5f;
        [Range(0, 1f)]
        public float Volume_target = 0.4f;

        [System.Serializable]
        public class Music_settings
        {
            public string Music_name;
            public AudioClip Clip;
            public float Volume;
        }

        [Header("Musics")]
        public List<Music_settings> Musics = new List<Music_settings>();

        [Header("State")]
        public Audio_souces Active_source = Audio_souces.A;

        [Header("Components")]
        public AudioSource Music_source_A;
        public AudioSource Music_source_B;

        //Internal variables
        [SerializeField] internal bool IsTransitioning = false;

        private void Awake()
        {
            Game_events.Day_stage_changed.AddListener(Update_music);
        }

        private AudioClip Get_music_by_name(string name)
        {
            AudioClip clip = null;

            clip = Musics.Find(m => string.Equals(m.Music_name, name, System.StringComparison.OrdinalIgnoreCase)).Clip;

            return clip;
        }

        private void Update_music(Game_controller.Day_stages stage)
        {
            if(IsTransitioning) { return; }

            AudioClip target_clip = null;

            switch (stage)
            {
                case Game_controller.Day_stages.Day:
                    target_clip = Get_music_by_name("Day");
                    break;

                case Game_controller.Day_stages.Night:
                    target_clip = Get_music_by_name("Night");
                    break;

                case Game_controller.Day_stages.Rain:
                    break;

                case Game_controller.Day_stages.Storm:
                    break;

                case Game_controller.Day_stages.Blood_moon:
                    break;
            }

            // Fade between music sources
            switch(Active_source)
            {
                case Audio_souces.A:
                    Active_source = Audio_souces.B;
                    Set_music_to_audio_source(Music_source_B, target_clip);

                    StartCoroutine(FadeOut(Music_source_A, Fade_time, 0f));
                    break;

                case Audio_souces.B:
                    Active_source = Audio_souces.A;
                    Set_music_to_audio_source(Music_source_A, target_clip);

                    StartCoroutine(FadeOut(Music_source_B, Fade_time, 0f));
                    break;
            }

            Debug.LogWarning("Music updated to " + target_clip.name);
        }

        private void Set_music_to_audio_source(AudioSource source, AudioClip clip)
        {
            source.clip = clip;

            StartCoroutine(FadeIn(source, Fade_time, Volume_target));
        }

        private IEnumerator FadeOut(AudioSource a, float dur, float vol)
        {
            float t = 0f;
            float startVol = a.volume;

            // Out
            while (t < dur)
            {
                t += Time.deltaTime;

                // Lerp volume
                a.volume = Mathf.Lerp(startVol, vol, t / dur);

                yield return null;
            }

            // Ensure final volume is set
            a.volume = vol;
            a.Stop();
        }

        private IEnumerator FadeIn(AudioSource a, float dur, float vol)
        {
            IsTransitioning = true;

            float t = 0f;

            a.volume = 0;
            a.Play();

            // In
            while (t < dur)
            {
                t += Time.deltaTime;

                // Lerp volume
                a.volume = Mathf.Lerp(0, vol, t / dur);

                yield return null;
            }

            // Ensure final volume is set
            a.volume = vol;

            IsTransitioning = false;
        }
    }
}